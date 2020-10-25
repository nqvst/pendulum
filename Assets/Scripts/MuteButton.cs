using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MuteButton : MonoBehaviour
{
    [SerializeField] Sprite activeImage;
    [SerializeField] Sprite inactiveImage;

    private Image image;
    public bool mute = false;

    private void Start()
    {
        image = transform.GetChild(0).GetComponentInChildren<Image>();
    }

    public void OnClick()
    {
        mute = !mute;
        image.sprite = mute ? activeImage : inactiveImage;

        string e = mute ? Events.MUTE : Events.UNMUTE;
        EventManager.TriggerEvent(e);
    }

}
