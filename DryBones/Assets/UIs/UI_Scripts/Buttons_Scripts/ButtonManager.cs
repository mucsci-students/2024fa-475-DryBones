using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject _mainMenuCanvas;  
    [SerializeField] private GameObject _settingMenuCanvas;
    [SerializeField] private GameObject _tutorialMenuCanvas;
    [SerializeField] private GameObject _informationMenuCanvas;

    [Header("Information Panel")]
    [SerializeField] private GameObject _renderObjectSliderPanel;
    [SerializeField] private GameObject _masterVolumeSliderPanel;
    [SerializeField] private GameObject _sfxVolumeSliderPanel;

    [Header("Texts")]
    [SerializeField] private TMP_Text _renderObjectSliderText;
    [SerializeField] private TMP_Text _masterVolumeSliderText;
    [SerializeField] private TMP_Text _sfxVolumeSliderText;

    private void Start()
    {
        MainMenu();
    }

    // Method to start playing game
    public void PlayButton()
    {
        SceneManager.LoadScene("PlayerTest");
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
        _tutorialMenuCanvas.SetActive(false);
        _informationMenuCanvas.SetActive(false);
    }

    // Setting menu
    public void SettingMenu()
    {
        TurnOffPanel();
        _informationMenuCanvas.SetActive(false);
        _mainMenuCanvas.SetActive(false);
        _tutorialMenuCanvas.SetActive(false);
        _settingMenuCanvas.SetActive(true);
    }

    // Tutorial menu
    public void TutorialMenu()
    {
        _informationMenuCanvas.SetActive(false);
        _mainMenuCanvas.SetActive(false);
        _settingMenuCanvas.SetActive(false);
        _tutorialMenuCanvas.SetActive(true);
    }

    // Information menu
    public void InformationMenu()
    {
        _mainMenuCanvas.SetActive(false);
        _settingMenuCanvas.SetActive(false);
        _tutorialMenuCanvas.SetActive(false);
        _informationMenuCanvas.SetActive(true);
    }

    

    public void RenderObjectInfoButton()
    {
        _renderObjectSliderPanel.SetActive(true);
        _masterVolumeSliderPanel.SetActive(false);
        _sfxVolumeSliderPanel.SetActive(false);
        _renderObjectSliderText.text = "Adjusts how many objects are rendered in the game. Higher values show more objects, while lower values show fewer.";
    }

    public void MasterVolumeInfoButton()
    {
        _masterVolumeSliderPanel.SetActive(true);
        _masterVolumeSliderText.text = "Controls the overall volume of the game, affecting all audio, including music and sound effects.";
        _renderObjectSliderPanel.SetActive(false);
        _sfxVolumeSliderPanel.SetActive(false);
    }

    public void SFXVolumeInfoButton()
    {
        _sfxVolumeSliderPanel.SetActive(true);
        _sfxVolumeSliderText.text = "Adjusts the volume of sound effects in the game, such as footsteps, gunshots, and environment sounds.";
        _renderObjectSliderPanel.SetActive(false);
        _masterVolumeSliderPanel.SetActive(false);
    }

    public void TurnOffPanel()
    {
        // Initially set all panels active status to be false
        _renderObjectSliderPanel.SetActive(false);
        _masterVolumeSliderPanel.SetActive(false);
        _sfxVolumeSliderPanel.SetActive(false);
    }

    public void ToggleThemeMusic()
    {
        AudioManager.Instance._themeAudioSource.mute = !AudioManager.Instance._themeAudioSource.mute;
    }

    public void ToggleSFX()
    {
        AudioManager.Instance._sfxAudioSource.mute = !AudioManager.Instance._sfxAudioSource.mute;
    }

    public void ClosePanelButton()
    {
        TurnOffPanel();
    }
}
