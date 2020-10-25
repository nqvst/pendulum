using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] InputAction grab;

    public bool isPressing = false;

    void Awake()
    {
        grab.started += ctx => Press();
        grab.performed += ctx => Press();
        grab.canceled += ctx => UnPress();
    }

    void Press()
    {
        isPressing = true;
        EventManager.TriggerEvent(Events.PRESS);
    }

    void UnPress()
    {
        isPressing = false;
        EventManager.TriggerEvent(Events.UNPRESS);
    }

    void OnEnable()
    {
        grab.Enable();
    }

    void OnDisable()
    {
        grab.Disable();

    }
}
