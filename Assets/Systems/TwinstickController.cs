using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArcadeSpaceshipController : MonoBehaviour
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

    [Header("Banking Settings")]
    [SerializeField] private float bankingAngle = 15f; // Maximum banking angle in degrees
    [SerializeField] private float bankingSpeed = 5f;   // How fast the ship banks

    private float currentBankingAngle = 0f; // Current banking angle

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
        HandleMovingRotation();
        RotateShip();
    }

    private void MoveShip()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        if (moveDirection.magnitude > 0.1f)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, moveDirection * moveSpeed, acceleration * Time.fixedDeltaTime);
            // rb.AddForce(moveDirection * acceleration, ForceMode.VelocityChange);
        }
        else
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
        }
    }

    private void HandleMovingRotation()
    {
        if (movementInput.magnitude > 0.1f)
        {
            // 1. Calculate base rotation toward movement direction
            Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
            Quaternion baseTargetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            // 2. Smooth banking transition
            float targetBankingAngle = -movementInput.x * bankingAngle;
            currentBankingAngle = Mathf.Lerp(currentBankingAngle, targetBankingAngle, bankingSpeed * Time.fixedDeltaTime);

            // 3. Combine base rotation with banking
            Quaternion bankingRotation = Quaternion.Euler(0, 0, currentBankingAngle);
            Quaternion finalTargetRotation = baseTargetRotation * bankingRotation;

            // 4. Apply smooth rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, finalTargetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Level out banking when not moving
            currentBankingAngle = Mathf.Lerp(currentBankingAngle, 0f, bankingSpeed * Time.fixedDeltaTime);

            if (Mathf.Abs(currentBankingAngle) > 0.1f)
            {
                Vector3 currentEuler = transform.rotation.eulerAngles;
                Quaternion levelRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, currentBankingAngle);
                transform.rotation = Quaternion.Slerp(transform.rotation, levelRotation, bankingSpeed * Time.fixedDeltaTime);
            }
        }
    }

    private void RotateShip()
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
