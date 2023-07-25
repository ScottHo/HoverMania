using UnityEngine;

public class CarController : MonoBehaviour
{
    public AxleInfo[] axleInfos;
    public Rigidbody rigidBody;
    float torque = 360;
    float angle = 35;
    float brakeTorque = 1000;
    LogicScript logic;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();

    }

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
                if (motor == 0 || (reverse && isMovingForward))
                {
                    axleInfo.leftWheel.brakeTorque = brakeTorque;
                    axleInfo.rightWheel.brakeTorque = brakeTorque;
                    axleInfo.leftWheel.motorTorque = 0;
                    axleInfo.rightWheel.motorTorque = 0;
                    logic.SetBatteryDraining(false);

                }
                else
                {
                    logic.SetBatteryDraining(true);
                    axleInfo.leftWheel.brakeTorque = 0;
                    axleInfo.rightWheel.brakeTorque = 0;
                    if (reverse)
                    {
                        axleInfo.leftWheel.motorTorque = motor * (float) .75;
                        axleInfo.rightWheel.motorTorque = motor * (float) .75;
                    }
                    else
                    {
                        axleInfo.leftWheel.motorTorque = motor;
                        axleInfo.rightWheel.motorTorque = motor;
                    }
                }
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
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
