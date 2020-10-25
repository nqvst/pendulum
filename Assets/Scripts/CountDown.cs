using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    AudioSource audio;
    IEnumerator counter;
    Text text;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        text = GetComponent<Text>();
    }

    void OnEnable()
    {
        EventManager.StartListening(Events.START_COUNTDOWN, OnStartCountDown);
        
    }

    void OnDisable()
    {
        EventManager.StopListening(Events.START_COUNTDOWN, OnStartCountDown);
        
    }

    void OnStartCountDown() {
        counter = Counter();
        StartCoroutine(counter);
    }

    void Done()
    {
        EventManager.TriggerEvent(Events.POINTS_CREATED);
        EventManager.TriggerEvent(Events.START);
        EventManager.TriggerEvent(Events.START_TIMER);
        gameObject.SetActive(false);
    }

    IEnumerator Counter()
    {
        //yield return new WaitForSeconds(1);
        int count = 3; 
        bool goTime;
        while (count >= 0)
        {
            if (text != null)
            {   
                goTime = count == 0;
                text.text = goTime ? "GO" : count.ToString();
                audio.pitch = goTime ? 1.7f : 1f;
                audio.Play();
                count--;
                yield return new WaitForSeconds(1);
            }
            yield return null;
        }
        Done();
    }

}
