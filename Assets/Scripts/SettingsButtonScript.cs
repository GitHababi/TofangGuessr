using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsButtonScript : MonoBehaviour, IPointerClickHandler
{
    public GameObject SettingsMenu;
    public GameObject MainMenu;

    public void OnPointerClick(PointerEventData eventData)
    {
        SettingsMenu.SetActive(!SettingsMenu.activeSelf);
        MainMenu.SetActive(!MainMenu.activeSelf);
    }
}
