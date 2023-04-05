using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StartButtonScript : MonoBehaviour, IPointerClickHandler
{
    public GameObject Cover;
    public Text LoadingBar;

    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(LoadGameAsync());
    }

    public IEnumerator LoadGameAsync()
    {
        var operation = SceneManager.LoadSceneAsync("Game");
        Cover.SetActive(true);
       
        while (!operation.isDone)
        {
            LoadingBar.text = (int)(operation.progress * 100) + "%";

            yield return null;
        }
    }
}
