using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    public float GrowRate;
    public float ScaleSize;
    public AudioClip ClickNoise;
    private Vector3 _baseScale;
    private bool _hovered;
    private float t;

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioSource.PlayClipAtPoint(ClickNoise, Vector3.zero);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovered = false;
    }

    void Start()
    {
        _baseScale = transform.localScale;
    }
    void Update()
    {
        if (!_hovered)
            t -= (t > 0) ? Time.deltaTime * GrowRate : 0;
        else
            t += (t < 1) ? Time.deltaTime * GrowRate : 0;

        // Basic tween

        var deltaSize = ScaleSize * Vector3.one - _baseScale;

        // cubic
        //this.gameObject.transform.localScale = _baseScale + t * t * t * deltaSize;

        //sinusoidal
        this.gameObject.transform.localScale = _baseScale + (0.5f * Mathf.Sin(Mathf.PI * (t - 0.5f)) + 0.5f) * deltaSize;
    }
}
