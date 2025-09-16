using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableWindow : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler
{
    [Header("Drag Settings")]
    public RectTransform dragHandle;  // Assign the header/grab point here

    private RectTransform windowRect;
    private Canvas canvas;
    private Vector2 offset;
    private Vector2 lastCanvasSize;

    private void Awake()
    {
        windowRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            Debug.LogError("DraggableWindow: No Canvas found in parent hierarchy.");
    }

    private void Start()
    {
        lastCanvasSize = (canvas.transform as RectTransform).rect.size;
        ClampToCanvas();
    }

    private void LateUpdate()
    {
        // Re-clamp if the canvas size changed
        Vector2 currentCanvasSize = (canvas.transform as RectTransform).rect.size;
        if (currentCanvasSize != lastCanvasSize)
        {
            lastCanvasSize = currentCanvasSize;
            ClampToCanvas();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        if (RectTransformUtility.RectangleContainsScreenPoint(dragHandle, eventData.position, cam))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                windowRect,
                eventData.position,
                cam,
                out offset
            );
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        if (!RectTransformUtility.RectangleContainsScreenPoint(dragHandle, eventData.position, cam))
        {
            eventData.pointerDrag = null; // Cancel drag if not clicking on handle
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (windowRect == null || canvas == null) return;

        RectTransform canvasRect = canvas.transform as RectTransform;
        var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                eventData.position,
                cam,
                out localPointerPosition))
        {
            Vector2 newPos = localPointerPosition - offset;
            windowRect.localPosition = newPos;

            ClampToCanvas();
        }
    }

    private void ClampToCanvas()
    {
        if (windowRect == null || canvas == null) return;

        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector2 pos = windowRect.localPosition;

        Vector2 minPosition = new Vector2(
            -canvasRect.rect.width / 2 + windowRect.rect.width / 2,
            -canvasRect.rect.height / 2 + windowRect.rect.height / 2
        );

        Vector2 maxPosition = new Vector2(
            canvasRect.rect.width / 2 - windowRect.rect.width / 2,
            canvasRect.rect.height / 2 - windowRect.rect.height / 2
        );

        pos.x = Mathf.Clamp(pos.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(pos.y, minPosition.y, maxPosition.y);

        windowRect.localPosition = pos;
    }
}
