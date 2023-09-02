using UnityEngine;

public class CarControllerScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AxleInfo[] axleInfos;
    public Rigidbody rigidBody;
    float torque = 2000;
    float angle = 50;
    float brakeTorque = 2000;
    float jumpPower = 12;
    AudioAction audioAction = AudioAction.None;
    bool grounded = true;
    bool newlyFloating = false;
    bool jumping = false;
    float timeSinceLastJump = 0;
    float currentFloatWheelAngle = 0f;
    float currentFloatSpinWheelAngle = 0f;
    LevelLogicScript logic;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("LevelLogic").GetComponent<LevelLogicScript>();
    }

    void FixedUpdate()
    {
        if (logic.gameIsOver)
            return;
        audioAction = AudioAction.None;
        CheckGrounded();
        Drive();
        ApplyBooster();
        // Double the gravity for fun
        rigidBody.AddForce(Physics.gravity * rigidBody.mass);
        AudioManager.PlayEffect(audioAction, ref audioSource);
    }

    void CheckGrounded()
    {
        bool wasGrounded = grounded;
        newlyFloating = false;
        grounded = false;
        foreach (AxleInfo axleInfo in axleInfos)
        {
            grounded |= axleInfo.leftWheel.isGrounded;
            grounded |= axleInfo.rightWheel.isGrounded;
        }
        if (wasGrounded && !grounded)
        {
            newlyFloating = true;
        }
        if (jumping)
        {
            timeSinceLastJump += Time.deltaTime;
            if (timeSinceLastJump > 0.25)
            {
                // Only allow consecutive jumps every .25 seconds
                jumping = false;
                timeSinceLastJump = 0;
            }
        }
    }

    void ApplyBooster()
    {
        if (grounded)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (!jumping)
                {
                    audioAction = AudioAction.Jump;
                    jumping = true;
                    logic.DrainBattery(500);
                    rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
                }
            }
        }
        else
        {
            float drift = Input.GetAxis("Horizontal");
            Vector3 angularVelocity = rigidBody.angularVelocity;
            angularVelocity.y = drift;
            rigidBody.angularVelocity = angularVelocity;
            float maxSpeed = 10;
            float speed = rigidBody.velocity.magnitude;
            if (speed <= maxSpeed)
            {
                float forwardDrift = Input.GetAxis("Vertical");
                rigidBody.AddForce(rigidBody.transform.forward * forwardDrift * 2000);
            }
        }
        LimitAngularVelocity();
    }

    void LimitAngularVelocity()
    {
        Vector3 angularVelocity = rigidBody.angularVelocity;
        float groundedFlipVelocity = 1f;
        float airFlipVelocity = .2f;
        if (grounded)
        {
            angularVelocity.x = Mathf.Clamp(angularVelocity.x, -groundedFlipVelocity, groundedFlipVelocity);
            angularVelocity.z = Mathf.Clamp(angularVelocity.z, -groundedFlipVelocity, groundedFlipVelocity);
        }
        else
        {
            angularVelocity.x = Mathf.Clamp(angularVelocity.x, -airFlipVelocity, airFlipVelocity);
            angularVelocity.z = Mathf.Clamp(angularVelocity.z, -airFlipVelocity, airFlipVelocity);
        }
        rigidBody.angularVelocity = angularVelocity;
    }

    void Drive()
    {
        foreach (AxleInfo axleInfo in axleInfos)
        {
            Steer(axleInfo);
            Motor(axleInfo);
            UpdateWheelVisuals(axleInfo);
        }
    }

    void Steer(AxleInfo axleInfo)
    {
        float steering = angle * Input.GetAxis("Horizontal");
        if (axleInfo.steering)
        {
            axleInfo.leftWheel.steerAngle = steering;
            axleInfo.rightWheel.steerAngle = steering;
        }
    }

    void Motor(AxleInfo axleInfo)
    {
        float motor = torque * Input.GetAxis("Vertical");
        float brakePower = brakeTorque;
        bool isMovingForward = axleInfo.leftWheel.rotationSpeed > 50;
        bool isMovingBackwards = axleInfo.leftWheel.rotationSpeed < -50;
        bool reverse = motor < 0;
        if (motor == 0)
        {
            brakePower = brakePower / 2;
        }
        axleInfo.leftWheel.brakeTorque = 0;
        axleInfo.rightWheel.brakeTorque = 0;
        axleInfo.leftWheel.motorTorque = 0;
        axleInfo.rightWheel.motorTorque = 0;
        if (axleInfo.motor && grounded)
        {
            if (motor == 0 || (reverse && isMovingForward) || (!reverse && isMovingBackwards))
            {
                logic.SetBatteryDraining(false);
                axleInfo.leftWheel.brakeTorque = brakePower;
                axleInfo.rightWheel.brakeTorque = brakePower;
            }
            else
            {
                logic.SetBatteryDraining(true);

                float maxRotationSpeed = 1200;
                if (Mathf.Abs(angle * Input.GetAxis("Horizontal")) > 25)
                {
                    maxRotationSpeed = maxRotationSpeed/2;
                }
                if (reverse)
                {
                    if (axleInfo.leftWheel.rotationSpeed > -maxRotationSpeed * .7f)
                    {
                        axleInfo.leftWheel.motorTorque = motor;
                    }
                    if (axleInfo.rightWheel.rotationSpeed > -maxRotationSpeed * .7f)
                    {
                        axleInfo.rightWheel.motorTorque = motor;
                    }
                }
                else
                {
                    if (axleInfo.leftWheel.rotationSpeed < maxRotationSpeed)
                    {
                        axleInfo.leftWheel.motorTorque = motor;
                    }
                    if (axleInfo.rightWheel.rotationSpeed < maxRotationSpeed)
                    {
                        axleInfo.rightWheel.motorTorque = motor;
                    }
                }
            }
        }
    }

    public void UpdateWheelVisuals(AxleInfo axleInfo)
    {
        ApplyLocalPositionToVisuals(axleInfo.leftWheel, true);
        ApplyLocalPositionToVisuals(axleInfo.rightWheel, false);

    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider, bool leftWheel)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        if (!grounded)
        {
            Vector3 angles = HoverModeAngles(true, leftWheel, rotation.eulerAngles);
            rotation.eulerAngles = angles;
        }
        else
        {
            if (currentFloatWheelAngle > 0)
            {
                Vector3 angles = HoverModeAngles(false, leftWheel, rotation.eulerAngles);
                rotation.eulerAngles = angles;
            }
        }
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    Vector3 HoverModeAngles(bool hover, bool leftWheel, Vector3 angles)
    {
        if (hover)
        {
            currentFloatWheelAngle += 3f;
        }
        else
        {
            currentFloatWheelAngle -= 3f;
        }
        currentFloatWheelAngle = Mathf.Clamp(currentFloatWheelAngle, 0f, 90f);
        if (newlyFloating)
        {
            currentFloatSpinWheelAngle = 0;
        }
        else
        {
            currentFloatSpinWheelAngle += 4;
        }
        angles.x = 0;
        angles.y = currentFloatSpinWheelAngle;
        angles.z = -currentFloatWheelAngle;
        if (leftWheel)
        {
            angles.z = currentFloatWheelAngle;
        }
        return angles;
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}
