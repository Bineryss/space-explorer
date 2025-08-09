using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI.BehaviourTree
{
    public interface IStrategy
    {
        Node.Status Process();
        void Reset()
        {
            //NOOP
        }

        public class PatrolStrategy : IStrategy
        {
            private readonly NavMeshAgent agent;
            private readonly List<Transform> waypoints;
            private int currentWaypointIndex;
            private bool isPathCalculated;

            public PatrolStrategy(NavMeshAgent agent, List<Transform> waypoints)
            {
                this.agent = agent;
                this.waypoints = waypoints;
                currentWaypointIndex = 0;
            }

            public Node.Status Process()
            {
                if (currentWaypointIndex == waypoints.Count) return Node.Status.Success;

                Transform target = waypoints[currentWaypointIndex];
                Vector3 agentTarget = target.position;
                agentTarget.y = -100;
                Debug.Log($"Setting destination for {target.name} - {agentTarget}");
                agent.SetDestination(agentTarget);

                if (isPathCalculated && agent.remainingDistance < 0.1f)
                {
                    currentWaypointIndex++;
                    isPathCalculated = false;
                }

                if (agent.pathPending)
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
        public class MoveToTarget : IStrategy
        {
            private readonly NavMeshAgent agent;
            private readonly Transform target;
            private bool isPathCalculated;

            public MoveToTarget(NavMeshAgent agent, Transform target)
            {
                this.agent = agent;
                this.target = target;
            }

            public Node.Status Process()
            {
                if (isPathCalculated && agent.remainingDistance < 0.1f)
                {
                    isPathCalculated = false;
                    return Node.Status.Success;
                }

                Vector3 agentTarget = target.position;
                agentTarget.y = -100;
                agent.SetDestination(agentTarget);

                if (agent.pathPending)
                {
                    isPathCalculated = true;
                }

                return Node.Status.Running;
            }

            public void Reset()
            {
                isPathCalculated = false;
            }
        }
        public class Condition : IStrategy
        {
            private readonly Func<bool> predicate;

            public Condition(Func<bool> predicate)
            {
                this.predicate = predicate;
            }

            public Node.Status Process()
            {
                return predicate() ? Node.Status.Success : Node.Status.Failure;
            }
        }

        public class ActionStrategy : IStrategy
        {
            private readonly Action action;

            public ActionStrategy(Action action)
            {
                this.action = action;
            }

            public Node.Status Process()
            {
                action();
                return Node.Status.Success;
            }
        }
    }
}