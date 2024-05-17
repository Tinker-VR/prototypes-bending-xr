using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmPointer : MonoBehaviour
{
    [SerializeField] private Transform handPointerPose;
    [SerializeField] private GameObject reticleInstance;
    [SerializeField] private LayerMask groundLayerMask;

    public Earth CurrentTarget { get; private set; }
    public Vector3 CurrentTargetPosition { get; private set; }
    public bool IsTargetLocked { get; private set; }

    void Update()
    {
        if (IsTargetLocked) return;
        UpdatereticleInstancePosition();
    }

    private void UpdatereticleInstancePosition()
    {
        if (IsPalmPointingDown())
        {
            RaycastHit hit;
            if (Physics.Raycast(handPointerPose.position, handPointerPose.forward, out hit, 100f, groundLayerMask))
            {
                Earth earth = hit.collider.GetComponent<Earth>();
                if (earth != null)
                {
                    CurrentTarget = earth;
                    
                    CurrentTargetPosition = hit.point;
                    reticleInstance.SetActive(true);
                    reticleInstance.transform.position = hit.point;
                }
            }
            else
            {
                reticleInstance.SetActive(false);
            }
        }
        else
        {
            reticleInstance.SetActive(false);
        }
    }

    private bool IsPalmPointingDown()
    {
        return Vector3.Angle(handPointerPose.forward, Vector3.down) <= 80.0f;
    }

    public void ToggleTargetLock(bool lockTarget)
    {
        IsTargetLocked = lockTarget;
        reticleInstance.SetActive(!lockTarget);
    }
}
