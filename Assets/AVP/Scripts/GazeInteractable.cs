using UnityEngine;
using UnityEngine.EventSystems;

public class GazeInteractable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Keep track of whether the gaze is currently on this object
    private Collider collider;
    private static GazeInteractable currentGazed;
    public bool IsHovered = false;
    public bool IsPinching = false;

    public void Awake()
    {
        collider = GetComponent<Collider>();
        // if(collider)
        // {
        //     collider.enabled = false;
        // }
    }

    public virtual void ToggleInteractable(bool isInteractable)
    {
        collider.enabled = isInteractable;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsHovered)
        {
            IsHovered = true;
            StartHoverAnimation();
            
            if (InteractionManager.Instance != null)
            {
                InteractionManager.Instance.SetGazedObject(this);
            }
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (IsHovered)
        {
            if (InteractionManager.Instance != null)
            {
                InteractionManager.Instance.ClearGazedObject(this);
            }

            IsHovered = false;
            EndHoverAnimation();
        }
    }

    public virtual void StartHoverAnimation()
    {
    }

    public virtual void EndHoverAnimation()
    {
    }

    public virtual  void OnPinchPress()
    {
    }
    
    public virtual  void OnPinchDown()
    {
    }
    
    public virtual  void OnPinchRelease()
    {
    }
}
