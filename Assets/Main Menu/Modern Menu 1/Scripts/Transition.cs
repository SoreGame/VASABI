using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public void ChooseScene(int a)
    {
        SceneManager.LoadScene(a);
    }

    public void Exit()
    {
        Application.Quit();
    }
}