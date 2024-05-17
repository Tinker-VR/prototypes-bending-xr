using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CloseButton : GazeInteractable
{
    [SerializeField] private Image buttonImage; // Assign in the editor
    [SerializeField] private Sprite defaultSprite; // Assign in the editor
    [SerializeField] private Sprite hoverSprite; // Assign in the editor
    private Vector3 originalScale;
    private float animationDuration = 0.15f; // Duration of the scale animation

    void Start()
    {
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        // Set the initial sprite
        if (buttonImage != null && defaultSprite != null)
            buttonImage.sprite = defaultSprite;

        originalScale = transform.localScale;
    }

    public override void OnPinchPress()
    {
        base.OnPinchPress(); // Call base method if it does something important
        CloseCurrentApp();
    }

    private void CloseCurrentApp()
    {
        if(AppMenu.Instance != null)
        {
            AppMenu.Instance.CloseCurrentApp();
        }
    }

    public override void StartHoverAnimation()
    {
        LeanTween.cancel(gameObject);
        
        base.StartHoverAnimation(); // Call base if needed
        if (buttonImage != null && hoverSprite != null)
        {
            buttonImage.sprite = hoverSprite;
            // Smoothly scale the button to be twice its size using LeanTween
            LeanTween.scale(gameObject, originalScale * 2f, animationDuration).setEase(LeanTweenType.easeInOutQuad);
        }
    }

    public override void EndHoverAnimation()
    {
        LeanTween.cancel(gameObject);

        base.EndHoverAnimation(); // Call base if needed
        if (buttonImage != null && defaultSprite != null)
        {
            buttonImage.sprite = defaultSprite;
            // Smoothly return the button to its original scale using LeanTween
            LeanTween.scale(gameObject, originalScale, animationDuration).setEase(LeanTweenType.easeInOutQuad);
        }
    }
}
