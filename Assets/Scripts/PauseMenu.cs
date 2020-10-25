using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 1;

    private CanvasGroup canvasGroup;
    private GameManager gameManager;

    IEnumerator fadeCoroutine;

    void Start()
    {
        gameManager = GameManager.instance;
        canvasGroup = GetComponent<CanvasGroup>();

    }

    void OnEnable()
    {
        EventManager.StartListening(Events.PAUSE, OnPause);
        EventManager.StartListening(Events.UNPAUSE, OnUnPause);
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.PAUSE, OnPause);
        EventManager.StopListening(Events.UNPAUSE, OnUnPause);
    }

    void OnUnPause()
    {
        SafeStartCoroutine(0);
    }

    void OnPause()
    {
        SafeStartCoroutine(1);
    }

    void SafeStartCoroutine(float targetAlpha)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = FadeTo(targetAlpha);
        StartCoroutine(fadeCoroutine); 
    }

    IEnumerator FadeTo(float targetAlpha)
    {
        while (Mathf.Abs(canvasGroup.alpha - targetAlpha) > 0.05f)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}
