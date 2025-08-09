using System.Collections.Generic;
using AI.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;
using static AI.BehaviourTree.IStrategy;

public class CargoAgent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject pickup;
    [SerializeField] private GameObject dropoff;
    [SerializeField] private int cargoVolume = 0;

    private BehaviourTree tree;

    void Awake()
    {
        tree = new BehaviourTree("Cargo Ship");
        Debug.Log($"CargoAgent Awake - {pickup.activeSelf} - {dropoff.activeSelf}");

        Sequence collectResources = new("Collect Resources",20);
        collectResources.AddChild(new Leaf("Check Pickup exists", new Condition(() => pickup.activeSelf)));
        collectResources.AddChild(new Leaf("Move to Pickup", new MoveToTarget(agent, pickup.transform )));
        collectResources.AddChild(new Leaf("Collect Cargo", new ActionStrategy(() => cargoVolume += 10)));

        Sequence dropOffCargo = new("Drop Off Cargo", 10);
        dropOffCargo.AddChild(new Leaf("Check Dropoff exists", new Condition(() => dropoff.activeSelf)));
        dropOffCargo.AddChild(new Leaf("Move to Dropoff", new MoveToTarget(agent, dropoff.transform)));
        dropOffCargo.AddChild(new Leaf("Deliver Cargo", new ActionStrategy(() => cargoVolume -= 10)));

        PrioritySelector collectAndDeliver = new("Collect or Deliver");
        collectAndDeliver.AddChild(collectResources);
        collectAndDeliver.AddChild(dropOffCargo);
        tree.AddChild(collectAndDeliver);
    }

    void Update()
    {
        tree.Process();
    }
    [ContextMenu("Manual Update")]
    void ManualUpdate()
    {
        tree.Process();
    }
}