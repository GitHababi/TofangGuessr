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
    public Material HIOfficeButura;
    public Material JungleHouse;
    public Material StrangeStructure;
    public Material TofangStadium;
    public Material IONSpaceLabs;
    public Material PresidentialManor;
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
        (new Vector2(0.3691428f, 0.55085714f),TMKTower),
        (MapDraggerScript.CoordsToUVRect(-259.50f, -190.5f),HIOfficeButura),
        (MapDraggerScript.CoordsToUVRect(-62.5f, -376.5f),JungleHouse),
        (MapDraggerScript.CoordsToUVRect(224.5f, -115.5f),StrangeStructure),
        (MapDraggerScript.CoordsToUVRect(227.5f, 75.5f),TofangStadium),
        (MapDraggerScript.CoordsToUVRect(44.5f, 238.5f),IONSpaceLabs),
        (MapDraggerScript.CoordsToUVRect(-211.5f, 361.5f),PresidentialManor)
        };
    }

}
