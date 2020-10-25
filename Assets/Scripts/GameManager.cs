﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {


    List<IEnumerator> startSequence = new List<IEnumerator>();

	[SerializeField] int targetFrameRate = 60;
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
	}

	void Awake() {
		Application.targetFrameRate = targetFrameRate;
	}
	void OnEnable()
	{
		actions = new Dictionary<string, UnityAction>() {
			{ Events.PRESS, Press},
			{ Events.PAUSE, OnPause},
			{ Events.UNPAUSE, OnUnPause},
			{ Events.GOAL, OnGoal},
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
	}

	void OnMute() { Camera.main.GetComponent<AudioListener>().enabled = false; }
	void OnUnMute() { Camera.main.GetComponent<AudioListener>().enabled = true; }

	void OnGoal() {
		Finished = true;
		if(audioSource != null)
        {
			audioSource.Play();
        }
	}
	void Press() { GenerateNewColors();}

	void OnPause() { Paused = true; }
	void OnUnPause() { Paused = false; }

	public void MainMenu()
    {
		SceneManager.LoadScene("MainMenu");
    }

	void GenerateNewColors()
	{
		var r = Random.Range(0.0f, 1.0f);
		var g = Random.Range(0.0f, 1.0f);
		var b = Random.Range(0.0f, 1.0f);

		currentBackgroundColor = new Color(r, g, b);
		currentContrastColor = new Color(1 - r, 1 - g, 1 - b);
		EventManager.TriggerEvent(Events.COLOR_CHANGE);
	}

}