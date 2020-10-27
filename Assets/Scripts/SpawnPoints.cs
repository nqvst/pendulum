using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    [SerializeField] float spreadSpeed = 1;

    [SerializeField] Transform pointPrefab;
    [SerializeField] int amountOfPoints;
    [SerializeField] float degree = 137.5f;
    [SerializeField] float distance = 0.5f;
    [SerializeField] bool sendStartEvent = false;
   

    [SerializeField] float scale = 10;

    private List<Transform> points;

    private Vector2 CalculatThing(float deg, float scale, int count)
    {
        double angle = count * (deg * Mathf.Deg2Rad);
        float radius = scale * Mathf.Sqrt(count);
        float x = radius * (float)System.Math.Cos(angle);
        float y = radius * (float)System.Math.Sin(angle);
        return new Vector2(x,y);
    }

    IEnumerator spawner;

    void Start()
    {
        points = new List<Transform>();
        spawner = SpawnAndMovePoints(amountOfPoints, distance, degree);   
        StartCoroutine(spawner);
    }

    void SpawnPointsInCircle(int amount, float minRadius = 0, float maxRadius = 200)
    {

        for (int i = 0; i < amount; i++)
        {
            Vector2 randomPosition = Random.insideUnitCircle.normalized * Random.Range(minRadius, maxRadius);
            Instantiate(pointPrefab, (Vector2)transform.position + randomPosition, Quaternion.identity);
        }
        EventManager.TriggerEvent(Events.POINTS_CREATED);
    }

    void SpawnPointsInFib(int amount)
    {

        for (int i = 0; i < amount; i++)
        {
            Vector2 pos = CalculatThing(degree, distance, i);
            Instantiate(pointPrefab, (Vector2)transform.position + pos, Quaternion.identity);
        }
        EventManager.TriggerEvent(Events.POINTS_CREATED);
    }

    IEnumerator SpawnAndMovePoints(int amount, float targetDistance, float targetAngle)
    {
        float currentDist = 0;
        float currentAngle = 130;
        yield return new WaitForSeconds(.4f);

        for (int i = 1; i < amount; i++)
        {
            Transform point = Instantiate(pointPrefab, (Vector2)transform.position, Quaternion.identity);
            points.Add(point);
        }

        while (Mathf.Abs(currentDist - targetDistance) > 0.1f)
        {
            currentDist = Mathf.Lerp(currentDist, targetDistance, Time.deltaTime * spreadSpeed);
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * spreadSpeed * 2);

            for (int i = 1; i < points.Count; i++)
            {
                var newPos = CalculatThing(currentAngle, currentDist, i);
                points[i].position = newPos + (Vector2)transform.position;
            }
            yield return null;
        }

        if(sendStartEvent)
        {
            EventManager.TriggerEvent(Events.POINTS_CREATED);
            EventManager.TriggerEvent(Events.START);
        }

    }
}
