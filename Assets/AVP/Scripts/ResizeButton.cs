using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResizeButton : GazeInteractable
{
    [SerializeField] private Image resizeImage;
    private Vector3 originalScale;
    private float animationDuration = 0.15f;

    void Start()
    {
        if (resizeImage == null)
            resizeImage = GetComponent<Image>();

        SetImageAlpha(0f);

        originalScale = transform.localScale;
    }

    void Update()
    {
        if (IsPinching)
        {
            Vector3 pinchDelta = InteractionManager.Instance.GetPinchMoveDelta();
            
            ApplyResizeDelta(new Vector2(pinchDelta.x, pinchDelta.y));
        }
    }
    
    private void ApplyResizeDelta(Vector2 pinchDelta)
    {
        RectTransform viewerRect = PhotosAppContent.Instance.PhotoViewer.GetComponent<RectTransform>();
        RectTransform imageRect = PhotosAppContent.Instance.PhotoViewerImage.GetComponent<RectTransform>();
        // float sensitivityFactor = 100f;
        // pinchDeltaX = pinchDelta.x;
        float scalingFactor = 0f;
        
        if(pinchDelta.x > 0 && pinchDelta.y < 0)
        {
            scalingFactor = pinchDelta.magnitude * 1000f;
        }
        else if(pinchDelta.x < 0 && pinchDelta.y > 0)
        {
            scalingFactor = -pinchDelta.magnitude * 1000f;
        }

        float viewerAR = viewerRect.sizeDelta.x / viewerRect.sizeDelta.y;
        Vector2 newSizeViewer = viewerRect.sizeDelta + new Vector2(scalingFactor, scalingFactor / viewerAR);

        // float minWidth = 100f; // Define minimum width
        // float maxWidth = 1000f; // Define maximum width
        // newSize.x = Mathf.Clamp(newSize.x, minWidth, maxWidth);
        // newSize.y = newSize.x / originalAspectRatio;

        viewerRect.sizeDelta = newSizeViewer;
        imageRect.sizeDelta = newSizeViewer;
    }

    private void SetImageAlpha(float alpha)
    {
        if (resizeImage != null)
        {
            Color color = resizeImage.color;
            color.a = alpha;
            resizeImage.color = color;
        }
    }

    public override void OnPinchDown()
    {
        base.OnPinchPress(); // Call base method if it does something important
        // Set opacity to 1 and scale to 0.95x
        SetImageAlpha(1f);
        LeanTween.scale(gameObject, originalScale * 0.95f, animationDuration).setEase(LeanTweenType.easeInOutQuad);

        IsPinching = true;
    }

    public override void OnPinchRelease()
    {
        if(IsHovered)
        {
            SetImageAlpha(0.6f);
        }
        else
        {
            SetImageAlpha(0f);
        }

        LeanTween.scale(gameObject, originalScale, animationDuration).setEase(LeanTweenType.easeInOutQuad);
        
        IsPinching = false;
    }

    public override void StartHoverAnimation()
    {
        LeanTween.cancel(gameObject);
        
        base.StartHoverAnimation(); // Call base if needed
        SetImageAlpha(0.6f); // Set opacity to 0.6
    }

    public override void EndHoverAnimation()
    {
        if(IsPinching) return;

        LeanTween.cancel(gameObject);

        base.EndHoverAnimation(); // Call base if needed
        SetImageAlpha(0f); // Reset opacity to 0
    }
}
