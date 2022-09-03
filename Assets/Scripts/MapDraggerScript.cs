using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapDraggerScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public RawImage MapImage;
    public CameraDraggerScript CameraDragger;
    public float Sensitivity;


    private bool _selected;
    private bool _dragging;
    private Vector3 _dragStart;
    private Rect _initialPosition;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!_selected || CameraDragger.Dragging)
            return;
        float scrolling = Input.GetAxis("Mouse ScrollWheel") / 2;
        var rect = MapImage.uvRect;
        if (Mathf.Approximately(rect.height, 0.1f) && scrolling > 0)
        {
            scrolling = 0;
        }
        rect.height = Mathf.Clamp(rect.height - scrolling / Sensitivity, 0.1f, 1f);
        rect.width = Mathf.Clamp(rect.width - scrolling / Sensitivity, 0.1f, 1f);
        rect.y += scrolling / (Sensitivity * 2);
        rect.x += scrolling / (Sensitivity * 2);
        if (_dragging)
        {
            var dragDelta = (_dragStart - Input.mousePosition) / 300f * rect.height;
            rect.y = _initialPosition.y + dragDelta.y;
            rect.x = _initialPosition.x + dragDelta.x;
        }
        

        rect.y = Mathf.Clamp(rect.y ,0, 1 - rect.width);
        rect.x = Mathf.Clamp(rect.x, 0, 1 - rect.height);
        MapImage.uvRect = rect;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        _selected = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _selected = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _dragging = true;
        _dragStart = Input.mousePosition;
        _initialPosition = MapImage.uvRect;
        Debug.Log($"{MapImage.gameObject.transform.position} Mouse: {Input.mousePosition}");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _dragging = false;
    }
}
