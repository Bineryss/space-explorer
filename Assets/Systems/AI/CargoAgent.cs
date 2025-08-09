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


        //check cargo load
        // if full, go to dropoff
        // check exists
        // pathfind
        // unload until empty
        // if not full, go to pickup
        // check if exists
        // pathfind
        // load until full

        Sequence collectCargo = new("Collect Cargo", 10);
        collectCargo.AddChild(new Leaf("Check Cargo Bay empty enough", new Condition(() => cargoVolume <= 50)));
        collectCargo.AddChild(new Leaf("Check Pickup point exists", new Condition(() => pickup.activeSelf)));
        collectCargo.AddChild(new Leaf("Move to Pickup", new MoveToTarget(agent, pickup.transform)));
        UntilSuccess harvest = new("Harvest Cargo");
        harvest.AddChild(new Leaf("Check if in range of pickup", new Condition(
            () =>
            {
                bool inDistance = Vector3.Distance(agent.transform.position, pickup.transform.position) < 50;
                if (!inDistance)
                {
                    collectCargo.Reset();
                }
                return inDistance;
            })));
        harvest.AddChild(new Leaf("Collect Cargo", new TimedActionStrategy(() => cargoVolume += 10, 2f)));
        harvest.AddChild(new Leaf("Check Cargo Bay full", new Condition(() => cargoVolume >= 100)));
        collectCargo.AddChild(harvest);

        Sequence deliverCargo = new("Deliver Cargo", 20);
        deliverCargo.AddChild(new Leaf("Check Cargo Bay full enough", new Condition(() => cargoVolume > 50)));
        deliverCargo.AddChild(new Leaf("Check Dropoff point exists", new Condition(() => dropoff.activeSelf)));
        deliverCargo.AddChild(new Leaf("Move to Dropoff", new MoveToTarget(agent, dropoff.transform)));
        UntilSuccess unload = new("Unload Cargo");
        unload.AddChild(new Leaf("Check if in range of dropoff", new Condition(
            () =>
            {
                bool inDistance = Vector3.Distance(agent.transform.position, dropoff.transform.position) < 50;
                if (!inDistance)
                {
                    deliverCargo.Reset();
                }
                return inDistance;
            })));
        unload.AddChild(new Leaf("Deliver Cargo", new TimedActionStrategy(() => cargoVolume -= 10, 2f)));
        unload.AddChild(new Leaf("Check Cargo Bay empty", new Condition(() => cargoVolume <= 0)));
        deliverCargo.AddChild(unload);


        PrioritySelector prioritySelector = new("Action Selector");
        prioritySelector.AddChild(collectCargo);
        prioritySelector.AddChild(deliverCargo);
        tree.AddChild(prioritySelector);

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