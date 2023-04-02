using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour
{
    private Button _button;
    private bool _hovered;
    private float t;
    void Start()
    {
        _button = this.gameObject.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
