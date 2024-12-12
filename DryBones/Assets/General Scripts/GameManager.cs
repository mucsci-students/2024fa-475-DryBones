using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum SceneNames
    {
        MainMenu,
        Main
    }

    // Singleton instance
    public static GameManager Instance { get; private set; }

    public static bool _isReplay = false;   

    private bool _hasStartedIngameMusic = false; // Flag to track if the coroutine has started

    private Vector2 aspectRatio = Vector2.zero; // desired aspect ratio (e.g., 16:9)

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

        if (aspectRatio != Vector2.zero)
        {
            float x = Screen.height * (aspectRatio.x / aspectRatio.y);
            float y = Screen.height;
            Screen.SetResolution((int)x, (int)y, true); // true for full screen
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
        if (currentScene.name == SceneNames.Main.ToString() && !_hasStartedIngameMusic)
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
