using UnityEngine;

public class SpaceshipVisualBanking : MonoBehaviour
{
    [Header("Banking Settings")]
    [SerializeField] private float bankingAngle = 45f;
    [SerializeField] private float bankingSpeed = 5f;
    [SerializeField] private float angularVelocityScale = 0.5f; // Much lower scale
    [SerializeField] private float directionSmoothTime = 0.1f;
    [SerializeField] private float maxAngularVelocity = 180f; // Degrees per second
    [SerializeField] private float angularVelocityDeadzone = 10f; // Ignore small angular velocities
    
    private float currentBankingAngle = 0f;
    private Vector2 smoothedDirection;
    private Vector2 directionVelocity;
    private float previousAngle;
    private ISpaceShipControler playerController;
    
    void Start()
    {
        playerController = GetComponentInParent<ISpaceShipControler>();
    }
    
    void Update()
    {
        ApplyProportionalAngularBanking();
    }
    
    private void ApplyProportionalAngularBanking()
    {
        Vector2 movementInput = playerController.MovementInput;
        
        if (movementInput.magnitude > 0.1f)
        {
            Vector2 targetDirection = movementInput.normalized;
            
            smoothedDirection = Vector2.SmoothDamp(smoothedDirection, targetDirection, 
                                                 ref directionVelocity, directionSmoothTime);
            
            float currentAngle = Mathf.Atan2(smoothedDirection.y, smoothedDirection.x) * Mathf.Rad2Deg;
            float angularVelocity = Mathf.DeltaAngle(previousAngle, currentAngle) / Time.deltaTime;
            
            // Apply deadzone and normalize angular velocity
            if (Mathf.Abs(angularVelocity) > angularVelocityDeadzone)
            {
                float normalizedAngularVelocity = Mathf.Clamp(angularVelocity / maxAngularVelocity, -1f, 1f);
                float targetBankingAngle = -normalizedAngularVelocity * bankingAngle * angularVelocityScale;
                
                currentBankingAngle = Mathf.Lerp(currentBankingAngle, targetBankingAngle, bankingSpeed * Time.deltaTime);
            }
            else
            {
                // Gradually level out for small angular velocities
                currentBankingAngle = Mathf.Lerp(currentBankingAngle, 0f, bankingSpeed * Time.deltaTime * 0.5f);
            }
            
            previousAngle = currentAngle;
        }
        else
        {
            currentBankingAngle = Mathf.Lerp(currentBankingAngle, 0f, bankingSpeed * Time.deltaTime);
            smoothedDirection = Vector2.zero;
        }
        
        Vector3 currentRotation = transform.localEulerAngles;
        transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentBankingAngle);
    }
}
