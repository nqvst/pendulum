using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalEndless : MonoBehaviour
{
    [SerializeField] Transform explosionPrefab;
    [SerializeField] int radius = 100;
    [SerializeField] int minRadius = 30;
    [SerializeField] int maxDistanceFromCenter = 400;

    private void OnGoal()
    {
        MoveToNewPosition();
    }

    private void Start()
    {
        MoveToNewPosition();
    }

    private Vector2 GetNextCenter()
    {
        Vector2 dir = transform.position;
        if (dir.magnitude + radius > maxDistanceFromCenter)
        {
            Debug.Log("magnitude + radius: " + dir.magnitude + radius);
            float padding = (dir.magnitude + radius) - maxDistanceFromCenter;
            Debug.Log("padding : " + padding);
            Vector2 dir2center = transform.position.normalized * -padding;
            return (Vector2)transform.position + dir2center;
        }
        return transform.position;
    }

    void MoveToNewPosition()
    {
        Vector2 randomPosition = Random.insideUnitCircle.normalized * Random.Range(minRadius, radius);
        Vector2 center = GetNextCenter();
        transform.position = randomPosition + center;
        InstantiateExplosion();
    }

    void InstantiateExplosion()
    {
        // BOOM
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            OnGoal();
            EventManager.TriggerEvent(Events.PLAY_GOAL_SOUND);
        }
    }
}
