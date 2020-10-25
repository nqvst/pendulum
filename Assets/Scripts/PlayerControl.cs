using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerControl : MonoBehaviour
{
    public GameObject ClosestPoint { get; private set; }
    public GameObject CurrentPoint { get; private set; }

    List<GameObject> swingPoints;
    DistanceJoint2D grapple;
    Rigidbody2D rb;

    [SerializeField] Transform graphicsPrefab;
    [SerializeField] Color closestColor;

    [SerializeField] float colorShiftSpeed = 1;
    [SerializeField] float targetSwitchSpeed = 1;
    [SerializeField] Gradient grad;

    [SerializeField] float spinningForce = 1000;
    [SerializeField] float graphicsFollowSpeed;

    [SerializeField] float maxDistanceFromCenter = 1000f;

    private bool hasStarted = false;
    private bool pointsCreated = false;

    private PlayerInput playerInput;

    private bool isPressing = false;
    private bool isPaused = false;

    [SerializeField] AudioClip clip;
    private AudioSource audioData;

    private Transform playerGraphics;

    private GameManager gameManager;

    private Dictionary<string, UnityAction> actions;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.simulated = false;

        audioData = GetComponent<AudioSource>();

        swingPoints = new List<GameObject>();

        playerGraphics = Instantiate(graphicsPrefab, transform.position, Quaternion.identity);

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("GrapplePoint"))
        {
            swingPoints.Add(go);
        }

        playerInput = GetComponent<PlayerInput>();

        gameManager = GameManager.instance;
    }

    void OnEnable()
    {
        actions = new Dictionary<string, UnityAction>() {
            { Events.START, OnStart },
            { Events.POINTS_CREATED, OnPointsCreated },
            { Events.PRESS, Press},
            { Events.UNPRESS, OnUnPress },
            { Events.PAUSE, OnPause },
            {Events.UNPAUSE, OnUnPause },
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

    void Press()
    {
        isPressing = true;
        if (gameManager.Finished)
        {
            gameManager.MainMenu();
        }
    }

    void OnUnPress()
    {
        isPressing = false;
    }

    void OnPointsCreated()
    {
        pointsCreated = true;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("GrapplePoint"))
        {
            swingPoints.Add(go);
        }
    }

    void OnStart()
    {
        hasStarted = true;
        if (!isPaused)
        {
            UnPausePlayer();
        }
    }

    void OnPause()
    {
        PausePlayer();
        isPaused = true;
    }

    void OnUnPause()
    {
        isPaused = false;
        if (hasStarted) {
            UnPausePlayer();
        }
    }

    void PausePlayer()
    {
        rb.isKinematic = true;
        rb.simulated = false;  
    }

    void UnPausePlayer()
    {
        rb.isKinematic = false;
        rb.simulated = true;
       
    }

    GameObject FindClosestPoint()
    {
        GameObject closest = null;
        foreach (GameObject go in swingPoints)
        {
            //var dir = new Ray2D(pos, (go.transform.position - pos).normalized);
            //var hit = Physics2D.Raycast(pos, dir.direction);
            //Debug.DrawRay(pos, dir.direction * 100, Color.green, 2);

            if (closest == null)
            {
                closest = go;
            }
            else
            {
                var currentDistance = Vector2.Distance(playerGraphics.position, closest.transform.position);
                var nextDistance = Vector2.Distance(playerGraphics.position, go.transform.position);
                if (currentDistance > nextDistance)
                {
                    closest = go;
                }
            
            }
        }
        return closest;
    }

    void Update()
    {
        // make sure the game has started
        if (!hasStarted || !pointsCreated) return;

        // make sure whe haven't paused or finished the game
        if (isPaused || gameManager.Finished) return;

        ClosestPoint = FindClosestPoint();

        if (playerInput.isPressing && CurrentPoint == null)
        {
            GrabPoint(ClosestPoint);
        }

        if(!playerInput.isPressing && CurrentPoint != null)
        {
            LetGo();
        }


        if (CurrentPoint != null && playerInput.isPressing)
        {
            rb.AddForce(rb.velocity.normalized * spinningForce * Time.deltaTime);
        }


        // simple out of ounds check that places the player back to the center of the map.
        Vector3 pos = transform.position;
        var distFromCenter = Vector2.Distance(Vector2.zero, pos);
        if (distFromCenter >= maxDistanceFromCenter)
        {
            transform.position = Vector2.zero;
            LetGo();
            rb.velocity = Vector2.zero;
        }
    }

   

    void GrabPoint(GameObject point)
    {
        if (!CurrentPoint)
        {
            CurrentPoint = point;

            grapple = gameObject.AddComponent<DistanceJoint2D>();
            grapple.autoConfigureDistance = true;
            grapple.connectedBody = point.GetComponent<Rigidbody2D>();

            audioData.volume = 0.3f;
            audioData.pitch = Random.Range(0.6f, .7f);
            audioData.Play();
        }
    }

    void LetGo()
    {
        if (CurrentPoint != null)
        {
            Destroy(grapple);
        }
        
        CurrentPoint = null;

        audioData.volume = 0.06f;
        audioData.pitch = Random.Range(1.2f, 1.3f);
        audioData.Play();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.CompareTag("Goal"))
        {
            LetGo();
            ClosestPoint = null;
            Debug.Log("player reached the goal");
            PausePlayer();
            EventManager.TriggerEvent(Events.GOAL);
            EventManager.TriggerEvent(Events.STOP_TIMER);
            StartCoroutine(GoToGoal(other.transform.position));
        }
    }

    IEnumerator GoToGoal(Vector2 goalPosition)
    {
        Debug.Log("start coroutine to move player to center of coal");
        float distance = Vector2.Distance(transform.position, goalPosition);
        while (distance > 0.1f)
        {
            Debug.Log("distance to center of goal = " + distance);
            transform.position = Vector3.Lerp(transform.position, goalPosition, Time.deltaTime * 2);
            yield return null;
        }
    }
}