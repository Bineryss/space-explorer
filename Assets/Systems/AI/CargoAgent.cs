using System.Collections.Generic;
using AI.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;
using static AI.BehaviourTree.IStrategy;

public class CargoAgent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform pickup;
    [SerializeField] private Transform dropoff;
    [SerializeField] private int cargoVolume = 0;

    private BehaviourTree tree;

    void Awake()
    {
        tree = new BehaviourTree("Cargo Ship");

        Sequence collectResources = new("Collect Resources");
        collectResources.AddChild(new Leaf("Check Target exists", new Condition(() => pickup != null)));
        collectResources.AddChild(new Leaf("Move to Pickup", new PatrolStrategy(agent, new List<Transform> { pickup })));
        collectResources.AddChild(new Leaf("Collect Cargo", new ActionStrategy(() => cargoVolume += 10)));

        Sequence dropOffCargo = new("Drop Off Cargo", 10);
        dropOffCargo.AddChild(new Leaf("Check Target exists", new Condition(() => dropoff != null)));
        dropOffCargo.AddChild(new Leaf("Move to Dropoff", new PatrolStrategy(agent, new List<Transform> { dropoff })));
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
}