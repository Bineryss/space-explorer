
using UnityEngine;

public class SpaceshipVisualBanking : MonoBehaviour
{
    [Header("Banking Settings")]
    [SerializeField] private float bankingAngle = 45f;
    [SerializeField] private float bankingSpeed = 5f;
    [SerializeField] private float angularVelocityScale = 100f;
    [SerializeField] private float directionSmoothTime = 0.1f;
    
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
        ApplyAngularVelocityBanking();
    }
    
    private void ApplyAngularVelocityBanking()
    {
        Vector2 movementInput = playerController.MovementInput;
        
        if (movementInput.magnitude > 0.1f)
        {
            Vector2 targetDirection = movementInput.normalized;
            
            // Smooth the direction change
            smoothedDirection = Vector2.SmoothDamp(smoothedDirection, targetDirection, 
                                                 ref directionVelocity, directionSmoothTime);
            
            // Calculate angular velocity
            float currentAngle = Mathf.Atan2(smoothedDirection.y, smoothedDirection.x) * Mathf.Rad2Deg;
            float angularVelocity = Mathf.DeltaAngle(previousAngle, currentAngle) / Time.deltaTime;
            
            // Apply banking based on angular velocity
            float targetBankingAngle = -angularVelocity * angularVelocityScale;
            targetBankingAngle = Mathf.Clamp(targetBankingAngle, -bankingAngle, bankingAngle);
            
            currentBankingAngle = Mathf.Lerp(currentBankingAngle, targetBankingAngle, bankingSpeed * Time.deltaTime);
            
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
