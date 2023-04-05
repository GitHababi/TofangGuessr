using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DifficultyButtonScript : MonoBehaviour, IPointerClickHandler
{
    public Text DifficultyText;
    public Button DifficultyButton;

    public static readonly string[] Difficulties =
    {
        "",
        "Easy",
        "Moderate",
        "Hard"
    };
    private int _difficulty;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            _difficulty = PlayerPrefs.GetInt("Difficulty");
        } 
        else
        {
            PlayerPrefs.SetInt("Difficulty", 1);
            _difficulty = 1;
        }
        DifficultyText.text = Difficulties[_difficulty];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _difficulty++;
        if(_difficulty > 3)
        {
            _difficulty = 1;
        }
        PlayerPrefs.SetInt("Difficulty", _difficulty);
        DifficultyText.text = Difficulties[_difficulty];
    }
}
