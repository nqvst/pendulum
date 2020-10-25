using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
public class ShowHighlights : MonoBehaviour
{
    [SerializeField] float panSpeed = 1;
    Transform highlightsContainer;
    List<Vector2> points;

    IEnumerator followCoroutine;
    IEnumerator abortCoroutine;

    GameObject player;
    void Start()
    {
        points = new List<Vector2>();
        highlightsContainer = GameObject.FindGameObjectWithTag("Path").transform;
        player  = GameObject.FindGameObjectWithTag("Player");

        foreach (Transform highlight in highlightsContainer)
        {
            Debug.Log("hl: " + highlight.name);
            points.Add(highlight.position);
        }
        if (player != null)
        {
            // go back to player in the end
            points.Add(player.transform.position);
        }

        if(points.Count > 0)
        {
            followCoroutine = FollowPath(points);
            StartCoroutine(followCoroutine);
        }
    }

    private void Done()
    {
        //Debug.Log("highlights are done, Start the game already! [" + Events.START + "]");
        //EventManager.TriggerEvent(Events.START);
        EventManager.TriggerEvent(Events.START_COUNTDOWN);
        enabled = false;
    }

    IEnumerator FollowPath(List<Vector2> targets)
    {
        foreach (Vector2 target in targets)
        {
            while (Vector2.Distance(transform.position, target) > 0.2f)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(target.x, target.y, transform.position.z), panSpeed * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(.1f);
        }
        Done();
    }

    IEnumerator Abort()
    {
        Vector2 playerPos = player.transform.position;
        while (Vector2.Distance(transform.position, playerPos) > 0.2f)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(playerPos.x, playerPos.y, transform.position.z), panSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void CancelPath()
    {
        StopCoroutine(followCoroutine);
        StartCoroutine(Abort());
        EventManager.TriggerEvent(Events.START_COUNTDOWN);
        //enabled = false;
        EventManager.StopListening(Events.PRESS, CancelPath);

    }

    void OnEnable()
    {
        EventManager.StartListening(Events.PRESS, CancelPath);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.PRESS, CancelPath);
    }


}
