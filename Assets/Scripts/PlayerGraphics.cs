using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraphics : MonoBehaviour
{
    [SerializeField] float graphicsFollowSpeed = 5;
    private PlayerControl player;
    private LineRenderer lineRenderer;

    //private PlayerInput playerInput;

    [SerializeField] private Color lineColorActive = Color.white;
    [SerializeField] private Color lineColorInactive = Color.clear;


    void Start()
    {
        //playerInput = FindObjectOfType<PlayerInput>();

        player = FindObjectOfType<PlayerControl>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        Vector3 playerPos = player.transform.position;

        if (playerPos != pos)
        {
            float dist = Vector2.Distance(playerPos, pos);
            transform.position = Vector2.Lerp(pos, playerPos, graphicsFollowSpeed * Time.fixedDeltaTime * dist);
        }

        if (player.ClosestPoint != null)
        {
            lineRenderer.startColor = lineColorInactive;
            lineRenderer.endColor = lineColorInactive;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, player.ClosestPoint.transform.position);
        }

        // player has grabbed a point and is holding down 
        if (player.CurrentPoint != null)
        {
            
            lineRenderer.startColor = lineColorActive;
            lineRenderer.endColor = lineColorActive;
            lineRenderer.SetPosition(1, player.CurrentPoint.transform.position);
        }

        // goal is reached and we explicitly set closest and current point to null in player control.
        if(player.CurrentPoint == null && player.ClosestPoint == null)
        {
            lineRenderer.SetPosition(1, transform.position);
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.startColor = Color.clear;
            lineRenderer.endColor = Color.clear;
        }
    }
}
