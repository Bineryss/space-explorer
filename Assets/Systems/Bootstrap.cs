using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Cinemachine;
using UnityEngine;

public class Bootstrap : SerializedMonoBehaviour
{
    [OdinSerialize] private Dictionary<ControlType, GameObject> shipControls;
    [SerializeField] private CinemachineCamera virtualCamera;

    private GameObject currentPlayer;

    [Title("Debug Controls")]
    [SerializeField] private ControlType type;
    [Button("Switch Controls")]
    public void Initialize()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        currentPlayer = Instantiate(shipControls[type]);
        currentPlayer.transform.position = Vector3.zero;

        virtualCamera.Follow = currentPlayer.transform;
    }

    void Start()
    {
        Initialize();
    }
}

public enum ControlType
{
    Arcade = 0,
    Physics = 1,
    ArcadeTower = 2,
}
