using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuCanvas;  
    [SerializeField] private GameObject _settingMenuCanvas;

    private void Start()
    {
        MainMenu();
    }

    // Method to start playing game
    public void PlayButton()
    {
        SceneManager.LoadScene("Player Test");
    }

    // Method to quit the game
    public void QuitButton()
    {
        // This will only work in a built version of the game, not in the editor
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void MainMenu()
    {
        _mainMenuCanvas.SetActive(true);
        _settingMenuCanvas.SetActive(false);
    }

    // Setting menu
    public void SettingMenu()
    {
        _mainMenuCanvas.SetActive(false);
        _settingMenuCanvas.SetActive(true);
    }
}
