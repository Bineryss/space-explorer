using System.Collections.Generic;
using AI.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

public class CargoAgent : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private List<Transform> waypoints = new();

    private BehaviourTree tree;

    void Awake()
    {
        tree = new BehaviourTree("Cargo Ship");
        tree.AddChild(new Leaf("Patrol", new PatrolStrategy(transform, agent, waypoints, 5f)));
    }

    void Update()
    {
        tree.Process();
    }
}