using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Waypoint : MonoBehaviour
{

    Image image;
    [SerializeField] Color color;
    [SerializeField] float maxAlpha;
    Transform target;
    [SerializeField] float paddingMultiplyer;
    [SerializeField] float offsetAngle = 0;

    private float targetAlpha = 1;

    GameManager gameManager;
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Goal").transform;
        image = GetComponent<Image>();
        gameManager = GameManager.instance;
    }

    void LateUpdate()
    {
        Vector2 padding = new Vector2(Screen.height, Screen.width).normalized * paddingMultiplyer;
        float minX = padding.x + (image.GetPixelAdjustedRect().width / 2);
        float maxX = -padding.x + (Screen.width - minX);

        float minY = padding.y + (image.GetPixelAdjustedRect().height / 2);
        float maxY = -padding.y + Screen.height - minY;

        Vector2 targetPos = Camera.main.WorldToScreenPoint(target.position);
        Vector2 arrowPos = Camera.main.WorldToScreenPoint(target.position);

        // stay inside the viewport
        arrowPos.x = Mathf.Clamp(arrowPos.x, minX, maxX);
        arrowPos.y = Mathf.Clamp(arrowPos.y, minY, maxY);

        Vector2 viewPortPos = Camera.main.WorldToViewportPoint(target.position);
        bool isInView = viewPortPos.x > 0 && viewPortPos.x < 1 && viewPortPos.y > 0 && viewPortPos.y < 1;

        color = gameManager.currentContrastColor;
        if (isInView)
        {
            color.a = 0;
        }
        else
        {
            color.a = maxAlpha;
        }

        image.color = Color.Lerp(image.color, color, Time.deltaTime * 2);

        transform.position = Vector2.Lerp(transform.position, arrowPos, Time.deltaTime * 2);

        // Rotate the arrow to point towards the target.
        Vector2 diff = targetPos - (Vector2)transform.position;


        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - offsetAngle);
    }
}
