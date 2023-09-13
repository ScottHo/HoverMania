using UnityEngine;

public class CarControllerScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource audioSourceAmbience;
    public AxleInfo[] axleInfos;
    public Rigidbody rigidBody;
    public Light bottomGlow;
    public float torque;
    public float angle;
    public float brakeTorque;
    public float jumpPower;
    AudioAction audioAction = AudioAction.None;
    bool grounded = true;
    bool canJump = false;
    float jumpTimeBuffer = 0;
    bool spacePressed = false;
    float hoverUpStep = 0;
    LevelLogicScript logic;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("LevelLogic").GetComponent<LevelLogicScript>();
    }

    void FixedUpdate()
    {
        if (logic.gameIsOver || logic.fading)
        {
            audioSourceAmbience.Stop();
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            return;
        }
        bottomGlow.enabled = false;
        audioAction = AudioAction.None;
        CheckGrounded();
        Drive();
        ApplyFloatState();
        LimitAngularVelocity();
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
            if (jumpTimeBuffer > .05)
            {
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
        spacePressed = Input.GetKey(KeyCode.Space);
    }

    void Drift()
    {
        rigidBody.drag = 0f;
        float drift = Input.GetAxis("Horizontal");
        Vector3 angularVelocity = rigidBody.angularVelocity;
        angularVelocity.y = drift;
        rigidBody.angularVelocity = angularVelocity;
        float maxSpeed = 15;
        float speed = rigidBody.velocity.magnitude;
        if (speed <= maxSpeed)
        {
            float forwardDrift = Input.GetAxis("Vertical");
            rigidBody.AddForce(rigidBody.transform.forward * forwardDrift * 1500);
        }
    }

    void Hover()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            bottomGlow.enabled = true;
            bottomGlow.color = Color.green;
            AudioManager.PlayAmbience(AudioAction.Hover, ref audioSourceAmbience);
            logic.SetBatteryDraining(true);
            rigidBody.AddForce(-Physics.gravity * rigidBody.mass * .6f);
        }
        else
        {
            bottomGlow.enabled = true;
            bottomGlow.color = Color.yellow;
            AudioManager.PlayAmbience(AudioAction.Idle, ref audioSourceAmbience);
            logic.SetBatteryDraining(false);
        }
    }

    void Jump()
    {
        if (canJump && Input.GetKey(KeyCode.Space) && !spacePressed)
        {
            audioAction = AudioAction.Jump;
            canJump = false;
            logic.DrainBattery(5);
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
        if (axleInfo.steering)
        {
            float steering = angle * Input.GetAxis("Horizontal");
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
            brakePower = brakePower * .5f;
        }
        axleInfo.leftWheel.brakeTorque = 0;
        axleInfo.rightWheel.brakeTorque = 0;
        axleInfo.leftWheel.motorTorque = 0;
        axleInfo.rightWheel.motorTorque = 0;
        if (axleInfo.motor && grounded)
        {
            if (motor == 0 || (reverse && isMovingForward) || (!reverse && isMovingBackwards))
            {
                AudioManager.PlayAmbience(AudioAction.Idle, ref audioSourceAmbience);
                logic.SetBatteryDraining(false);
                axleInfo.leftWheel.brakeTorque = brakePower;
                axleInfo.rightWheel.brakeTorque = brakePower;
                rigidBody.drag = 0f;
            }
            else
            {
                float speed = rigidBody.velocity.magnitude;
                if (speed > 4)
                {
                    rigidBody.drag = speed * speed * .03f;
                    float currAngle = Mathf.Abs(angle * Input.GetAxis("Horizontal"));
                    if (currAngle > 0)
                    {
                        motor *= .2f;
                        if (currAngle > angle-5)
                            motor *= 1.7f;
                    }
                }
                else
                {
                    rigidBody.drag = 0f;
                }
                AudioManager.PlayAmbience(AudioAction.Drive, ref audioSourceAmbience);
                logic.SetBatteryDraining(true);
                
                if (reverse)
                {
                    bottomGlow.enabled = true;
                    bottomGlow.color = Color.red;
                    axleInfo.leftWheel.motorTorque = motor * .7f;
                    axleInfo.leftWheel.motorTorque = motor * .7f;
                }
                else
                {
                    bottomGlow.enabled = true;
                    bottomGlow.color = Color.cyan;
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                }
            }
        }
    }

    public void UpdateWheelVisuals(AxleInfo axleInfo)
    {
        ApplyLocalPositionToVisuals(axleInfo.leftWheel, axleInfo.leftOrig, axleInfo.leftSpring);
        ApplyLocalPositionToVisuals(axleInfo.rightWheel, axleInfo.rightOrig, axleInfo.rightSpring);
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider, GameObject orig, GameObject spring)
    {
        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        if (!grounded)
        {
            if (hoverUpStep < 1.0f)
            {
                hoverUpStep += .02f;
                rotation = Quaternion.Slerp(orig.transform.rotation, spring.transform.rotation, hoverUpStep);
                position = Vector3.MoveTowards(orig.transform.position, spring.transform.position, .2f);
            }
            else
            {
                hoverUpStep = 1.0f;
                rotation = spring.transform.rotation;
                position = spring.transform.position;
            }
        }
        else
        {
            if (hoverUpStep > 0.0f)
            {
                hoverUpStep -= .05f;
                rotation = Quaternion.Slerp(orig.transform.rotation, spring.transform.rotation, hoverUpStep);
                position = Vector3.MoveTowards(spring.transform.position, orig.transform.position, .4f);
            }
            else
            {
                hoverUpStep = 0.0f;
                collider.GetWorldPose(out position, out rotation);
            }
        }
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public GameObject leftSpring;
    public GameObject leftOrig;
    public WheelCollider rightWheel;
    public GameObject rightSpring;
    public GameObject rightOrig;
    public bool motor;
    public bool steering;
}
