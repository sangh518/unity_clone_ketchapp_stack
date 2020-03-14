using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    static public UIManager instance;

    public Text scoreText;
    public GameObject Title;
    public Button playButton;

    private void Awake()
    {
        instance = this;
        playButton.onClick.AddListener(OnStartPlay);
    }

    public void OnStartPlay()
    {
        Title.SetActive(false);
        playButton.onClick.RemoveAllListeners();
        playButton.gameObject.SetActive(false);
        SetScoreText("0");
        GameManager.instance.isGameStarted = true;
    }


    public void SetScoreText(string score)
    {
        scoreText.text = score;
    }

    public void OnGameOver()
    {
        playButton.gameObject.SetActive(true);
        playButton.onClick.AddListener(GameManager.instance.Replay);
    }
}
