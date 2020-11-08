using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] int targetFrameRate = 60;
    [SerializeField] bool endless;
    [SerializeField] float colorChangeFrequency = 1;
    [SerializeField] bool colorChangeTriggeredByUser = false;

    public bool EndlessMode
    {
        get
        {
            return endless;
        }
        private set { }
    }

    IEnumerator interval;

    public Color currentBackgroundColor;
    public Color currentContrastColor;

    private AudioSource audioSource;

    public bool Paused { get; private set; }
    public bool Finished { get; private set; }

    private static GameManager gameManager;
    public static GameManager instance
    {
        get
        {
            if (!gameManager)
            {
                gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (!gameManager)
                {
                    Debug.LogError("There needs to be one active GameManager script on a GameObject in your scene.");
                }
                else
                {
                    gameManager.Init();
                }
            }

            return gameManager;
        }
    }

    Dictionary<string, UnityAction> actions;

    void Init()
    {
        GenerateNewColors();
        audioSource = GetComponent<AudioSource>();
        if (!colorChangeTriggeredByUser)
        {
            interval = ChangeColor();
            StartCoroutine(interval);
        }
    }

    void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }
    void OnEnable()
    {
        actions = new Dictionary<string, UnityAction>() {
            { Events.PRESS, Press},
            { Events.PAUSE, OnPause},
            { Events.UNPAUSE, OnUnPause},
            { Events.GOAL, OnGoal},
            { Events.PLAY_GOAL_SOUND, OnGoalSound },
            { Events.MUTE, OnMute},
            { Events.UNMUTE, OnUnMute},
        };

        foreach (KeyValuePair<string, UnityAction> act in actions)
        {
            EventManager.StartListening(act.Key, act.Value);
        }
    }

    void OnDisable()
    {
        foreach (KeyValuePair<string, UnityAction> act in actions)
        {
            EventManager.StopListening(act.Key, act.Value);
        }
        if (interval != null)
        {
            StopCoroutine(interval);
        }
    }

    void OnMute() { Camera.main.GetComponent<AudioListener>().enabled = false; }
    void OnUnMute() { Camera.main.GetComponent<AudioListener>().enabled = true; }

    void OnGoal()
    {
        OnGoalSound();
        if (!endless)
        {
            Finished = true;
        }
    }

    void OnGoalSound()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
    void Press()
    {
        if (colorChangeTriggeredByUser)
        {
            GenerateNewColors();
        }
    }

    void OnPause() { Paused = true; }
    void OnUnPause() { Paused = false; }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GenerateNewColors()
    {
        var dominantColor = Mathf.CeilToInt(Random.Range(0, 3));
        float[] rgb = {
            Random.Range(0.0f, .5f),
            Random.Range(0.0f, .5f),
            Random.Range(0.0f, .5f),
        };

        rgb[dominantColor] = 1.0f;
        var r = rgb[0]; var g = rgb[1]; var b = rgb[2];
        currentBackgroundColor = new Color(r, g, b);
        currentContrastColor = new Color(1 - r, 1 - g, 1 - b);
        EventManager.TriggerEvent(Events.COLOR_CHANGE);
    }

    IEnumerator ChangeColor()
    {
        while (true)
        {
            GenerateNewColors();
            yield return new WaitForSeconds(colorChangeFrequency);
        }
    }
}
