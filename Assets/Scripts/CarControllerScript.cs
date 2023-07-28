using System;
using UnityEngine;

public class CarControllerScript : MonoBehaviour
{
    public AxleInfo[] axleInfos;
    public Rigidbody rigidBody;
    float torque = 2000;
    float angle = 45;
    float brakeTorque = 3000;
    float jumpPower = 350000;
    float maxSpeed = 250;
    bool grounded = true;
    bool jumping = false;
    float timeSinceLastJump = 0;
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
        CheckGrounded();
        Drive();
        ApplyBooster();
        // Double the gravity for fun
        rigidBody.AddForce(Physics.gravity * rigidBody.mass);
    }

    void CheckGrounded()
    {
        grounded = false;
        foreach (AxleInfo axleInfo in axleInfos)
        {
            grounded |= axleInfo.leftWheel.isGrounded;
            grounded |= axleInfo.rightWheel.isGrounded;
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
                    jumping = true;
                    logic.DrainBattery(500);
                    rigidBody.AddForce(Vector3.up * jumpPower);
                }
            }
        }
        else
        {
            float drift = Input.GetAxis("Horizontal");
            Vector3 angularVelocity = rigidBody.angularVelocity;
            angularVelocity.y = drift/2;
            rigidBody.angularVelocity = angularVelocity;
            float speed = Mathf.Abs(rigidBody.velocity.sqrMagnitude);
            if (speed <= maxSpeed)
            {
                float forwardDrift = Input.GetAxis("Vertical");
                rigidBody.AddForce(rigidBody.transform.forward * forwardDrift * 4000);
            }
        }
        LimitAngularVelocity();
    }

    void LimitAngularVelocity()
    {
        Vector3 angularVelocity = rigidBody.angularVelocity;
        angularVelocity.x = Mathf.Clamp(angularVelocity.x, -.1f, .1f);
        angularVelocity.z = Mathf.Clamp(angularVelocity.z, -.1f, .1f);
        angularVelocity.y = Mathf.Clamp(angularVelocity.y, -angle*(float)Math.PI / 180.0f, angle*(float)Math.PI/180.0f);
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
        if (axleInfo.motor && grounded)
        {
            bool isMovingForward = axleInfo.leftWheel.rotationSpeed > 5;
            bool isMovingBackwards = axleInfo.leftWheel.rotationSpeed < -5;
            bool reverse = motor < 0;
            if (motor == 0 || (reverse && isMovingForward) || (!reverse && isMovingBackwards))
            {
                logic.SetBatteryDraining(false);
                axleInfo.leftWheel.brakeTorque = brakeTorque;
                axleInfo.rightWheel.brakeTorque = brakeTorque;
                axleInfo.leftWheel.motorTorque = 0;
                axleInfo.rightWheel.motorTorque = 0;
            }
            else
            {
                logic.SetBatteryDraining(true);
                axleInfo.leftWheel.brakeTorque = 0;
                axleInfo.rightWheel.brakeTorque = 0;

                float speed = Mathf.Abs(rigidBody.velocity.sqrMagnitude);
                if (reverse)
                {
                    if (speed < maxSpeed * .5)
                    {
                        axleInfo.leftWheel.motorTorque = motor;
                        axleInfo.rightWheel.motorTorque = motor;
                    }
                    else 
                    {
                        axleInfo.leftWheel.motorTorque = 0;
                        axleInfo.rightWheel.motorTorque = 0;
                    }
                }
                else
                {
                    if (speed < maxSpeed)
                    {
                        axleInfo.leftWheel.motorTorque = motor;
                        axleInfo.rightWheel.motorTorque = motor;
                    }
                    else
                    {
                        axleInfo.leftWheel.motorTorque = 0;
                        axleInfo.rightWheel.motorTorque = 0;
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
        angles.z = -currentFloatWheelAngle;
        if (leftWheel)
        {
            angles.z = currentFloatWheelAngle;
        }
        angles.x = 0;
        angles.y = 0;
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
