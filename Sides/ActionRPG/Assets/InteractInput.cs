using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractInput : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI textOnScreen;
    [SerializeField] MouseInput mouseInput;
    [HideInInspector]
    public InteractableObject hoveringOverObject;
    CharacterMovement characterMovement;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        CheckInteractableObject();

        if (Input.GetMouseButtonDown(0))
        {
            if (hoveringOverObject != null)
            {
                hoveringOverObject.Interact();
            }
            else 
            {
               characterMovement.SetDestination(mouseInput.mouseInputPosition);
            }
        }
    }

    private void CheckInteractableObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            InteractableObject interactableObject = hit.transform.GetComponent<InteractableObject>();
            if (interactableObject != null)
            {
                hoveringOverObject = interactableObject;
                textOnScreen.text = interactableObject.objectName;
            }
            else 
            {
                hoveringOverObject = null;
                textOnScreen.text = "";
            }
        }
    }
}
