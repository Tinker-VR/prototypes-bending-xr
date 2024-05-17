using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotosAppContent : MonoBehaviour
{
    public static PhotosAppContent Instance { get; private set; }

    public GameObject rowPrefab;
    public GameObject tilePrefab;
    public Transform contentPanel; 

    private const int ImagesPerRow = 5;

    [SerializeField] private GameObject photoGallery; 
    [SerializeField] private GameObject galleryBar; 

    [SerializeField] private GameObject photoViewer; 
    public GameObject PhotoViewer
    {
        get
        {
            return photoViewer;
        }
    }

    [SerializeField] private Image photoViewerImage; 
    public Image PhotoViewerImage
    {
        get
        {
            return photoViewerImage;
        }
    }
    
    public float fadeInDuration = 0.5f;
    public float fadeInDelayIncrement = 0.4f; // Delay increment for each "layer" of icons

    public float fadeOutDuration = 0.4f;
    public float fadeOutDelay = 0.2f; // Delay increment for each "layer" of icons

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

    void Start()
    {
        PopulateList();
        ClosePhotoGallery();
        ClosePhotoViewer();
    }

    void PopulateList()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Images");
        GameObject currentRow = null;

        for (int i = 0; i < textures.Length; i++)
        {
            if (i % ImagesPerRow == 0 || currentRow == null)
            {
                currentRow = Instantiate(rowPrefab, contentPanel);
            }

            GameObject tileGO = Instantiate(tilePrefab, currentRow.transform);
            PhotoTile photoTile = tileGO.GetComponent<PhotoTile>();
            if (photoTile != null)
            {
                photoTile.SetTexture(textures[i]);
            }
        }
    }

    public void OpenPhotoInViewer(Texture2D texture)
    {
        if (photoViewer != null && photoViewerImage != null)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            photoViewerImage.sprite = sprite;
            
            AdjustViewerSizeForImage(texture);
            ClosePhotoGallery();
            OpenPhotoViewer();
        }
    }

    public void OpenPhotoViewer()
    {
        CanvasGroup viewerCG = photoViewer.GetComponent<CanvasGroup>();
        if (viewerCG != null)
        {
            viewerCG.alpha = 0f;
            viewerCG.interactable = true;
            viewerCG.blocksRaycasts = true;

            if(photoViewer.GetComponentInChildren<GazeInteractable>())
            {
                photoViewer.GetComponentInChildren<GazeInteractable>().ToggleInteractable(true);
            }

            foreach(GazeInteractable interactable in photoViewer.GetComponentsInChildren<GazeInteractable>())
            {
                interactable.ToggleInteractable(true);
            }

            LeanTween.alphaCanvas(viewerCG, 1, fadeInDuration).setFrom(0f).setDelay(5 * fadeOutDelay);
        }
    }

    public void OpenPhotoGallery()
    {
        CanvasGroup galleryCG = photoGallery.GetComponent<CanvasGroup>();
        CanvasGroup barCG = galleryBar.GetComponent<CanvasGroup>();

        if (galleryCG != null)
        {
            galleryCG.alpha = 0f;
            galleryCG.interactable = true;
            galleryCG.blocksRaycasts = true;

            foreach(GazeInteractable interactable in photoGallery.GetComponentsInChildren<GazeInteractable>())
            {
                interactable.ToggleInteractable(true);
            }

            LeanTween.alphaCanvas(galleryCG, 1, fadeInDuration).setFrom(0f).setDelay(5 * fadeOutDelay);
            LeanTween.alphaCanvas(barCG, 1, fadeInDuration).setFrom(0f).setDelay(5 * fadeOutDelay);
        }
    }
    
    public void ClosePhotoViewer()
    {
        CanvasGroup viewerCG = photoViewer.GetComponent<CanvasGroup>();
        if (viewerCG != null)
        {
            viewerCG.interactable = false;
            viewerCG.blocksRaycasts = false;

            if(photoViewer.GetComponentInChildren<GazeInteractable>())
            {
                photoViewer.GetComponentInChildren<GazeInteractable>().ToggleInteractable(false);
            }

            foreach(GazeInteractable interactable in photoViewer.GetComponentsInChildren<GazeInteractable>())
            {
                interactable.ToggleInteractable(false);
            }

            LeanTween.alphaCanvas(viewerCG, 0f, fadeOutDuration);
        }
    }

    public void ClosePhotoGallery()
    {
        CanvasGroup galleryCG = photoGallery.GetComponent<CanvasGroup>();
        CanvasGroup barCG = galleryBar.GetComponent<CanvasGroup>();

        if (galleryCG != null)
        {
            galleryCG.interactable = false;
            galleryCG.blocksRaycasts = false;

            foreach(GazeInteractable interactable in photoGallery.GetComponentsInChildren<GazeInteractable>())
            {
                interactable.ToggleInteractable(false);
            }

            LeanTween.alphaCanvas(galleryCG, 0f, fadeInDuration);
            LeanTween.alphaCanvas(barCG, 0f, fadeInDuration);
        }
    }
    
    public void AdjustViewerSizeForImage(Texture2D image)
    {
        // Define the max size for the viewer
        float maxWidth = 580f; // Max width for the viewer
        float maxHeight = 435f; // Max height for the viewer
        
        RectTransform viewerRectTransform = photoViewer.GetComponent<RectTransform>();
        RectTransform imageRectTransform = photoViewerImage.GetComponent<RectTransform>();

        viewerRectTransform.sizeDelta = new Vector2( maxWidth, maxHeight);
        imageRectTransform.sizeDelta = new Vector2( maxWidth, maxHeight);

        // Calculate the aspect ratio of the image
        float imageAspectRatio = (float)image.width / (float)image.height;

        // Determine the target size for the viewer
        float targetWidth, targetHeight;
        if (imageAspectRatio > 1) // Landscape or square
        {
            targetWidth = Mathf.Min(image.width, maxWidth);
            targetHeight = targetWidth / imageAspectRatio;
        }
        else // Portrait
        {
            targetHeight = Mathf.Min(image.height, maxHeight);
            targetWidth = targetHeight * imageAspectRatio;
        }

        // Ensure the viewer does not exceed the max dimensions
        targetWidth = Mathf.Min(targetWidth, maxWidth);
        targetHeight = Mathf.Min(targetHeight, maxHeight);

        // Apply the new size to the photo viewer's RectTransform
        if (viewerRectTransform != null)
        {
            viewerRectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
        }
    }
}
