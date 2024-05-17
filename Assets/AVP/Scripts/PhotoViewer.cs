using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PhotoViewer : GazeInteractable
{
    void Start()
    {
    }

    public override void OnPinchPress()
    {
        base.OnPinchPress(); // Call base method if it does something important

        PhotosAppContent.Instance.ClosePhotoViewer();
        PhotosAppContent.Instance.OpenPhotoGallery();
    }
}
