using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] Transform explosionPrefab;

    void Start()
    {
        
    }

    void OnEnable()
    {
        EventManager.StartListening(Events.GOAL, OnGoal);
    }

    private void OnDisable()
    {
        EventManager.StopListening(Events.GOAL, OnGoal);
    }

    private void OnGoal()
    {
        MoveToNewPosition();
    }

    void MoveToNewPosition(float radius = 200)
    {
        Vector2 randomPosition = Random.insideUnitCircle.normalized * Random.Range(0, radius);
        transform.position = randomPosition;
        InstantiateExplosion();
    }

    void InstantiateExplosion()
    {
        // BOOM
    }
}
