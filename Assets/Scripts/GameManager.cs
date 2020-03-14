using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static public GameManager instance;
    public GameSettingData gameData;

    public delegate void OnTouchEvent();
    public event OnTouchEvent OnTouch;


    //public Text scoreText;

    [HideInInspector]
    public bool isGameOver = false;

    [HideInInspector]
    public bool isGameStarted = false;

    private void Awake()
    {
        instance = this;
        
    }

    private void Start()
    {
        StartCoroutine(FSM());
    }

    IEnumerator FSM()
    {
        yield return OnWaitForStart();
        yield return OnUpdate();
        OnGameOver();
    }

    IEnumerator OnWaitForStart()
    {
        while (!isGameStarted) yield return null;

        yield return null;
    }


    IEnumerator OnUpdate()
    {
        while(!isGameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnTouch();
            }

            yield return null;
        }


    }

    void OnGameOver()
    {
        UIManager.instance.OnGameOver();
    }

    public void Replay()
    {
        SceneManager.LoadScene(0);
    }
}
