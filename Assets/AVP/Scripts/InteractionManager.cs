using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    public OVRHand rightHand;
    
    public CanvasGroup uiCanvasGroup;

    private GazeInteractable currentGazed;
    private GazeInteractable currentPinched;

    public bool IsIndexPinching = false;
    
    private Vector3 pinchStartPosition;
    private Vector3 lastPinchPosition;
    private Vector3 pinchMoveDelta;
    
    private float pinchStartTime;
    private const float holdThreshold = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            if(IsIndexPinching)
            {
                UpdatePinchMovement();
                return;
            }
            else
            {
                StartPinching();
            }
            
            if(!AppMenu.Instance.IsVisible && AppMenu.Instance.CurrentApp == null)
            {
                AppMenu.Instance.ShowMenu();
            }
        }
        else
        {
            if(IsIndexPinching)
            {
                EndPinching();
            }
        }
    }

    private void StartPinching()
    {
        if (currentGazed != null)
        {
            currentGazed.OnPinchDown();
            SetPinchedObject(currentGazed);
        }
        
        IsIndexPinching = true;

        pinchStartPosition = rightHand.transform.position;
        lastPinchPosition = pinchStartPosition;

        pinchStartTime = Time.time; 
    }

    private void UpdatePinchMovement()
    {
        Vector3 currentPosition = rightHand.transform.position;
        pinchMoveDelta = new Vector3(currentPosition.x - lastPinchPosition.x, currentPosition.y - lastPinchPosition.y, currentPosition.z - lastPinchPosition.z);
        lastPinchPosition = currentPosition;
    }

    private void EndPinching()
    {
        float pinchDuration = Time.time - pinchStartTime;

        if (pinchDuration < holdThreshold)
        {
            // Quick pinch
            if (currentGazed != null)
            {
                currentGazed.OnPinchPress();
            }
        }
        else
        {
            // if (currentPinched != null)
            // {
            // }
            // // Pinch hold
            // You can implement specific actions for pinch hold if needed
        }
        
        if (currentPinched != null)
        {
            currentPinched.OnPinchRelease();
            ClearPinchedObject(currentPinched);
        }

        pinchMoveDelta = Vector3.zero;

        IsIndexPinching = false;
    }

    public Vector3 GetPinchMoveDelta()
    {
        return pinchMoveDelta;
    }

    public GazeInteractable GetCurrentGazed()
    {
        return currentGazed;
    }
    
    public GazeInteractable GetCurrentPinched()
    {
        return currentPinched;
    }

    // Call this from your GazeInteractable objects when they are gazed upon
    public void SetGazedObject(GazeInteractable gazedObject)
    {
        if (currentGazed != null && currentGazed != gazedObject && currentGazed.IsHovered == true)
        {
            currentGazed.IsHovered = false; // Ensure isHovered is updated
            currentGazed.EndHoverAnimation(); // Force unhighlight
        }
        currentGazed = gazedObject;
    }

    // Call this from your GazeInteractable objects when the gaze exits
    public void ClearGazedObject(GazeInteractable gazedObject)
    {
        if (currentGazed == gazedObject)
        {
            currentGazed = null;
        }
    }

    // Call this from your GazeInteractable objects when they are gazed upon
    public void SetPinchedObject(GazeInteractable pinchedObject)
    {
        // if (currentGazed != null && currentGazed != gazedObject && currentGazed.IsHovered == true)
        // {
        //     currentGazed.IsHovered = false; // Ensure isHovered is updated
        //     currentGazed.EndHoverAnimation(); // Force unhighlight
        // }
        currentPinched = pinchedObject;
    }

    // Call this from your GazeInteractable objects when the gaze exits
    public void ClearPinchedObject(GazeInteractable pinchedObject)
    {
        if (currentPinched == pinchedObject)
        {
            pinchedObject = null;
        }
    }
}