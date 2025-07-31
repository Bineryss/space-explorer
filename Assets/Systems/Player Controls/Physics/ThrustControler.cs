using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ImprovedThrustController : MonoBehaviour, ISpaceShipControler
{
    [Header("Ship Physics Settings")]
    [SerializeField] private float rotationTorque = 1500f;
    [SerializeField] private float maxAngularVelocity = 360f; // Degrees per second
    [SerializeField] private float thrustForce = 2000f;
    [SerializeField] private float maxSpeed = 25f;

    [Header("Damping & Feel")]
    [SerializeField] private float linearDrag = 2f;
    [SerializeField] private float angularDrag = 8f;
    [SerializeField] private float rotationDeadzone = 0.1f;

    [Header("Tower Control")]
    [SerializeField] private GameObject tower;
    [SerializeField] private float towerRotationSpeed = 720f; // Degrees per second
    [SerializeField] private float towerSmoothing = 15f;

    [Header("Input Settings")]
    [SerializeField] private float gamepadDeadzone = 0.1f;
    [SerializeField] private Camera playerCamera;

    [Header("Debug")]
    [SerializeField] private Vector2 rotationInput;
    [SerializeField] private float thrustInput;
    [SerializeField] private Vector2 aimInput;
    [SerializeField] private bool isGamepad;

    [Header("Collision Avoidance")]
    [SerializeField] private float repulsionForce = 1500f;
    [SerializeField] private float repulsionRadius = 2f;
    [SerializeField] private LayerMask avoidanceLayers = -1;
    [SerializeField] private bool enableCollisionAvoidance = true;

    private PhysicsSpaceshipActions inputActions;
    private Rigidbody rb;
    private Vector2 mouseScreenPosition;
    [SerializeField] private PlayerInput playerInput;

    public Vector2 MovementInput => rotationInput;
    public Vector2 LookInput => aimInput;
    public Transform Transform => transform;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PhysicsSpaceshipActions();

        if (playerCamera == null)
            playerCamera = Camera.main;

        SetupPhysics();
    }

    void SetupPhysics()
    {
        // Configure rigidbody for responsive spaceship feel
        rb.linearDamping = linearDrag;
        rb.angularDamping = angularDrag;

        // Prevent physics from affecting Y axis (keep it top-down)
        // rb.constraints = RigidbodyConstraints.FreezePositionY | 
        //                 RigidbodyConstraints.FreezeRotationX | 
        //                 RigidbodyConstraints.FreezeRotationZ;

        // Set max angular velocity for snappy rotation
        rb.maxAngularVelocity = maxAngularVelocity * Mathf.Deg2Rad;
    }

    void OnEnable()
    {
        playerInput.actions.Enable();
        inputActions.ship.Enable();
    }

    void FixedUpdate()
    {
        ApplyRotationTorque();
        ApplyThrust();
        LimitSpeed();
        RotateTower();
    }

    private void ApplyRotationTorque()
    {
        if (rotationInput.magnitude > rotationDeadzone)
        {
            // Calculate rotation direction for top-down view
            // For WASD: W=forward, S=backward, A=rotate left, D=rotate right
            float rotationDirection = rotationInput.x; // A/D input

            // Apply torque for smooth but responsive rotation
            Vector3 torque = Vector3.up * rotationDirection * rotationTorque;
            rb.AddTorque(torque, ForceMode.Force);
        }
    }

    private void ApplyThrust()
    {
        if (Mathf.Abs(thrustInput) > 0.01f)
        {
            // Apply thrust in the direction the ship is facing
            Vector3 thrustDirection = transform.forward * thrustInput;
            rb.AddForce(thrustDirection * thrustForce, ForceMode.Force);
        }
    }

    private void LimitSpeed()
    {
        // Clamp velocity to max speed for snappy feel
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    // Tower rotation remains similar but with improvements
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
            Vector3 aimDirection = new Vector3(aimInput.x, 0, aimInput.y).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(aimDirection);

            float rotSpeed = towerRotationSpeed * Mathf.Deg2Rad;
            tower.transform.rotation = Quaternion.RotateTowards(
                tower.transform.rotation,
                targetRotation,
                rotSpeed * Time.fixedDeltaTime
            );
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mouseWorldPos = ScreenToWorldPosition(mouseScreenPosition);
        Vector3 directionToMouse = (mouseWorldPos - tower.transform.position).normalized;
        directionToMouse.y = 0;

        if (directionToMouse.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);

            float rotSpeed = towerRotationSpeed * Mathf.Deg2Rad;
            tower.transform.rotation = Quaternion.RotateTowards(
                tower.transform.rotation,
                targetRotation,
                rotSpeed * Time.fixedDeltaTime
            );
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            rotationInput = context.ReadValue<Vector2>();
        }
    }

    public void OnThrust(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            thrustInput = context.ReadValue<float>();
        }
    }

    public void OnAim(InputAction.CallbackContext context)
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

    private Vector3 ScreenToWorldPosition(Vector2 screenPosition)
    {
        Ray ray = playerCamera.ScreenPointToRay(screenPosition);
        Plane groundPlane = new(Vector3.up, transform.position.y);

        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }

    public void OnControlsChanged(PlayerInput input)
    {
        if (inputActions == null) return;
        isGamepad = Equals(input.currentControlScheme, inputActions.GamepadScheme.name);
    }
    
     void OnCollisionEnter(Collision collision)
    {
        if (!enableCollisionAvoidance) return;
        
        // Check if we should avoid this object
        if (ShouldAvoidObject(collision.collider))
        {
            ApplyRepulsionForce(collision);
        }
    }
    
    void OnCollisionStay(Collision collision)
    {
        if (!enableCollisionAvoidance) return;
        
        if (ShouldAvoidObject(collision.collider))
        {
            // Apply continuous repulsion while touching
            ApplyRepulsionForce(collision, 0.3f); // Reduced intensity for continuous contact
        }
    }
    
    private bool ShouldAvoidObject(Collider other)
    {
        // Check layer mask
        return (avoidanceLayers.value & (1 << other.gameObject.layer)) != 0;
    }
    
    private void ApplyRepulsionForce(Collision collision, float intensityMultiplier = 1f)
    {
        // Calculate repulsion direction from collision point
        Vector3 repulsionDirection = Vector3.zero;
        
        foreach (ContactPoint contact in collision.contacts)
        {
            // Direction away from collision point
            Vector3 awayFromContact = (transform.position - contact.point).normalized;
            repulsionDirection += awayFromContact;
        }
        
        repulsionDirection = repulsionDirection.normalized;
        
        // Keep repulsion on horizontal plane (for top-down game)
        repulsionDirection.y = 0;
        
        // Apply the repulsion force
        float finalForce = repulsionForce * intensityMultiplier;
        rb.AddForce(repulsionDirection * finalForce, ForceMode.Force);
        
        Debug.Log($"Applied repulsion force: {finalForce} in direction: {repulsionDirection}");
    }
}
