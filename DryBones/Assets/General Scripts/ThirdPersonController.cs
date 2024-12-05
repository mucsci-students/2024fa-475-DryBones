using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{

    [Header("Movement Speeds")]
    [SerializeField] private float _walkSpeed = 10f;
    [SerializeField] private float _runSpeedMultiplier = 1.5f;

    [Header("Jump Parameters")]
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private float _doubleJumpMultiplier = 1.5f;
    [SerializeField] private int _maxJump = 3;

    [Header("Dash Parameters")]
    [SerializeField] private float _dashSpeed = 15f;

    [Header("Look Sensitivity")]
    [SerializeField] private float _mouseSensitivity = 2f;
    [SerializeField] private float _updownRange = 90f;

    [Header("Player Input Handler And Camera")]
    [SerializeField] private PlayerInputHandler _playerInputHandler;
    [SerializeField] private Camera _mainCamera;

    private CharacterController _characterController;
    private Vector3 _currentMovement;
    private float _verticalRotation;
    private int _jumpCount = 0;

    public static bool _isDoubleJump = true;

    // Start is called before the first frame update
    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        float speed = _walkSpeed * (_playerInputHandler.RunValue > 0 ? _runSpeedMultiplier : 1f);

        Vector3 inputDirection = new Vector3(_playerInputHandler.WalkInput.x, 0f, _playerInputHandler.WalkInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        worldDirection.Normalize();

        _currentMovement.x = worldDirection.x * speed;
        _currentMovement.z = worldDirection.z * speed;
        HandleJumping();
        HandleDashing();
        _characterController.Move(_currentMovement * Time.deltaTime);
    }

    private void HandleJumping()
    {
        if (_characterController.isGrounded)
        {
            _currentMovement.y = -1f;

            if (_playerInputHandler.JumpTriggered && _jumpCount > 0)
            {
                _playerInputHandler.ConsumeJump();
            }

            _jumpCount = 0;

            if (_playerInputHandler.JumpTriggered)
            {
                _jumpCount++;
                _currentMovement.y = _jumpForce;
                _playerInputHandler.ConsumeJump(); // Reset JumpTriggered
            }
        }
        else
        {
            if (_playerInputHandler.JumpTriggered && _isDoubleJump && _jumpCount < _maxJump)
            {
                ++_jumpCount;
                _currentMovement.y = _jumpForce * _doubleJumpMultiplier;
                _playerInputHandler.ConsumeJump(); // Reset JumpTriggered
            }
            _currentMovement.y -= _gravity * Time.deltaTime;
        }
    }

    private void ConsumeDashWrapper()
    {
        _playerInputHandler.ConsumeDash();
    }

    private void HandleDashing()
    {
        if (_playerInputHandler.DashTriggered)
        {
            Vector3 inputDirection = new Vector3(_playerInputHandler.WalkInput.x, 0f, _playerInputHandler.WalkInput.y);
            Vector3 worldDirection = transform.TransformDirection(inputDirection);
            worldDirection.Normalize();
            _currentMovement.x = worldDirection.x * _dashSpeed;
            _currentMovement.z = worldDirection.z * _dashSpeed;
            Invoke("ConsumeDashWrapper", 0.1f);
        }
    }

    private void HandleRotation()
    {
        float mouseXRotation = _playerInputHandler.LookInput.x * _mouseSensitivity;
        transform.Rotate(0f, mouseXRotation, 0f);

        _verticalRotation -= _playerInputHandler.LookInput.y * _mouseSensitivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -_updownRange, _updownRange);
        _mainCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
    }

}
