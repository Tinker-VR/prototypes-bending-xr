using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class HandGestureManager : MonoBehaviour
{
    public event Action OnPinchRelease;

    [SerializeField] private OVRHand ovrHand;
    [SerializeField] private PalmPointer palmPointer;

    private GameObject currentRock;
    private Vector3 lastHandPosition;
    private float lastPinchTime;

    private bool _isCloseFist;

    private const float upwardGestureSpeed = 0.5f; // Speed threshold for quick upward motion


    void Start()
    {
    }

    public void CloseFistValidation(bool _toggle)
    { 
        _isCloseFist = !_isCloseFist;
        Debug.Log($"Is Fist {_toggle}");
    }

    void Update()
    {
        HandlePinchGesture();
        HandleLiftGesture();
    }

    private void HandlePinchGesture()
    {
        bool isPinching = ovrHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
        if (_isCloseFist)
        {
            if (!palmPointer.IsTargetLocked)
            {
                if (palmPointer.CurrentTarget.earthType == EarthType.Rock)
                {
                    currentRock = palmPointer.CurrentTarget.gameObject;
                }

                palmPointer.ToggleTargetLock(true);
                lastHandPosition = ovrHand.transform.position; // Store the hand position at the moment of locking the target
                lastPinchTime = Time.time;
            }
        }
        else
        {
            if (palmPointer.IsTargetLocked)
            {
                if(currentRock)
                {
                    ReleaseRock();
                }
                palmPointer.ToggleTargetLock(false);
                OnPinchRelease?.Invoke();
            }
        }
    }
    
    private void HandleLiftGesture()
    {
        if (!palmPointer.IsTargetLocked) return;

        Vector3 currentHandPosition = ovrHand.transform.position;
        float distanceMovedUpwards = currentHandPosition.y - lastHandPosition.y;
        
        if(currentRock == null)
        {
            if (distanceMovedUpwards < 0.2f) return;

            float timeTaken = Time.time - lastPinchTime;
            float scale = Mathf.Lerp(0.75f, 3f, Mathf.InverseLerp(0.5f, 2.5f, timeTaken));
            
            currentRock = EarthManager.Instance.SpawnRock(palmPointer.CurrentTargetPosition, scale, ovrHand.gameObject);
        }
        else
        {
            if (distanceMovedUpwards < 0.05f) return;

            if(currentRock.GetComponent<RockMovement>() && currentRock.GetComponent<RockMovement>().IsLifted)
            {
                return;
            }
            
            EarthManager.Instance.LiftRock(currentRock, ovrHand.gameObject);
        }
    }

    private void ReleaseRock()
    {
        currentRock.GetComponent<RockMovement>().StopFollowingHand();
        currentRock = null;
    }
}
