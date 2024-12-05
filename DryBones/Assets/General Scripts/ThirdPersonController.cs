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

    private Vector3 _currentMovement;

    [Header("Wall Running")]
    [SerializeField] Transform _playerCamera;
    [SerializeField] Transform _orientation;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] float _wallRunForce = 10f;
    [SerializeField] float _maxWallRuntime = 5f;

    private bool _isRunningOnWall = false;
    private bool _isWallLeft = false;
    private bool _isWallRight = false;

    [Header("Jump Parameters")]
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _gravity = 9.81f;
    [SerializeField] private float _doubleJumpMultiplier = 1.5f;
    [SerializeField] private int _maxJump = 3;

    public static bool _isDoubleJump = true;
    private int _jumpCount = 0;

    [Header("Dash Parameters")]
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private float _dashSpeed = 15f;
    [SerializeField] private float _dashDuration = 0.5f;
    [SerializeField] private float _dashCooldown = 1f;

    private bool _canDash = true;

    [Header("Look Sensitivity")]
    [SerializeField] private float _mouseSensitivity = 2f;
    [SerializeField] private float _updownRange = 90f;

    private float _verticalRotation;

    [Header("Player Input Handler And Camera")]
    [SerializeField] private PlayerInputHandler _playerInputHandler;
    [SerializeField] private Camera _mainCamera;

    private CharacterController _characterController;
    
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
        CheckForWall();
        WallRunInput();
        HandleJumping();
        if (_playerInputHandler.DashTriggered && _canDash)
        {
            StartCoroutine(HandleDashing());
        }
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

            if(!_isRunningOnWall)
            {
                _currentMovement.y -= _gravity * Time.deltaTime;
            }
        }
    }

    private IEnumerator HandleDashing()
    {
        Vector3 inputDirection = new Vector3(_playerInputHandler.WalkInput.x, 0f, _playerInputHandler.WalkInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        worldDirection.Normalize();
        _currentMovement.x = worldDirection.x * _dashSpeed;
        _currentMovement.z = worldDirection.z * _dashSpeed;
        _trailRenderer.emitting = true;
        yield return new WaitForSeconds(_dashDuration);
        _playerInputHandler.ConsumeDash();
        _trailRenderer.emitting = false;
        _canDash = false;
        Debug.Log("DASHING");
        // Check for input during cooldown
        // Cooldown check
        float cooldownTimer = 0f;
        while (cooldownTimer < _dashCooldown)
        {
            cooldownTimer += Time.deltaTime;

            // Check if dash is pressed during cooldown
            if (_playerInputHandler.DashTriggered)
            {
                Debug.Log("Dash pressed during cooldown!");
                _playerInputHandler.ConsumeDash(); // Reset the dash trigger
            }

            yield return null; // Wait for the next frame
        }
        //yield return new WaitForSeconds(_dashCooldown);
        _canDash = true;
    }

    private void HandleRotation()
    {
        float mouseXRotation = _playerInputHandler.LookInput.x * _mouseSensitivity;
        transform.Rotate(0f, mouseXRotation, 0f);

        _verticalRotation -= _playerInputHandler.LookInput.y * _mouseSensitivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -_updownRange, _updownRange);
        _mainCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
    }

    /* Wall Running */

    private void WallRunInput()
    {
        if (_isWallRight && _playerInputHandler.WalkInput.x > 0)
        {
            StartWallRun();
        }
        else if (_isWallLeft && _playerInputHandler.WalkInput.x < 0)
        {
            StartWallRun();
        }
        else
        {
            StopWallRun();
        }
    }

    private void StartWallRun()
    {
        Debug.Log("RUNNING ON THE WALL!");
        _isRunningOnWall = true;

        // Cancel vertical gravity
        _gravity = 0;
        _currentMovement.y = 0f;

        // Add forward momentum
        _currentMovement += _orientation.forward * _wallRunForce * Time.deltaTime;
    }

    private void StopWallRun()
    {
        Debug.Log("STOP RUNNING ON THE WALL!");
        _isRunningOnWall = false;
        _gravity = 9.81f;
        // Allow gravity to act again
        _currentMovement.y -= _gravity * Time.deltaTime;
    }

    private void CheckForWall()
    {
        _isWallRight = Physics.Raycast(transform.position, _orientation.right, 1f, _wallLayer);
        _isWallLeft = Physics.Raycast(transform.position, -_orientation.right, 1f, _wallLayer);

        Debug.DrawRay(transform.position, _orientation.right * 1f, Color.red);   // Right raycast
        Debug.DrawRay(transform.position, -_orientation.right * 1f, Color.blue); // Left raycast

        if (!_isWallRight && !_isWallLeft)
        {
            StopWallRun();
        }

        // Reset jump count when touching a wall
        if (_isWallRight || _isWallLeft)
        {
            _jumpCount = 0;
        }
    }

}
