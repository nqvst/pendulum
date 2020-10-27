using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void ZenMode()
    {
        SceneManager.LoadScene("ZenMode");
    }

    public void TimeAttack()
    {
        SceneManager.LoadScene("TimeAttack");
    }

    public void Endless()
    {
        SceneManager.LoadScene("Endless");
    }
}
