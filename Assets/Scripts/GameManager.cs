using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Camera Camera;
    public RawImage MapImage;

    public GameObject GameplayPanel;
    public GameObject ScoreboardPanel;
    
    public Button GuessButton;
    public Button StartButton;


    public TMP_Text TimerText;
    public TMP_Text RoundNumber;
    public TMP_Text StartButtonText;

    public Text ScoreText;

    private int _roundNumber;
    private float _timeRemaining;
    private int _totalScore;

    public GameObject Cover;
    public Text LoadingBar;

    void Start()
    {
        GuessButton.onClick.AddListener(MakeGuess);
        StartButton.onClick.AddListener(NextRoundButton);
        _roundNumber = 1;
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
        _roundNumber++;

        MapImage.transform.position = new(Screen.width * 0.5f, Screen.height * 0.5f);
        MapImage.transform.localScale = 1.6f * MapImage.transform.localScale;
        MapImage.uvRect = new Rect(0, 0, 1, 1);
        var draggerScript = MapImage.gameObject.GetComponent<MapDraggerScript>();
        draggerScript.CorrectPinVisible = true;
        draggerScript.DraggingEnabled = false;

        ScoreboardPanel.SetActive(true);
        GameplayPanel.SetActive(false);  

        var cameraDragger = Camera.gameObject.GetComponent<CameraDraggerScript>();
        cameraDragger.DraggingEnabled = false;

        var guessCoords = MapDraggerScript.AbsoluteUVRectToInGameCoords(draggerScript.GuessLocation);
        var correctCoords = MapDraggerScript.AbsoluteUVRectToInGameCoords(draggerScript.CorrectLocation);
        int score = GetScore(Vector3.Distance(guessCoords, correctCoords));
        _totalScore += score;

        ScoreText.text = $"You were {(int)Vector3.Distance(guessCoords, correctCoords)} blocks away!\nScore +{score}/100";
    }

    public void NextRoundButton()
    {
        if (_roundNumber > 6)
            StartCoroutine(LoadMenuAsync());
        if (_roundNumber > 5)
        {
            ScoreText.text = $"You got {_totalScore} out of 500\non {Constants.Difficulties[Constants.Difficulty]} difficulty!";
            StartButtonText.text = "Main Menu";
            _roundNumber++;
        }   
        else
            StartRound();
    }

    public IEnumerator LoadMenuAsync()
    {
        var operation = SceneManager.LoadSceneAsync("Menu");
        Cover.SetActive(true);

        while (!operation.isDone)
        {
            LoadingBar.text = (int)(operation.progress * 100) + "%";

            yield return null;
        }
    }

    public void StartRound()
    {
        Camera.transform.localEulerAngles = 360f * UnityEngine.Random.value * Vector3.up;

        var mapDragger = MapImage.gameObject.GetComponent<MapDraggerScript>(); // Return Map to Corner
        MapImage.transform.position = mapDragger.OriginalPosition;
        MapImage.transform.localScale = Vector3.one;
        MapImage.uvRect = new Rect(0, 0, 1, 1);

        ScoreboardPanel.SetActive(false);
        GameplayPanel.SetActive(true);

        int randomLocation = (int)(UnityEngine.Random.value * Constants.Locations.Length); // Randomize Location
        RenderSettings.skybox = Constants.Locations[randomLocation].Item2;
        mapDragger.CorrectLocation = Constants.Locations[randomLocation].Item1;


        RoundNumber.text = _roundNumber + "/5";

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
