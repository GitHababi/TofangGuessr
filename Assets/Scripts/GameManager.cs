using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button GuessButton;
    public Button StartButton;
    public RawImage MapImage;
    public Camera Camera;

    void Start()
    {
        GuessButton.onClick.AddListener(MakeGuess);
        
        NextRound();
    }
    void Update()
    {

    }

    void MakeGuess()
    {
        MapImage.transform.position = new(Screen.width * 0.5f, Screen.height * 0.5f);
        MapImage.transform.localScale = 2f * MapImage.transform.localScale;
        MapImage.uvRect = new Rect(0, 0, 1, 1);
        var draggerScript = MapImage.gameObject.GetComponent<MapDraggerScript>();
        draggerScript.CorrectPinVisible = true;
        draggerScript.DraggingEnabled = false;

        var cameraDragger = Camera.gameObject.GetComponent<CameraDraggerScript>();
        cameraDragger.DraggingEnabled = false;

        var guessCoords = MapDraggerScript.AbsoluteUVRectToInGameCoords(draggerScript.GuessLocation);
        var correctCoords = MapDraggerScript.AbsoluteUVRectToInGameCoords(draggerScript.CorrectLocation);
        int score = GetScore(Vector3.Distance(guessCoords, correctCoords));

        GuessButton.enabled = false;

        Debug.Log($"Distance is: {Vector3.Distance(guessCoords, correctCoords)}");
        Debug.Log($"Score is: {score}");
    }

    public void NextRound()
    {
        var mapDragger = MapImage.gameObject.GetComponent<MapDraggerScript>();
        MapImage.transform.position = mapDragger.OriginalPosition;
        MapImage.transform.localScale = Vector3.one;
        MapImage.uvRect = new Rect(0, 0, 1, 1);

        int randomLocation = (int)(UnityEngine.Random.value * Constants.Locations.Length);
        RenderSettings.skybox = Constants.Locations[randomLocation].Item2;
        mapDragger.CorrectLocation = Constants.Locations[randomLocation].Item1;
    }
    private int GetScore(float distance)
    {
        float maxDistance = 400 - 100 * Constants.Difficulty;
        if (distance > maxDistance)
            return 0;
        return Mathf.RoundToInt(50f * Mathf.Cos(Mathf.PI * distance / maxDistance) + 50f);
    }
}
