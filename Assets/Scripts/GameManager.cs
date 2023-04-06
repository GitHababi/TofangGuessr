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

    public AudioClip PlingNoise;

    private (Vector2, Material)[] locations;

    void Start()
    {
        GuessButton.onClick.AddListener(MakeGuess);
        StartButton.onClick.AddListener(NextRoundButton);
        _roundNumber = 1;
        locations = RandomSampleWithoutReplacement(Constants.Locations, 5);
        StartRound();
    }
    void Update()
    {
        if (!ScoreboardPanel.activeSelf && (int)(_timeRemaining) == 0)
            MakeGuess();
        if (_timeRemaining == 0)
            return;
            _timeRemaining -= Time.deltaTime;
        TimerText.text = ((int)_timeRemaining) / 60 + ":" + ((((int)_timeRemaining % 60) < 10) ? "0" : "") + ((int)_timeRemaining) % 60;
        
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

        var guessCoords = MapDraggerScript.UVRectToCoords(draggerScript.GuessLocation);
        var correctCoords = MapDraggerScript.UVRectToCoords(draggerScript.CorrectLocation);
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

        mapDragger.CorrectLocation = locations[_roundNumber - 1].Item1;
        RenderSettings.skybox = locations[_roundNumber - 1 ].Item2;

        RoundNumber.text = _roundNumber + "/5";

        var draggerScript = MapImage.gameObject.GetComponent<MapDraggerScript>(); // Enable Dragging
        draggerScript.CorrectPinVisible = false;
        draggerScript.DraggingEnabled = true;

        var cameraDragger = Camera.gameObject.GetComponent<CameraDraggerScript>();
        cameraDragger.DraggingEnabled = true;


        AudioSource.PlayClipAtPoint(PlingNoise, Vector3.zero);
        _timeRemaining = 100f - 25f * Constants.Difficulty;
    }
    private int GetScore(float distance)
    {
        float maxDistance = 400 - 100 * Constants.Difficulty;
        if (distance > maxDistance)
            return 0;
        return Mathf.RoundToInt(50f * Mathf.Cos(Mathf.PI * distance / maxDistance) + 50f);
    }

    private static (Vector2, Material)[] RandomSampleWithoutReplacement((Vector2, Material)[] array, int sampleSize)
    {
        if (sampleSize >= array.Length)
        {
            return array; // return the original array if the sample size is equal to or larger than the length of the array
        }

        (Vector2, Material)[] result = new (Vector2, Material)[sampleSize];
        int currentIndex = 0;

        for (int i = 0; i < sampleSize; i++)
        {
            int remaining = array.Length - currentIndex;
            int selectedIndex = (int)(UnityEngine.Random.value * remaining); // select a random index from the remaining elements
            result[i] = array[currentIndex + selectedIndex]; // add the selected element to the result array
            array[currentIndex + selectedIndex] = array[currentIndex + remaining - 1]; // swap the selected element with the last remaining element
            currentIndex++; // move to the next element in the array
        }

        return result;
    }
}
