using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI.BehaviourTree
{
    public interface IStrategy
    {
        Node.Status Process();
        void Reset();
    }

    public class PatrolStrategy : IStrategy
    {
        private readonly Transform entity;
        private readonly NavMeshAgent agent;
        private readonly List<Transform> waypoints;
        private readonly float speed;
        private int currentWaypointIndex;
        private bool isPathCalculated;

        public PatrolStrategy(Transform entity, NavMeshAgent agent, List<Transform> waypoints, float speed)
        {
            this.entity = entity;
            this.agent = agent;
            this.waypoints = waypoints;
            this.speed = speed;
            currentWaypointIndex = 0;
        }

        public Node.Status Process()
        {
            if (currentWaypointIndex == waypoints.Count) return Node.Status.Success;

            Transform target = waypoints[currentWaypointIndex];
            Vector3 agentTarget = target.position;
            agentTarget.y = -100;
            agent.SetDestination(agentTarget);

            if (isPathCalculated && agent.remainingDistance < 0.1f)
            {
                currentWaypointIndex++;
                isPathCalculated = false;
            }

            if(agent.pathPending)
            {
                isPathCalculated = true;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            currentWaypointIndex = 0;
            agent.ResetPath();
        }
    }
}