using Codice.CM.Common;
using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public AxleInfo[] axleInfos;
    public Rigidbody rigidBody;
    float torque = 1000;
    float angle = 45;
    float brakeTorque = 2000;
    public float jumpPower = 100000;
    public float driftPower = 1000;
    public float maxSpeed = 250;
    LogicScript logic;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();

    }

    void Update()
    {
        Drive();
        ApplyBooster();
    }

    void ApplyBooster()
    {
        bool grounded = false;
        foreach (AxleInfo axleInfo in axleInfos)
        {
            grounded |= axleInfo.leftWheel.isGrounded;
            grounded |= axleInfo.rightWheel.isGrounded;
        }
        if (grounded)
        {
            //rigidBody.constraints = RigidbodyConstraints.None;
            if (Input.GetKey(KeyCode.Space))
            {
                rigidBody.AddForce(Vector3.up * jumpPower);
            }
        }
        else
        {
            //rigidBody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            float drift = Input.GetAxis("Horizontal");
            Vector3 angularVelocity = rigidBody.angularVelocity;
            angularVelocity.y = drift/3;
            rigidBody.angularVelocity = angularVelocity;
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
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
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
        if (axleInfo.motor)
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

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
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
