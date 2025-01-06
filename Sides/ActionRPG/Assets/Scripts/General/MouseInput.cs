using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MouseInput : MonoBehaviour
{
    public Vector3 mouseInputPosition;
    public InteractableObject hoveringOverObject;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            mouseInputPosition = hit.point;
            hoveringOverObject = hit.collider.GetComponent<InteractableObject>();
        }
        else
        {
            hoveringOverObject = null;
        }
    }
}
