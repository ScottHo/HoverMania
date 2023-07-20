using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Vector2 turn;
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        //turn.x += Input.GetAxis("Mouse X");
        //turn.y += Input.GetAxis("Mouse Y");
        //transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);
    }
}
