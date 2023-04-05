using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapDraggerScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public RawImage MapImage;
    public RawImage Pin;
    public RawImage GreenPin;
    public CameraDraggerScript CameraDragger;
    public float Sensitivity;
    public Text LocationString;
    public Vector2 GuessLocation { get; private set; }
    public Vector2 CorrectLocation { get; set; }
    public Vector3 OriginalPosition { get; private set; }
    public bool CorrectPinVisible;
    public bool DraggingEnabled;

    private bool _selected; // whether mouse is in frame
    private bool _dragging; // whether mouse is held down
    private bool _hasMoved; // whether at any point during dragging the mouse has moved.
    
    private Vector3 _dragStart;
    private Rect _initialPosition;
    private float MapSize => _rectTransform.sizeDelta.x * _rectTransform.localScale.x;

    // Start is called before the first frame update

    void Awake()
    {
        GuessLocation = new Vector2(0.5f, 0.5f);
        OriginalPosition = gameObject.transform.position;
        _rectTransform = MapImage.GetComponent<RectTransform>();
    }


    // Update is called once per frame
    void Update()
    {
        UpdatePins();
        if (!_selected || CameraDragger.Dragging || !DraggingEnabled)
            return;
        float scrolling = Input.GetAxis("Mouse ScrollWheel") / 2;
        var rect = MapImage.uvRect;
        if (Mathf.Approximately(rect.height, 0.05f) && scrolling > 0)
        {
            scrolling = 0;
        }
        rect.height = Mathf.Clamp(rect.height - scrolling / Sensitivity, 0.05f, 1f);
        rect.width = Mathf.Clamp(rect.width - scrolling / Sensitivity, 0.05f, 1f);
        rect.y += scrolling / (Sensitivity * 2);
        rect.x += scrolling / (Sensitivity * 2);
        if (_dragging)
        {
            if (Input.mousePosition != _dragStart)
                _hasMoved = true;
            var dragDelta = (_dragStart - Input.mousePosition) / MapSize * rect.height;
            rect.y = _initialPosition.y + dragDelta.y;
            rect.x = _initialPosition.x + dragDelta.x;
        }


        rect.y = Mathf.Clamp(rect.y, 0, 1 - rect.width);
        rect.x = Mathf.Clamp(rect.x, 0, 1 - rect.height);
        MapImage.uvRect = rect;

    }

    private void UpdatePins()
    {
        var pinScreenLocation = AbsoluteUVRectToScreenLocation(GuessLocation);
        // This bounds checking code is cheese, and yes i could make it with Mathf.abs but idc
        if (pinScreenLocation.x > gameObject.transform.position.x - MapSize / 2 &&
            pinScreenLocation.x < gameObject.transform.position.x + MapSize / 2)
            Pin.transform.position = new(pinScreenLocation.x, Pin.transform.position.y);
        if (pinScreenLocation.y > gameObject.transform.position.y - MapSize / 2 &&
            pinScreenLocation.y < gameObject.transform.position.y + MapSize / 2)
            Pin.transform.position = new(Pin.transform.position.x, pinScreenLocation.y);

        if (CorrectPinVisible)
        {
            var greenPinScreenLocation = AbsoluteUVRectToScreenLocation(CorrectLocation);
            if (greenPinScreenLocation.x > gameObject.transform.position.x - MapSize / 2 &&
            greenPinScreenLocation.x < gameObject.transform.position.x + MapSize / 2)
                GreenPin.transform.position = new(greenPinScreenLocation.x, GreenPin.transform.position.y);
            if (greenPinScreenLocation.y > gameObject.transform.position.y - MapSize / 2 &&
                greenPinScreenLocation.y < gameObject.transform.position.y + MapSize / 2)
                GreenPin.transform.position = new(GreenPin.transform.position.x, greenPinScreenLocation.y);
        }
        else
            GreenPin.transform.position = new(-1000, -1000);
    }

    private RectTransform _rectTransform;
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _selected = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _selected = false || _dragging;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!DraggingEnabled)
            return;

        _dragging = true;
        _dragStart = Input.mousePosition;
        _initialPosition = MapImage.uvRect;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!DraggingEnabled)
            return;

        if (_dragStart == Input.mousePosition && !_hasMoved)
        {
            GuessLocation = ScreenLocationToAbsoluteUVRect(Input.mousePosition);
            Debug.Log($"mouse click at {AbsoluteUVRectToInGameCoords(GuessLocation = ScreenLocationToAbsoluteUVRect(Input.mousePosition))}");
            LocationString.text = AbsoluteUVRectToInGameCoords(GuessLocation).ToString("F0");
        }
        _hasMoved = false;
        _dragging = false;
    }

    // this is the one that is stored
    // Transforms the position of the mouse on screen into a vector of where it is x,y wise on the map [0,1] for both. (0,0) is bottom left
    private Vector2 ScreenLocationToAbsoluteUVRect(Vector3 clickLocation)
    {
        var uvRectLocation = Vector3.one - (this.gameObject.transform.position - clickLocation + new Vector3(MapSize / 2, MapSize / 2, MapSize / 2)) / MapSize;
        return new Vector2(uvRectLocation.x * MapImage.uvRect.width + MapImage.uvRect.x, uvRectLocation.y * MapImage.uvRect.height + MapImage.uvRect.y);
    }

    // Invert Linear Transform of Above
    private Vector3 AbsoluteUVRectToScreenLocation(Vector2 absoluteUVRectPosition)
    {
        var someVar = new Vector3((absoluteUVRectPosition.x - MapImage.uvRect.x) / MapImage.uvRect.width, (absoluteUVRectPosition.y - MapImage.uvRect.y) / MapImage.uvRect.height, 0);
        return this.gameObject.transform.position + new Vector3(MapSize /2,MapSize / 2, MapSize) - ((Vector3.one - someVar) * MapSize);
    }
    // this is the one that is displayed
    public static Vector2 AbsoluteUVRectToInGameCoords(Vector2 absoluteUVRectPosition)
    {
        return new Vector2( 
            Mathf.RoundToInt((absoluteUVRectPosition.x) * 875f) - 437f, 
            437f - Mathf.RoundToInt(absoluteUVRectPosition.y * 875f));
    }
}
