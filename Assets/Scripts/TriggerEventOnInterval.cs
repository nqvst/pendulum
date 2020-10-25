using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventOnInterval : MonoBehaviour
{
    [SerializeField] string eventName;
    [SerializeField] float interval = 1;
    [SerializeField] int maxRepeates = 0;
    IEnumerator repeater;

    void Start()
    {
        if(eventName != null)
        {
            repeater = Repeate();
            StartCoroutine(repeater);

        } else
        {
            Debug.Log("The repeater needs an event name to trigger.");
        }
    }

    IEnumerator Repeate ()
    {
        var currentRepeates = 0;
        while (currentRepeates < maxRepeates || maxRepeates == 0)
        {
            EventManager.TriggerEvent(eventName);
            yield return new WaitForSeconds(interval);
        }
    }
}
