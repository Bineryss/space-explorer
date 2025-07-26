using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TwinstickControler : MonoBehaviour, ISpaceShipControler
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 10f;

    [SerializeField] private float controllerDeadzone = 0.1f;
    [SerializeField] private float gamepadRotationSmoothing = 1000f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private bool useSlerpRotation = true;

    [Header("Mouse Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseRotationSpeed = 180f;

    [SerializeField] private Vector2 movementInput;
    [SerializeField] private Vector2 aimInput;


    private SpaceshipActions inputActions;
    private Rigidbody rb;
    private bool isGamepad;
    private Vector2 mouseScreenPosition;


    public Vector2 MovementInput => movementInput;
    public Vector2 LookInput => aimInput;

    public Transform Transform => transform;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new SpaceshipActions();
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
        RotateShip();
    }

    private void MoveShip()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        if (moveDirection.magnitude > 0.1f)
        {
            // rb.AddForce(moveDirection * acceleration, ForceMode.VelocityChange);
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, moveDirection * moveSpeed, acceleration * Time.fixedDeltaTime);

            //ship rotation twoard movement direction
            Quaternion baseTargetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, baseTargetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }
    }

    private void RotateShip()
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
        if (aimInput.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(aimInput.x, 0, aimInput.y));
            if (useSlerpRotation)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
            else
            {
                rb.MoveRotation(targetRotation);
            }
        }
    }

    private void RotateTowardsMouse()
    {
        // Convert mouse screen position to world position
        Vector3 mouseWorldPos = ScreenToWorldPosition(mouseScreenPosition);

        // Calculate direction from player to mouse position
        Vector3 directionToMouse = (mouseWorldPos - transform.position).normalized;
        directionToMouse.y = 0; // Keep it on the horizontal plane

        if (directionToMouse.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, mouseRotationSpeed * Time.fixedDeltaTime);
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
}
