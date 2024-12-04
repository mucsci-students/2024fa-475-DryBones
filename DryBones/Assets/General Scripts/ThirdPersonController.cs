using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{

    [Header("Movement Speeds")]
    [SerializeField] private float _walkSpeed = 10f;
    [SerializeField] private float _runSpeedMultiplier = 2f;
    
    [Header("Jump Parameters")]
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _gravity = 9.81f;

    [Header("Look Sensitivity")]
    [SerializeField] private float _mouseSensitivity = 2f;
    [SerializeField] private float _updownRange = 90f;
    [SerializeField] private bool smooth;
    [SerializeField] private float smoothTime = 5f;

    [Header("Player Input Handler And Camera")]
    [SerializeField] private PlayerInputHandler _playerInputHandler;
    [SerializeField] private Camera _mainCamera;

    private CharacterController _characterController;
    private Vector3 _currentMovement;
    private float _verticalRotation;

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
        _characterController.Move(_currentMovement * Time.deltaTime);
    }

    private void HandleJumping()
    {
        if (_characterController.isGrounded)
        {
            _currentMovement.y = -0.5f;

            if (_playerInputHandler.JumpTriggered)
            {
                _currentMovement.y = _jumpForce;
            }
        }
        else
        {
            _currentMovement.y -= _gravity * Time.deltaTime;
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
