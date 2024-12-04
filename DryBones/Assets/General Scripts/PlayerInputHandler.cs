using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance { get; private set; }


    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset _playerController;

    [Header("Input Map Name References")]
    [SerializeField] private string _actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string _walk = "Walk";
    [SerializeField] private string _run = "Run";
    [SerializeField] private string _jump = "Jump";
    [SerializeField] private string _look = "Look";

    private InputAction _walkAction;
    private InputAction _runAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;

    public Vector2 WalkInput { get; private set; }
    public float RunValue { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }


    // Start is called before the first frame update
    void Awake()
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

        _walkAction = _playerController.FindActionMap(_actionMapName).FindAction(_walk);
        _runAction = _playerController.FindActionMap(_actionMapName).FindAction(_run);
        _jumpAction = _playerController.FindActionMap(_actionMapName).FindAction(_jump);
        _lookAction = _playerController.FindActionMap(_actionMapName).FindAction(_look);
        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        _walkAction.performed += context => WalkInput = context.ReadValue<Vector2>();
        _walkAction.canceled += context => WalkInput = Vector2.zero;

        _runAction.performed += context => RunValue = context.ReadValue<float>();
        _runAction.canceled += context => RunValue = 0f;

        _lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        _lookAction.canceled += context => LookInput = Vector2.zero;

        _jumpAction.performed += context => JumpTriggered = true;
        _jumpAction.canceled += context => JumpTriggered = false;
    }

    private void OnEnable()
    {
        _walkAction.Enable();
        _runAction.Enable();
        _jumpAction.Enable();
        _lookAction.Enable();
    }

    private void OnDisable()
    {
        _walkAction.Disable();
        _runAction.Disable();
        _jumpAction.Disable();
        _lookAction.Disable();
    }
}
