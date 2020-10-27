using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] Transform explosionPrefab;
    [SerializeField] int radius;

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

    private void Start()
    {
        MoveToNewPosition();
    }

    void MoveToNewPosition()
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
