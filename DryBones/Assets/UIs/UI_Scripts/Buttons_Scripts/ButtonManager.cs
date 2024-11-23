using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject _mainMenuCanvas;  
    [SerializeField] private GameObject _settingMenuCanvas;

    [Header("Information Panel")]
    [SerializeField] private GameObject _renderObjectSliderPanel;
    [SerializeField] private GameObject _masterVolumeSliderPanel;
    [SerializeField] private GameObject _sfxVolumeSliderPanel;

    [Header("Texts")]
    [SerializeField] private GameObject _renderObjectSliderText;
    [SerializeField] private GameObject _masterVolumeSliderText;
    [SerializeField] private GameObject _sfxVolumeSliderText;

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
        TurnOffPanel();
        _mainMenuCanvas.SetActive(false);
        _settingMenuCanvas.SetActive(true);
    }

    public void ClosePanelButton()
    {
        TurnOffPanel();
    }

    public void RenderObjectInfoButton()
    {
        _renderObjectSliderPanel.SetActive(true);
    }

    public void MasterVolumeInfoButton()
    {
        _masterVolumeSliderPanel.SetActive(true);
    }

    public void SFXVolumeInfoButton()
    {
        _sfxVolumeSliderPanel.SetActive(true);
    }

    public void TurnOffPanel()
    {
        // Initially set all panels active status to be false
        _renderObjectSliderPanel.SetActive(false);
        _masterVolumeSliderPanel.SetActive(false);
        _sfxVolumeSliderPanel.SetActive(false);
    }
}
