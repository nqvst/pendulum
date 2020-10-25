using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HudManager : MonoBehaviour
{
    // Start is called before the first frame update
    bool paused = false;
    bool mute = false;
    bool goalReached = false;

    public void PauseButton()
    {
        paused = !paused;
        Debug.Log("pause [" + paused + "]");
        string e = paused ? Events.PAUSE : Events.UNPAUSE;
        EventManager.TriggerEvent(e);
    }

    void OnEnable()
    {
        EventManager.StartListening(Events.GOAL, OnGoal);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.GOAL, OnGoal);
    }

    void OnGoal()
    {
        goalReached = true;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleMute()
    {
        mute = !mute;
        string e = mute ? Events.MUTE : Events.UNMUTE;
        EventManager.TriggerEvent(e);
    }
}
