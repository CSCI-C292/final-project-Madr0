using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] RuntimeData _runtimeData;
    [SerializeField] TextMeshProUGUI _timeDisplay;
    [SerializeField] TextMeshProUGUI _scoreDisplay;

    private bool timerRunning;
    private bool timerStarted;
    private float seconds;
    private int minutes;

    void Awake()
    {
        GameEvents.levelStart += startTimer;
        GameEvents.levelFinish += stopTimer;
        GameEvents.gameOver += restartLevel;
    }

    void Update()
    {
        if(timerRunning) {
            seconds += Time.deltaTime;
            if(seconds >= 60) {
                seconds -= 60;
                minutes += 1;
            }
        }
        if(timerStarted) {
            string minutesString = "";
            string secondsString = "";
            if(seconds < 10) {
                secondsString = "0";
            }
            secondsString += Mathf.Round(seconds*1000)/1000;
            if(minutes < 10) {
                minutesString = "0";
            }
            minutesString += minutes;
            _timeDisplay.text = ""+minutesString+":"+secondsString;
        }
    }

    void startTimer(object sender, EventArgs args) {
        timerRunning = true;
        timerStarted = true;
    }

    void stopTimer(object sender, EventArgs args) {
        timerRunning = false;
        _scoreDisplay.text = "Score: "+Mathf.Round(_runtimeData.currentScore/((seconds+60*minutes)/60));
    }

    void restartLevel(object sender, EventArgs args) {
        SceneManager.LoadScene("Level1");
    }
}
