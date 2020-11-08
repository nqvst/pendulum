using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] List<Transform> transforms;
    [SerializeField] Queue<Transform> positions;
    private Vector2 nextPosition;

    private void OnGoal()
    {
        MoveToNextPoint();
    }

    void Start()
    {
        positions = new Queue<Transform>();
        transforms.ForEach(t => positions.Enqueue(t));
        MoveToNextPoint();
    }

    void MoveToNextPoint()
    {
        if (positions.Count > 0)
        {
            var index = positions.Count - 1;
            nextPosition = positions.Dequeue().position;
            transform.position = nextPosition;
        }
        else
        {
            Debug.Log("the Positions queue is empty!");
        }
    }

    void InstantiateExplosion()
    {
        // BOOM
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            EventManager.TriggerEvent(Events.PLAY_GOAL_SOUND);
            if (positions.Count == 0)
            {
                EventManager.TriggerEvent(Events.GOAL);
                EventManager.TriggerEvent(Events.STOP_TIMER);
            }
            else
            {
                MoveToNextPoint();
            }

        }
    }
}
