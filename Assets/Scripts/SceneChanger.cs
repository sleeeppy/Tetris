using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{
    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ToGame()
    {
        SceneManager.LoadScene(1);
    }
    
    public void FinishGame()
    {
        Application.Quit();
    }
}
