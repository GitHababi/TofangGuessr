using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CameraDraggerScript : MonoBehaviour
{
    public bool Dragging { get; private set; }
    public bool DraggingEnabled;
    public Camera Camera;
    
    private float _speed;
    private Vector3 _dragStart;
    private Vector3 _initialEulers;
    private Transform _cameraTransform;
    void Start()
    {
        _speed = Camera.fieldOfView / 360f;
        _cameraTransform = this.GetComponent<Transform>();
    }

    void Update()
    {
        if (!DraggingEnabled)
        {
            Dragging = false;
            return;
        }

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Camera.fieldOfView -= scroll * 15;
            _speed = Camera.fieldOfView / 360f;
            Camera.fieldOfView = Mathf.Clamp(Camera.fieldOfView, 15, 120);
        }

        //start mouse drag
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) // ensure mouse is in range for start
            {
                Dragging = false;
            }
            else
            {
                Dragging = true;
                _dragStart = Input.mousePosition;
                _initialEulers = new Vector3(_cameraTransform.localEulerAngles.x, _cameraTransform.localEulerAngles.y, 0f);
            }
        }
        //drag process
        else if (Input.GetMouseButton(0) && Dragging)
        {
            Vector3 _dragDelta = Input.mousePosition - _dragStart;

            Vector3 eulers = _cameraTransform.localEulerAngles;
            eulers.y = _initialEulers.y - _dragDelta.x * _speed;
            eulers.x = _initialEulers.x + _dragDelta.y * _speed;

            // bring rotation range to [-180, 180] interval, instead of default [0, 360]
            if (eulers.x > 180f)
                eulers.x -= 360f;

            eulers.x = Mathf.Clamp(eulers.x, -89.9f, 89.9f);    //so that rotation does not go on vertical flip
            _cameraTransform.localEulerAngles = eulers;
        } else if (Input.GetMouseButtonUp(0))
        {
            Dragging = false;
        }
    }
}
