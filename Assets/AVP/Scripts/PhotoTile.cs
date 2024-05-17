using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PhotoTile : GazeInteractable
{
    [SerializeField] private Image photoImage;
    [SerializeField] private Image highlightImage;

    [SerializeField] private Texture2D texture;
    
    private float highlightOpacity = 0.25f;
    private float fadeInDuration = 0.15f; // Duration for the fade animation
    private float fadeOutDuration = 0.05f; // Duration for the fade animation

    // private static PhotoTile currentHovered;

    void Start()
    {
        if (highlightImage != null)
        {
            Color initialColor = highlightImage.color;
            initialColor.a = 0f;
            highlightImage.color = initialColor;
        }
    }
    
    public void SetTexture(Texture2D newTexture)
    {
        Debug.Log("Inside Set Texture");
        if (newTexture != null && photoImage != null)
        {
            Debug.Log("Inside Set Texture Conditions Met");
            texture = newTexture;

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            photoImage.sprite = sprite;
            photoImage.preserveAspect = true;
            FitImageInTile(texture);
        }
    }
    
    private void FitImageInTile(Texture2D texture)
    {
        RectTransform rectTransform = photoImage.rectTransform;
        
        RectTransform tileRectTransform = photoImage.GetComponentInParent<Mask>().rectTransform;
        float tileSize = Mathf.Min(tileRectTransform.rect.width, tileRectTransform.rect.height);

        // Calculate the scaling factor needed to fit the image within the tile size
        float widthFactor = tileSize / texture.width;
        float heightFactor = tileSize / texture.height;

        // Determine whether the image is portrait or landscape and scale accordingly
        if (texture.height > texture.width) // Portrait
        {
            rectTransform.sizeDelta = new Vector2(tileSize, texture.height * widthFactor);
        }
        else // Landscape
        {
            rectTransform.sizeDelta = new Vector2(texture.width * heightFactor, tileSize);
        }
        
        // Set anchors to the middle to ensure the image is centered
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public override void StartHoverAnimation()
    {
        if (highlightImage != null)
        {
            LeanTween.cancel(highlightImage.gameObject);
            // Animate the highlightImage to become visible
            LeanTween.value(highlightImage.gameObject, updateHighlightOpacity, highlightImage.color.a, highlightOpacity, fadeInDuration);
        }
    }

    public override void EndHoverAnimation()
    {
        if (highlightImage != null)
        {
            LeanTween.cancel(highlightImage.gameObject);
            // Animate the highlightImage to become invisible again
            LeanTween.value(highlightImage.gameObject, updateHighlightOpacity, highlightImage.color.a, 0f, fadeOutDuration);
        }
    }

    private void updateHighlightOpacity(float opacity)
    {
        if (highlightImage != null)
        {
            Color color = highlightImage.color;
            color.a = opacity;
            highlightImage.color = color;
        }
    }

    public override void OnPinchPress()
    {
        Debug.Log("PhotoTile Pressed: " + gameObject.name);
        PhotosAppContent.Instance.OpenPhotoInViewer(texture); // Open the clicked photo in the viewer
    }
}
