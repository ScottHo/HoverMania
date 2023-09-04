using UnityEngine;

public class CarControllerScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AxleInfo[] axleInfos;
    public Rigidbody rigidBody;
    public float torque = 2000;
    public float angle = 50;
    public float brakeTorque = 2000;
    public float jumpPower = 12;
    public float maxRotationSpeed = 1500;
    public BoxCollider bottomCollider;
    AudioAction audioAction = AudioAction.None;
    bool grounded = true;
    bool canJump = false;
    float jumpTimeBuffer = 0;
    float currentFloatWheelAngle = 0f;
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
        ApplyFloatState();
        LimitAngularVelocity();
        rigidBody.AddForce(Physics.gravity * rigidBody.mass * 2);
        AudioManager.PlayEffect(audioAction, ref audioSource);
    }

    void CheckGrounded()
    {
        bool wasGrounded = grounded;
        grounded = false;
        foreach (AxleInfo axleInfo in axleInfos)
        {
            grounded |= axleInfo.leftWheel.isGrounded;
            grounded |= axleInfo.rightWheel.isGrounded;
        }
        bool justLanded = grounded && !wasGrounded;
        if (justLanded)
        {
            canJump = false;
            jumpTimeBuffer = 0f;
        }
        if (!canJump)
        {
            jumpTimeBuffer += Time.deltaTime;
            if (jumpTimeBuffer > .15)
            {
                // Only allow consecutive jumps every .25 seconds
                canJump = true;
                jumpTimeBuffer = 0;
            }
        }
    }

    void ApplyFloatState()
    {
        if (grounded)
        {
            Jump();
        }
        else
        {
            Drift();
            Hover();
        }
    }

    void Drift()
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

    void Hover()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            logic.SetBatteryDraining(true);
            rigidBody.AddForce(-Physics.gravity * rigidBody.mass * 2f);
        }
        else
        {
            logic.SetBatteryDraining(false);
        }
    }

    void Jump()
    {
        if (canJump && Input.GetKey(KeyCode.Space))
        {
            audioAction = AudioAction.Jump;
            canJump = false;
            logic.DrainBattery(500);
            rigidBody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
        }
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
                float _maxRotationSpeed = maxRotationSpeed;
                logic.SetBatteryDraining(true);
                if (Mathf.Abs(angle * Input.GetAxis("Horizontal")) > 25)
                {
                    _maxRotationSpeed = maxRotationSpeed * .75f;
                }
                if (reverse)
                {
                    if (axleInfo.leftWheel.rotationSpeed > -_maxRotationSpeed * .7f)
                    {
                        axleInfo.leftWheel.motorTorque = motor;
                    }
                    if (axleInfo.rightWheel.rotationSpeed > -_maxRotationSpeed * .7f)
                    {
                        axleInfo.rightWheel.motorTorque = motor;
                    }
                }
                else
                {
                    if (axleInfo.leftWheel.rotationSpeed < _maxRotationSpeed)
                    {
                        axleInfo.leftWheel.motorTorque = motor;
                    }
                    if (axleInfo.rightWheel.rotationSpeed < _maxRotationSpeed)
                    {
                        axleInfo.rightWheel.motorTorque = motor;
                    }
                }
            }
        }
    }

    public void UpdateWheelVisuals(AxleInfo axleInfo)
    {
        ApplyLocalPositionToVisuals(axleInfo.leftWheel, axleInfo.leftSpring, true);
        ApplyLocalPositionToVisuals(axleInfo.rightWheel, axleInfo.rightSpring, false);

    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider, GameObject spring, bool leftWheel)
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
            position = Vector3.MoveTowards(position, spring.transform.position, 10f);
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
            currentFloatWheelAngle += 2f;
        }
        else
        {
            currentFloatWheelAngle -= 4f;
        }
        currentFloatWheelAngle = Mathf.Clamp(currentFloatWheelAngle, 0f, 90f);
        float zBackup = rigidBody.transform.rotation.eulerAngles.z;
        angles.x = rigidBody.transform.rotation.eulerAngles.x;
        angles.y = rigidBody.transform.rotation.eulerAngles.y;
        angles.z = -currentFloatWheelAngle;
        if (leftWheel)
        {
            angles.z = currentFloatWheelAngle;
        }
        angles.z += zBackup;
        return angles;
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public GameObject leftSpring;
    public WheelCollider rightWheel;
    public GameObject rightSpring;
    public bool motor;
    public bool steering;
}
