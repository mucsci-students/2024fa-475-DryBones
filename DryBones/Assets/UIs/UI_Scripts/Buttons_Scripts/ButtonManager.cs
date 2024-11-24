using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public enum SceneNames
    {
        MainMenu,
        PlayerTest
    }

    [Header("Canvases")]
    [SerializeField] private GameObject _mainMenuCanvas;  
    [SerializeField] private GameObject _settingMenuCanvas;
    [SerializeField] private GameObject _tutorialMenuCanvas;
    [SerializeField] private GameObject _informationMenuCanvas;
    [SerializeField] private GameObject _mainGameCanvas;
    [SerializeField] private GameObject _pauseCanvas;

    [Header("Button To Show From Pause Menu")]
    [SerializeField] private GameObject _resumeButtonInMainMenu;
    [SerializeField] private GameObject _resumeButtonInSettingMenu;

    [Header("Information Panel")]
    [SerializeField] private GameObject _renderObjectSliderPanel;
    [SerializeField] private GameObject _masterVolumeSliderPanel;
    [SerializeField] private GameObject _sfxVolumeSliderPanel;

    [Header("Texts")]
    [SerializeField] private TMP_Text _renderObjectSliderText;
    [SerializeField] private TMP_Text _masterVolumeSliderText;
    [SerializeField] private TMP_Text _sfxVolumeSliderText;

    [Header("Inactive Panels")]
    [SerializeField] private GameObject _masterVolumeInactive;
    [SerializeField] private GameObject _sfxVolumeInactive;

    private bool _isMasterVolumeInactive = false;
    private bool _isSfxVolumeInactive = false;

    private bool _isPause = false;

    Scene _currentScene;

    private void Start()
    {
        MainMenu();
        _currentScene = SceneManager.GetActiveScene();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPause)
            {
                ResumeButton();
            }
            else
            {
                PauseButton();
            }
        }
    }

    // Method to start playing game
    public void PlayButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("PlayerTest");
        _mainGameCanvas.SetActive(true);
        TurnOffAllCanvasInMainMenu();
    }

    // Method to quit the game
    public void QuitButton()
    {
        // This will only work in a built version of the game, not in the editor
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void TurnOffAllCanvasInMainMenu()
    {
        _mainMenuCanvas.SetActive(false);
        _settingMenuCanvas.SetActive(false);
        _tutorialMenuCanvas.SetActive(false);
        _informationMenuCanvas.SetActive(false);
        _pauseCanvas.SetActive(false);
    }

    public void MainMenu()
    {
        _mainMenuCanvas.SetActive(true);
        _settingMenuCanvas.SetActive(false);
        _tutorialMenuCanvas.SetActive(false);
        _informationMenuCanvas.SetActive(false);

        _mainGameCanvas.SetActive(false);
        _pauseCanvas.SetActive(false);
    }

    // Setting menu
    public void SettingMenu()
    {
        TurnOffPanel();
        _informationMenuCanvas.SetActive(false);
        _mainMenuCanvas.SetActive(false);
        _tutorialMenuCanvas.SetActive(false);
        _settingMenuCanvas.SetActive(true);

        _mainGameCanvas.SetActive(false);
        _pauseCanvas.SetActive(false);
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

    public void PauseButton()
    {
        _isPause = true;
        Time.timeScale = 0f;
        _pauseCanvas.SetActive(true);
        _resumeButtonInMainMenu.SetActive(true);
        _resumeButtonInSettingMenu.SetActive(true);
        _mainGameCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible
    }

    public void ResumeButton()
    {
        _isPause = false;
        Time.timeScale = 1f;
        _pauseCanvas.SetActive(false);
        _mainGameCanvas.SetActive(true);
        TurnOffAllCanvasInMainMenu();

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
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
        _isMasterVolumeInactive = !_isMasterVolumeInactive;
        _masterVolumeInactive.SetActive(_isMasterVolumeInactive);
    }

    public void ToggleSFX()
    {
        AudioManager.Instance._sfxAudioSource.mute = !AudioManager.Instance._sfxAudioSource.mute;
        _isSfxVolumeInactive = !_isSfxVolumeInactive;
        _sfxVolumeInactive.SetActive(_isSfxVolumeInactive);
    }

    public void ClosePanelButton()
    {
        TurnOffPanel();
    }
}
