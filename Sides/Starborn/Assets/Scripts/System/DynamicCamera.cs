using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    private Transform cameraTarget;

    private void Awake()
    {
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            cameraTarget = GameManager.Instance.player.transform;
        }
    }

    void Update()
    {
        if (cameraTarget != null)
        {
            transform.position = new Vector3(cameraTarget.transform.position.x, cameraTarget.transform.position.y, -100);
        }
    }
}
