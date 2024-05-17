using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeRaycaster : MonoBehaviour
{
    // public transform vrCamera;
    public LayerMask interactableLayer; // Set this to the layer your interactable objects are on.
    public float maxRayDistance = 100.0f;

    private GazeInteractable currentGazeTarget;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, interactableLayer))
        {
            // Check if the object hit has the GazeInteractable script
            GazeInteractable interactable = hit.collider.GetComponent<GazeInteractable>();
            if (interactable != null)
            {
                if (currentGazeTarget != interactable)
                {
                    // Call OnPointerExit on the last target
                    if (currentGazeTarget != null)
                    {
                        currentGazeTarget.OnPointerExit(null);
                    }

                    // Set the new target and call OnPointerEnter
                    currentGazeTarget = interactable;
                    currentGazeTarget.OnPointerEnter(null);
                }
            }
        }
        else
        {
            // No hit with interactable layer, so call OnPointerExit on the last target if there was one
            if (currentGazeTarget != null)
            {
                currentGazeTarget.OnPointerExit(null);
                currentGazeTarget = null;
            }
        }
    }
}
