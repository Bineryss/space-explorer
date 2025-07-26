using UnityEngine;

public class SpaceshipVisualBanking : MonoBehaviour
{
    [Header("Banking Settings")]
    [SerializeField] private float bankingAngle = 45f;
    [SerializeField] private float bankingSpeed = 5f;
    
    [Header("Direction Difference Banking")]
    [SerializeField] private float directionDifferenceWeight = 0.7f;
    [SerializeField] private float directionDifferenceScale = 1f;
    [SerializeField] private float minAngleDifference = 5f;
    
    [Header("Angular Velocity Banking")]
    [SerializeField] private float angularVelocityWeight = 0.3f;
    [SerializeField] private float angularVelocityScale = 0.1f;
    [SerializeField] private float directionSmoothTime = 0.1f;
    [SerializeField] private float maxAngularVelocity = 90f;
    [SerializeField] private float angularVelocityDeadzone = 10f;
    
    private float currentBankingAngle = 0f;
    private Vector2 smoothedDirection;
    private Vector2 directionVelocity;
    private float previousAngle;
    private ISpaceShipControler playerController;
    private Transform parentTransform;
    
    void Start()
    {
        playerController = GetComponentInParent<ISpaceShipControler>();
        parentTransform = playerController.Transform;
    }
    
    void Update()
    {
        ApplyHybridBanking();
    }
    
    private void ApplyHybridBanking()
    {
        Vector2 movementInput = playerController.MovementInput;
        
        if (movementInput.magnitude > 0.1f)
        {
            // Calculate direction difference banking
            float directionBanking = CalculateDirectionDifferenceBanking(movementInput);
            
            // Calculate angular velocity banking
            float angularBanking = CalculateAngularVelocityBanking(movementInput);
            
            // Combine both banking effects
            float targetBankingAngle = (directionBanking * directionDifferenceWeight) + 
                                     (angularBanking * angularVelocityWeight);
            
            currentBankingAngle = Mathf.Lerp(currentBankingAngle, targetBankingAngle, bankingSpeed * Time.deltaTime);
        }
        else
        {
            currentBankingAngle = Mathf.Lerp(currentBankingAngle, 0f, bankingSpeed * Time.deltaTime);
            smoothedDirection = Vector2.zero;
        }
        
        Vector3 currentRotation = transform.localEulerAngles;
        transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, currentBankingAngle);
    }
    
    private float CalculateDirectionDifferenceBanking(Vector2 movementInput)
    {
        Vector3 movementDirection3D = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        Vector3 parentForward = parentTransform.forward;
        
        float angleDifference = Vector3.SignedAngle(parentForward, movementDirection3D, Vector3.up);
        
        if (Mathf.Abs(angleDifference) > minAngleDifference)
        {
            float normalizedDifference = Mathf.Clamp(angleDifference / 90f, -1f, 1f);
            return normalizedDifference * bankingAngle * directionDifferenceScale;
        }
        
        return 0f;
    }
    
    private float CalculateAngularVelocityBanking(Vector2 movementInput)
    {
        Vector2 targetDirection = movementInput.normalized;
        smoothedDirection = Vector2.SmoothDamp(smoothedDirection, targetDirection, 
                                             ref directionVelocity, directionSmoothTime);
        
        float currentAngle = Mathf.Atan2(smoothedDirection.y, smoothedDirection.x) * Mathf.Rad2Deg;
        float angularVelocity = Mathf.DeltaAngle(previousAngle, currentAngle) / Time.deltaTime;
        
        if (Mathf.Abs(angularVelocity) > angularVelocityDeadzone)
        {
            float normalizedAngularVelocity = Mathf.Clamp(angularVelocity / maxAngularVelocity, -1f, 1f);
            previousAngle = currentAngle;
            return -normalizedAngularVelocity * bankingAngle * angularVelocityScale;
        }
        
        previousAngle = currentAngle;
        return 0f;
    }
}
