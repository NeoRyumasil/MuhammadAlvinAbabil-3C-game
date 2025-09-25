using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Scene Name
    [SerializeField] private string _gameSceneName;

    // Play Button
    public void Play()
    {
        SceneManager.LoadScene(_gameSceneName);
    }

    // Exit Button
    public void Exit()
    {
        Application.Quit();
    }
}
