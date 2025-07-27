// Method 1: Verify in Player Input component
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDiagnostics : MonoBehaviour
{
    public PlayerInput playerInput;
    
    void Start()
    {
        // Check if the asset is assigned and enabled
        if (playerInput.actions != null)
        {
            Debug.Log($"Input Asset: {playerInput.actions.name}");
            Debug.Log($"Asset Enabled: {playerInput.actions.enabled}");
            
            // Force enable if needed
            if (!playerInput.actions.enabled)
            {
                playerInput.actions.Enable();
                Debug.Log("Input Asset manually enabled");
            }
        }
        else
        {
            Debug.LogError("No Input Asset assigned to Player Input!");
        }
    }
}
