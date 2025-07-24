using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class TwinStickMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float mass = 1f;
    [SerializeField] private float controllerDeadzone = 0.1f;
    [SerializeField] private float gamepadRotationSmoothing = 1000f;
    [SerializeField] private bool isGamePad;

    private CharacterController controller;
    private Vector2 movement;
    private Vector2 aim;
    private Vector3 velocity;

    private InputSystem_Actions controls;
    private PlayerInput input;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new InputSystem_Actions();
        input = GetComponent<PlayerInput>();
    }

    void Start()
    {
        // controls.Player.Move.performed += (context) =>
        // {
        //     Debug.Log("move player with callback");
        //     Vector2 movement2 = context.ReadValue<Vector2>();
        //     Vector3 move = new Vector3(movement2.x, 0, movement2.y);
        //     controller.Move(move * Time.deltaTime * speed);
        // };
    }

    // public void HandleMovement(InputAction.CallbackContext context)
    // {
    //     Vector2 movement2 = context.ReadValue<Vector2>();
    //     Vector3 move = new Vector3(movement2.x, 0, movement2.y);
    //     controller.Move(move * speed);
    // }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        HandleInput();
        HandleRotation();
        HandleMovement2();
    }

    private void HandleInput()
    {
        movement = controls.Player.Move.ReadValue<Vector2>();
        aim = controls.Player.Look.ReadValue<Vector2>();
        Debug.Log(aim);
    }
    private void HandleMovement2()
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        controller.Move(move * Time.deltaTime * speed);
    }
    private void HandleRotation()
    {
        if (isGamePad)
        {
            GamepadRotation();
        }
    }

    private void GamepadRotation()
    {
        if (Mathf.Abs(aim.x) < controllerDeadzone && Mathf.Abs(aim.y) < controllerDeadzone) return;

        Vector3 playerDirection = Vector3.right * aim.x + Vector3.forward * aim.y;
        if (playerDirection.sqrMagnitude <= 0.0f) return;

        Quaternion newRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, gamepadRotationSmoothing * Time.deltaTime);
    }

    public void OnDeviceChange(PlayerInput input)
    {
        if (controls == null) return;
        isGamePad = Equals(controls.GamepadScheme.name, input.currentControlScheme);
    }
}
