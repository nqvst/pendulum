using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{

    [SerializeField] float colorShiftSpeed = 1;
    Camera cam;
    SpriteRenderer spriteRenderer;
    TrailRenderer trailRenderer;

    IEnumerator backgroundcoroutine;
    IEnumerator contrastcoroutine;
    IEnumerator trailcoroutine;

    void Start()
    {
        cam = GetComponent<Camera>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    void OnEnable()
    {
        EventManager.StartListening(Events.COLOR_CHANGE, OnColorChange);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.COLOR_CHANGE, OnColorChange);
    }

    void OnColorChange()
    {
        var backgroundColor = GameManager.instance.currentBackgroundColor;
        var contrastColor = GameManager.instance.currentContrastColor;
        if(cam != null)
        {
            if (backgroundcoroutine != null) StopCoroutine(backgroundcoroutine);
            backgroundcoroutine = LerpBackgroundColor(backgroundColor);
            StartCoroutine(backgroundcoroutine);
        }

        if(spriteRenderer != null)
        {
            if (contrastcoroutine != null) StopCoroutine(contrastcoroutine);
            contrastcoroutine = LerpSpriteColor(contrastColor);
            StartCoroutine(contrastcoroutine);
        }

        if(trailRenderer != null)
        {
            if (trailcoroutine != null) StopCoroutine(trailcoroutine);
            trailcoroutine = LerpTrailColor(contrastColor);
            StartCoroutine(trailcoroutine);
        }

    }

    IEnumerator LerpBackgroundColor(Color targetColor)
    { 
        while (targetColor != cam.backgroundColor)
        {
            cam.backgroundColor = Color.Lerp(cam.backgroundColor, targetColor, Time.deltaTime * colorShiftSpeed);
            yield return null;
        } 
    }

    IEnumerator LerpSpriteColor(Color targetColor)
    {
        while (targetColor != spriteRenderer.color)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, Time.deltaTime * colorShiftSpeed);
            yield return null;
        }
    }

    IEnumerator LerpTrailColor (Color targetColor)
    {
        while (targetColor != trailRenderer.startColor)
        {
            trailRenderer.startColor = Color.Lerp(trailRenderer.startColor, targetColor, Time.deltaTime * colorShiftSpeed);
            yield return null;
        }
    }
}
