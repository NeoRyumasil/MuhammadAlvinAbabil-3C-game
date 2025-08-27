using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Play Button
    public void Play()
    {
        SceneManager.LoadScene("Gameplay");
    }

    // Exit Button
    public void Exit()
    {
        Application.Quit();
    }
}
