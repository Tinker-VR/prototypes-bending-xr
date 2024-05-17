using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinchScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    
    private void Awake()
    {
        if(scrollRect == null)
        {
            scrollRect = GetComponent<ScrollRect>();
        }
    }

    void Update()
    {
        // Check if we should scroll based on the current gazed object and if a pinch gesture is ongoing
        if (InteractionManager.Instance.GetCurrentPinched() is PhotoTile && InteractionManager.Instance.IsIndexPinching)
        {
            float deltaY = InteractionManager.Instance.GetPinchMoveDelta().y;
            Scroll(deltaY);
        }
    }

    private void Scroll(float deltaY)
    {
        // Adjust the scroll position based on deltaY
        // You might need to map deltaY to the scrollRect's content position or use it to adjust verticalNormalizedPosition
        scrollRect.verticalNormalizedPosition -= deltaY * Time.deltaTime * 300f; // This is a simplified example; adjust as needed
    }
}
