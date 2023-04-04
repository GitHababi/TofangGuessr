using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Camera Camera;
    public RawImage MapImage;
    
    public Button GuessButton;
    public Button StartButton;

    public Text GuessText;

    public Image TimerBackground;
    public TMP_Text TimerText;

    public Image ScoreBackground;
    public Text ScoreText;
    private float _timeRemaining;
    void Start()
    {
        GuessButton.onClick.AddListener(MakeGuess);
        StartButton.onClick.AddListener(NextRoundButton);
        
        StartRound();
    }
    void Update()
    {
        if (_timeRemaining == 0)
            return;

            _timeRemaining -= Time.deltaTime;
            TimerText.text = ((int)_timeRemaining)/60 + ":" + ((int)_timeRemaining) % 60;
        
    }

    void MakeGuess()
    {
        _timeRemaining = 0;
        MapImage.transform.position = new(Screen.width * 0.5f, Screen.height * 0.5f);
        MapImage.transform.localScale = 1.6f * MapImage.transform.localScale;
        MapImage.uvRect = new Rect(0, 0, 1, 1);
        var draggerScript = MapImage.gameObject.GetComponent<MapDraggerScript>();
        draggerScript.CorrectPinVisible = true;
        draggerScript.DraggingEnabled = false;

        var cameraDragger = Camera.gameObject.GetComponent<CameraDraggerScript>();
        cameraDragger.DraggingEnabled = false;

        var guessCoords = MapDraggerScript.AbsoluteUVRectToInGameCoords(draggerScript.GuessLocation);
        var correctCoords = MapDraggerScript.AbsoluteUVRectToInGameCoords(draggerScript.CorrectLocation);
        int score = GetScore(Vector3.Distance(guessCoords, correctCoords));

        GuessButton.gameObject.SetActive(false);
        GuessText.gameObject.SetActive(false);
        TimerBackground.gameObject.SetActive(false);
        StartButton.gameObject.SetActive(true);
        ScoreBackground.gameObject.SetActive(true);

        ScoreText.text = $"You were {(int)Vector3.Distance(guessCoords, correctCoords)} blocks away!\nScore +{score}/100";
    }

    public void NextRoundButton()
    {
        StartRound();
    }
    public void StartRound()
    {
        Camera.transform.localEulerAngles = 360f * UnityEngine.Random.value * Vector3.up;

        var mapDragger = MapImage.gameObject.GetComponent<MapDraggerScript>(); // Return Map to Corner
        MapImage.transform.position = mapDragger.OriginalPosition;
        MapImage.transform.localScale = Vector3.one;
        MapImage.uvRect = new Rect(0, 0, 1, 1);


        int randomLocation = (int)(UnityEngine.Random.value * Constants.Locations.Length); // Randomize Location
        RenderSettings.skybox = Constants.Locations[randomLocation].Item2;
        mapDragger.CorrectLocation = Constants.Locations[randomLocation].Item1;

        GuessButton.gameObject.SetActive(true); // Enable UI
        GuessText.gameObject.SetActive(true);
        TimerBackground.gameObject.SetActive(true);
        StartButton.gameObject.SetActive(false);
        ScoreBackground.gameObject.SetActive(false);

        var draggerScript = MapImage.gameObject.GetComponent<MapDraggerScript>(); // Enable Dragging
        draggerScript.CorrectPinVisible = false;
        draggerScript.DraggingEnabled = true;

        var cameraDragger = Camera.gameObject.GetComponent<CameraDraggerScript>();
        cameraDragger.DraggingEnabled = true;


        _timeRemaining = 150f - 30f * Constants.Difficulty;
    }
    private int GetScore(float distance)
    {
        float maxDistance = 400 - 100 * Constants.Difficulty;
        if (distance > maxDistance)
            return 0;
        return Mathf.RoundToInt(50f * Mathf.Cos(Mathf.PI * distance / maxDistance) + 50f);
    }
}
