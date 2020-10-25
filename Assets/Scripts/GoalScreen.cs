using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoalScreen : MonoBehaviour
{
    Text text;
    void Start()
    {
        
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
     
    }
}
