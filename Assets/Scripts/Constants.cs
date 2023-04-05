using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static int Difficulty; // From 1 - 3

    public static readonly string[] Difficulties =
    {
        "",
        "Easy",
        "Moderate",
        "Hard"
    };

    public Material Center;
    public Material TMKTower;

    public static (Vector2,Material)[] Locations;
    private void Awake()
    {
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            Difficulty = PlayerPrefs.GetInt("Difficulty");
        }
        else
        {
            PlayerPrefs.SetInt("Difficulty", 1);
            Difficulty = 1;
        }

        Locations = new[]
        {
        (new Vector2(0.5f,0.5f), Center),
        (new Vector2(0.3691428f, 0.55085714f),TMKTower)
        };
    }

}
