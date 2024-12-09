using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset _playerController;

    [Header("Input Map Name References")]
    [SerializeField] private string _actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string _walk = "Walk";
    [SerializeField] private string _run = "Run";
    [SerializeField] private string _dash = "Dash";
    [SerializeField] private string _jump = "Jump";
    [SerializeField] private string _look = "Look";
    [SerializeField] private string _camera = "Camera";

    private InputAction _walkAction;
    private InputAction _runAction;
    private InputAction _dashAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _cameraAction;

    public Vector2 WalkInput { get; private set; }
    public float RunValue { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool DashTriggered { get; private set; }
    public bool CameraSwitched { get; private set; }


    // Start is called before the first frame update
    void Awake()
    {
        _walkAction = _playerController.FindActionMap(_actionMapName).FindAction(_walk);
        _runAction = _playerController.FindActionMap(_actionMapName).FindAction(_run);
        _dashAction = _playerController.FindActionMap(_actionMapName).FindAction(_dash);
        _jumpAction = _playerController.FindActionMap(_actionMapName).FindAction(_jump);
        _lookAction = _playerController.FindActionMap(_actionMapName).FindAction(_look);
        _cameraAction = _playerController.FindActionMap(_actionMapName).FindAction(_camera);
        RegisterInputActions();
    }

    private void Update()
    {
        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        _walkAction.performed += context => WalkInput = context.ReadValue<Vector2>();
        _walkAction.canceled += context => WalkInput = Vector2.zero;

        if(ButtonManager._isSprintBought)
        {
            _runAction.performed += context => RunValue = context.ReadValue<float>();
            _runAction.canceled += context => RunValue = 0f;
        }

        _lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        _lookAction.canceled += context => LookInput = Vector2.zero;

        _jumpAction.performed += context => JumpTriggered = true;
        
        _dashAction.performed += context => DashTriggered = true;

        _cameraAction.performed += context => CameraSwitched = !CameraSwitched;
    }

    public void ConsumeJump()
    {
        JumpTriggered = false;
    }

    public void ConsumeDash()
    {
        DashTriggered = false;
    }

    public void OnEnable()
    {
        _walkAction.Enable();
        _runAction.Enable();
        _dashAction.Enable();
        _jumpAction.Enable();
        _lookAction.Enable();
        _cameraAction.Enable();
    }

    public void OnDisable()
    {
        _walkAction.Disable();
        _runAction.Disable();
        _dashAction.Disable();
        _jumpAction.Disable();
        _lookAction.Disable();
        _cameraAction.Disable();
    }
}
