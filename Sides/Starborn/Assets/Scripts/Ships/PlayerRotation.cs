using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public float tiltAngle = 50f;

    void Update()
    {
        float y = Input.GetAxis("Horizontal");
        float x = Input.GetAxis("Vertical");
        float tilt = Mathf.Abs(Input.GetAxis("Vertical"));
        if (y != 0 || x != 0)
        {
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(tilt * tiltAngle, angle, -y * tiltAngle);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);

        }

    }
}
