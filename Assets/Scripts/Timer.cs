using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    [SerializeField] Color finishColor;

    private bool paused;
    private bool started;

    private float timer;

    IEnumerator timerRoutine;

    Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    void OnEnable()
    {
        EventManager.StartListening(Events.START_TIMER, OnStartTimer);
        EventManager.StartListening(Events.STOP_TIMER, OnStopTimer);
        EventManager.StartListening(Events.PAUSE, OnPause);
        EventManager.StartListening(Events.UNPAUSE, OnUnPause);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.START_TIMER, OnStartTimer);
        EventManager.StopListening(Events.STOP_TIMER, OnStopTimer);
        EventManager.StopListening(Events.PAUSE, OnPause);
        EventManager.StopListening(Events.UNPAUSE, OnUnPause);
    }

    void OnStartTimer() {
        started = true;
        timerRoutine = TimerLoop();
        StartCoroutine(timerRoutine);
    }

    void OnStopTimer() {
        started = false;
        text.color = finishColor;
    }

    void OnPause()
    {
        paused = true;
    }
    void OnUnPause()
    {
        paused = false;
    }


    IEnumerator TimerLoop()
    {
        while(started)
        {
            if(!paused) {
                timer += Time.deltaTime;
                int minutes = Mathf.FloorToInt(timer / 60F);
                int seconds = Mathf.FloorToInt(timer % 60F);
                int milliseconds = Mathf.FloorToInt((timer * 100F) % 100F);
                text.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + milliseconds.ToString("00");
            }
            yield return null;
        }
    }
}
