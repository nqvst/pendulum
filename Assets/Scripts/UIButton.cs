using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    [SerializeField] string eventName;


    public virtual void OnClick()
    {
        Debug.Log("on click not implemented");
    }
}
