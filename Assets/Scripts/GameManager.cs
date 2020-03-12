using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static public GameManager instance;
    public GameSettingData gameData;
    public delegate void OnTouchEvent();
    public event OnTouchEvent OnTouch;

    public Text scoreText;

    public bool isGameOver = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        
        
    }

    private void Update()
    {
        if(!isGameOver && Input.GetMouseButtonDown(0))
        {
            OnTouch();
        }
    }


}
