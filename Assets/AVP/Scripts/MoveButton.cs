using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveButton : GazeInteractable
{
    [SerializeField] private Image buttonImage;
    private float animationDuration = 0.15f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private float moveSensitivity = 10f;

    void Start()
    {
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        
        SetImageAlpha(0.6f);

        originalScale = transform.localScale;
        originalPosition = PhotosAppContent.Instance.transform.position;
    }

    void Update()
    {
        if (IsPinching)
        {
            Vector3 pinchDelta = InteractionManager.Instance.GetPinchMoveDelta();
            
            ApplyMoveDelta(pinchDelta);
        }
    }
    
    private void ApplyMoveDelta(Vector3 pinchDelta)
    {
        Vector3 localDelta = Camera.main.transform.InverseTransformDirection(pinchDelta);

        float rotateSensitivity = 150f;

        float rotateAngleY = localDelta.x * rotateSensitivity; // Left/right movement affects rotation around Y
        float rotateAngleX = -localDelta.y * rotateSensitivity; // Up/down movement affects rotation around X

        float moveDeltaZ = localDelta.z * moveSensitivity;

        Vector3 currentPositionRelativeToPlayer = PhotosAppContent.Instance.transform.position - Camera.main.transform.position;

        Quaternion rotationY = Quaternion.AngleAxis(rotateAngleY, Vector3.up);
        Quaternion rotationX = Quaternion.AngleAxis(rotateAngleX, Camera.main.transform.right);
        Vector3 newPositionRelativeToPlayer = rotationY * rotationX * currentPositionRelativeToPlayer + moveDeltaZ * Camera.main.transform.forward;

        PhotosAppContent.Instance.transform.position = Camera.main.transform.position + newPositionRelativeToPlayer;

        PhotosAppContent.Instance.transform.LookAt(Camera.main.transform);
        PhotosAppContent.Instance.transform.rotation = Quaternion.Euler(PhotosAppContent.Instance.transform.eulerAngles.x, PhotosAppContent.Instance.transform.eulerAngles.y, 0);
        PhotosAppContent.Instance.transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    private void SetImageAlpha(float alpha)
    {
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = alpha;
            buttonImage.color = color;
        }
    }

    public override void OnPinchDown()
    {
        base.OnPinchDown();

        SetImageAlpha(1f);
        LeanTween.scale(gameObject, originalScale * 0.95f, animationDuration).setEase(LeanTweenType.easeInOutQuad);

        IsPinching = true;
    }

    public override void OnPinchRelease()
    {        
        if(IsHovered)
        {
            SetImageAlpha(1f);
        }
        else
        {
            SetImageAlpha(0.6f);
        }

        LeanTween.scale(gameObject, originalScale, animationDuration).setEase(LeanTweenType.easeInOutQuad);
        
        IsPinching = false;
        
        originalPosition = PhotosAppContent.Instance.transform.position;
    }

    public override void StartHoverAnimation()
    {
        LeanTween.cancel(gameObject);
        
        base.StartHoverAnimation();
        SetImageAlpha(1f);
    }

    public override void EndHoverAnimation()
    {
        if(IsPinching) return;

        LeanTween.cancel(gameObject);

        base.EndHoverAnimation();
        SetImageAlpha(0.6f);
    }
}
