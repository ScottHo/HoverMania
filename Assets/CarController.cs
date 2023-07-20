using System;
using System.IO;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public AxleInfo[] axleInfos;
    public Rigidbody rigidBody;
    float torque = 360;
    float angle = 35;
    float brakeTorque = 1000;
    // Start is called before the first frame update
    void Start()
    {
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
    // Update is called once per frame
    void Update()
    {
        float motor = torque * Input.GetAxis("Vertical");

        float steering = angle * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                bool isMovingForward = axleInfo.leftWheel.rotationSpeed > 5;
                bool reverse = motor < 0;
                if (reverse && isMovingForward)
                {
                    axleInfo.leftWheel.brakeTorque = brakeTorque;
                    axleInfo.rightWheel.brakeTorque = brakeTorque;
                    axleInfo.leftWheel.motorTorque = 0;
                    axleInfo.rightWheel.motorTorque = 0;
                }
                else
                {
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;
                    if (reverse)
                    {
                        axleInfo.leftWheel.motorTorque = motor/2;
                        axleInfo.rightWheel.motorTorque = motor/2;
                    }
                    else
                    {
                        axleInfo.leftWheel.motorTorque = motor;
                        axleInfo.rightWheel.motorTorque = motor;
                    }
                }
                // ComputeSpeed();
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }
    private void ComputeSpeed()
    {
        // Main speed in km/h.
        var speed = rigidBody.velocity.magnitude * 3.6f;

        // wheel circumference in meter. (radius in meter)
        float circumferenceLeft = 2 * Mathf.PI * axleInfos[0].leftWheel.radius;
        float circumferenceRight = 2 * Mathf.PI * axleInfos[0].rightWheel.radius;

        // Speed in meter per minute. (from radius in meter and revolutions per minute)
        var speedLeft = axleInfos[0].leftWheel.rpm * circumferenceLeft;
        var speedRight = axleInfos[0].rightWheel.rpm * circumferenceRight;

        // Convert meter per minute to kilometers per hour (km/h) (* 60 minutes / 1000 meter)
        speedLeft *= 60f / 1000f;
        speedRight *= 60f / 1000f;
        var s = "Speed: " + speed + "; speed L/R: " + speedLeft + " / " + speedRight;
        Debug.Log(s);
    }

}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}
