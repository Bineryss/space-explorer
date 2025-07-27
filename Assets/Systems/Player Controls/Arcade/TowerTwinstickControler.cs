using UnityEngine;
using UnityEngine.InputSystem;

public class TowerTwinstickControler : MonoBehaviour, ISpaceShipControler
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float deceleration = 15f;

    [Header("Gamepad Settings")]
    [SerializeField] private float gamepadDeadzone = 0.1f;
    [SerializeField] private float gamepadRotationSmoothing = 1500f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 500f;

    [Header("Mouse Settings")]
    [SerializeField] private Camera playerCamera;

    private Vector2 movementInput;
    private Vector2 aimInput;

    private TwinstickSpaceshipActions inputActions;
    private Rigidbody rb;
    private bool isGamepad;
    private Vector2 mouseScreenPosition;


    public Vector2 MovementInput => movementInput;
    public Vector2 LookInput => aimInput;

    public Transform Transform => transform;
    [SerializeField] private GameObject tower;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new TwinstickSpaceshipActions();
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void OnEnable()
    {
        inputActions.ship.Enable();
    }

    void OnDisable()
    {
        inputActions.ship.Disable();
    }

    void OnDestroy()
    {
        inputActions?.Dispose();
    }

    void FixedUpdate()
    {
        MoveShip();
        RotateTower();
    }

    private void MoveShip()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        if (moveDirection.magnitude > gamepadDeadzone)
        {
            // rb.AddForce(moveDirection * acceleration, ForceMode.VelocityChange);
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, moveDirection * moveSpeed, acceleration * Time.fixedDeltaTime);

            Quaternion baseTargetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, baseTargetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }
    }

    private void RotateTower()
    {
        if (isGamepad)
        {
            RotateWithGamepad();
        }
        else
        {
            RotateTowardsMouse();
        }
    }

    private void RotateWithGamepad()
    {
        if (aimInput.magnitude > gamepadDeadzone)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(aimInput.x, 0, aimInput.y));
            tower.transform.rotation = Quaternion.Slerp(tower.transform.rotation, targetRotation, gamepadRotationSmoothing / rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void RotateTowardsMouse()
    {
        // Convert mouse screen position to world position
        Vector3 mouseWorldPos = ScreenToWorldPosition(mouseScreenPosition);

        // Calculate direction from player to mouse position
        Vector3 directionToMouse = (mouseWorldPos - transform.position).normalized;
        directionToMouse.y = 0; // Keep it on the horizontal plane

        if (directionToMouse.magnitude > gamepadDeadzone)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);
            tower.transform.rotation = Quaternion.Slerp(tower.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private Vector3 ScreenToWorldPosition(Vector2 screenPosition)
    {
        // Create a ray from camera through mouse position
        Ray ray = playerCamera.ScreenPointToRay(screenPosition);

        // Create a plane at the player's Y position
        Plane groundPlane = new(Vector3.up, transform.position.y);

        // Find where the ray intersects the plane
        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            movementInput = context.ReadValue<Vector2>();
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            aimInput = context.ReadValue<Vector2>();
            isGamepad = context.control.device is Gamepad;

            if (!isGamepad)
            {
                mouseScreenPosition = aimInput;
            }
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Handle dodge action
            Debug.Log("Dodge action performed");
        }
    }

    public void OnControlsChanged(PlayerInput input)
    {
        if (inputActions == null) return;
        isGamepad = Equals(input.currentControlScheme, inputActions.GamepadScheme.name);
    }
}
