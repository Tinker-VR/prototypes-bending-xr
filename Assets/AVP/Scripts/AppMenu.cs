using UnityEngine;

public class AppMenu : MonoBehaviour
{
    public static AppMenu Instance { get; private set; }

    public Transform centerIcon; // Assign the center icon transform
    public float fadeInDuration = 0.5f;
    public float fadeInDelayIncrement = 0.4f; // Delay increment for each "layer" of icons

    public float fadeOutDuration = 0.4f;
    public float fadeOutDelay = 0.2f; // Delay increment for each "layer" of icons

    public bool IsVisible { get; private set; }
    
    public GameObject CurrentApp;

    [SerializeField] private CanvasGroup sideMenu;

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

    private void Start()
    {
        ShowMenu();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FadeInMenu();
        }
        if(Input.GetKeyDown(KeyCode.K))
        {
            FadeOutMenu();
        }
        // Here you would use your eye-tracking data to determine if the user is looking at this object.
        // For now, we'll use OnPointerEnter and OnPointerExit for mouse-based testing.
    }
    
    public void ShowMenu()
    {
        FadeInMenu();
        
        foreach(GazeInteractable interactable in GetComponentsInChildren<GazeInteractable>())
        {
            interactable.ToggleInteractable(true);
        }

        IsVisible = true;
    }

    // Call this method to fade in the menu
    public void FadeInMenu()
    {
        foreach(Transform icon in transform)
        {
            CanvasGroup canvasGroup = icon.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // Reset the scale to original before fading in
                LeanTween.scale(icon.gameObject, Vector3.one, fadeInDuration).setEase(LeanTweenType.easeOutQuad);
            
                float delay = Vector3.Distance(icon.position, centerIcon.position) * fadeInDelayIncrement;
                canvasGroup.alpha = 0f;
                LeanTween.alphaCanvas(canvasGroup, 1, fadeInDuration).setDelay(delay);
            }
        }
        
        LeanTween.alphaCanvas(sideMenu, 1, fadeInDuration).setDelay(3*fadeInDelayIncrement);
    }
    
    public void HideMenu()
    {
        FadeOutMenu();
        
        foreach(GazeInteractable interactable in GetComponentsInChildren<GazeInteractable>())
        {
            interactable.ToggleInteractable(false);
        }

        IsVisible = false;
    }

    // Call this method to fade out the menu
    public void FadeOutMenu()
    {
        foreach(Transform icon in transform)
        {
            LeanTween.cancel(icon.gameObject);
            
            CanvasGroup canvasGroup = icon.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                // Start scaling up the icon immediately
                LeanTween.scale(icon.gameObject, Vector3.one * 1.5f, fadeOutDuration).setEase(LeanTweenType.easeInBack);

                // Start fading out the icon with a slight delay
                LeanTween.alphaCanvas(canvasGroup, 0, fadeOutDuration * 0.75f).setDelay(fadeOutDelay);
            }
        }

        LeanTween.alphaCanvas(sideMenu, 0, fadeOutDuration * 0.75f);
    }

    public void OpenApp(GameObject appObject)
    {
        // If there's already an open app, close it first
        if (CurrentApp != null)
        {
            CurrentApp.SetActive(false);
        }
        
        // Open the new app
        CurrentApp = appObject;
        // appObject.SetActive(true);
        CanvasGroup appCG = appObject.GetComponent<CanvasGroup>();
        if (appCG != null)
        {
            appCG.alpha = 0f;
            appCG.interactable = true;

            // foreach(GazeInteractable interactable in appObject.GetComponentsInChildren<GazeInteractable>())
            // {
            //     interactable.ToggleInteractable(true);
            // }
            PhotosAppContent.Instance.ClosePhotoViewer();
            PhotosAppContent.Instance.OpenPhotoGallery();

            LeanTween.alphaCanvas(appCG, 1, fadeInDuration).setFrom(0f).setDelay(5 * fadeOutDelay);
        }
    }

    public void CloseCurrentApp()
    {
        if (CurrentApp != null)
        {
            CanvasGroup appCG = CurrentApp.GetComponent<CanvasGroup>();
            if (appCG != null)
            {
                PhotosAppContent.Instance.ClosePhotoViewer();
                PhotosAppContent.Instance.ClosePhotoGallery();

                LeanTween.alphaCanvas(appCG, 0, 0.5f).setOnComplete(() =>
                {
                    CurrentApp.SetActive(false);
                });
            }
            else
            {
                CurrentApp.SetActive(false);
            }
            CurrentApp = null;
        }

        ShowMenu(); // Optionally, show the main menu again
    }
}
