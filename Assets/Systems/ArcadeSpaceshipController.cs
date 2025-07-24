using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArcadeSpaceshipController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 10f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private bool useSlerpRotation = true;


    [SerializeField] private Vector2 movementInput;
    [SerializeField] private Vector2 aimInput;


    private SpaceshipActions inputActions;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new SpaceshipActions();
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
            rb.AddForce(moveDirection * acceleration, ForceMode.Acceleration);
        }
        else
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }
    }

    private void RotateShip()
    {
        if (aimInput.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(aimInput.x, 0, aimInput.y));
            if (useSlerpRotation)
            {
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
            else
            {
                rb.MoveRotation(targetRotation);
            }
        }
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
