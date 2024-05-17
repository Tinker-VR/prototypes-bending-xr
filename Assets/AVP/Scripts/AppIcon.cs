using UnityEngine;
using UnityEngine.EventSystems;

public class AppIcon : GazeInteractable
{
    // Animation parameters
    public float scaleAmount = 1.05f;
    public float moveAmount = 1.5f;
    public float hoverInDuration = 0.15f;
    public float hoverOutDuration = 0.1f;
    public CanvasGroup titleCanvasGroup;

    // Title Opacity parameters
    public float normalOpacity = 0.6f;
    public float hoverOpacity = 1f;

    // References to child objects for bg, fg1, and fg2
    public GameObject icon;
    public Transform bg;
    public Transform fg1;
    public Transform fg2;

    // Original positions and sizes
    private Vector3 originalBgPosition;
    private Vector3 originalFg1Position;
    private Vector3 originalFg2Position;
    private Vector3 originalScale;

    [SerializeField] GameObject appObject;

    void Start()
    {
        // Record the original positions and scale
        originalBgPosition = bg.localPosition;
        originalFg1Position = fg1.localPosition;
        originalFg2Position = fg2.localPosition;
        originalScale = icon.transform.localScale;
        
        // Set initial title opacity
        if (titleCanvasGroup != null)
        {
            titleCanvasGroup.alpha = normalOpacity;
        }
    }

    void Update()
    {
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }

    public override void StartHoverAnimation()
    {
        LeanTween.cancel(icon);

        LeanTween.scale(icon, originalScale * scaleAmount, hoverInDuration).setEase(LeanTweenType.easeInOutQuad);

        LeanTween.moveLocalZ(bg.gameObject, originalBgPosition.z + moveAmount, hoverInDuration).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveLocalZ(fg1.gameObject, originalFg1Position.z - moveAmount/2, hoverInDuration).setEase(LeanTweenType.easeInOutQuad);

        LeanTween.moveLocalZ(fg2.gameObject, originalFg2Position.z - moveAmount, hoverInDuration).setEase(LeanTweenType.easeInOutQuad);
        
        if (titleCanvasGroup != null)
        {
            LeanTween.alphaCanvas(titleCanvasGroup, hoverOpacity, hoverInDuration).setEase(LeanTweenType.easeInOutQuad);
        }
    }

    public override void EndHoverAnimation()
    {
        LeanTween.cancel(icon);

        LeanTween.scale(icon, originalScale, hoverOutDuration).setEase(LeanTweenType.easeInOutQuad);

        LeanTween.moveLocalZ(bg.gameObject, originalBgPosition.z, hoverOutDuration).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveLocalZ(fg1.gameObject, originalFg1Position.z, hoverOutDuration).setEase(LeanTweenType.easeInOutQuad);
        
        LeanTween.moveLocalZ(fg2.gameObject, originalFg2Position.z, hoverOutDuration).setEase(LeanTweenType.easeInOutQuad);
        
        if (titleCanvasGroup != null)
        {
            LeanTween.alphaCanvas(titleCanvasGroup, normalOpacity, hoverOutDuration).setEase(LeanTweenType.easeInOutQuad);
        }
    }

    public override void OnPinchPress()
    {
        if(!AppMenu.Instance.IsVisible)
        {
            return;
        }

        LeanTween.cancel(icon);

        LeanTween.scale(icon, originalScale * 0.9f, hoverInDuration * 0.5f).setEase(LeanTweenType.easeInOutQuad);

        LeanTween.scale(icon, originalScale, hoverInDuration * 0.5f) 
            .setEase(LeanTweenType.easeOutElastic)
            .setDelay(hoverInDuration * 0.5f)
            .setOnComplete(() => 
            {
                AppMenu.Instance.HideMenu();

                if (appObject != null)
                {
                    AppMenu.Instance.OpenApp(appObject);
                }
            });
    }
}
