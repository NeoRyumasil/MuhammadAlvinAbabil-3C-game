using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{

    // Set Object References
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private string _mainMenuSceneName;

    // Start
    private void Start()
    {
        _inputManager.OnMainMenuInput += BackToMainMenu;
    }

    // Update
    private void OnDestroy()
    {
        _inputManager.OnMainMenuInput -= BackToMainMenu;
    }

    // Back to main menu
    private void BackToMainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(_mainMenuSceneName);
    }
}
