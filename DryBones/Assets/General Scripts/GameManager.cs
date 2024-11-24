using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum SceneNames
    {
        MainMenu,
        PlayerTest
    }

    // Singleton instance
    public static GameManager Instance { get; private set; }

    private bool _hasStartedIngameMusic = false; // Flag to track if the coroutine has started

    private void Awake()
    {
        // Check if an instance of GameManager already exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManager
        }
    }

    private void Start()
    {
        Debug.Log("Playing Theme Music: AllMenuAudio");
        AudioManager.Instance.PlayThemeMusic("AllMenuAudio");
    }

    private void Update()
    {
        SceneCheck();
    }

    private void SceneCheck()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == SceneNames.PlayerTest.ToString() && !_hasStartedIngameMusic)
        {
            _hasStartedIngameMusic = true; // Set the flag
            StartCoroutine(PlayIngameMusicAfterDelay());
        }
    }

    private System.Collections.IEnumerator PlayIngameMusicAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlayThemeMusic("IngameAudio");
    }
}
