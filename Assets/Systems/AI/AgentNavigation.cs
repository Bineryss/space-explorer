using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SimpleNavMeshAgentMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Movement tuning")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float arriveRadius = 150f;
    [SerializeField] private float turnSpeed = 3f;
    [SerializeField] private float arriveDist = 50f;

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(
         transform.position,
           agent.nextPosition,
        speed * Time.deltaTime);

        if(lineRenderer != null)
        {
            lineRenderer.positionCount = agent.path.corners.Length;
            lineRenderer.SetPositions(agent.path.corners.Select(el => new Vector3(el.x, 0, el.z)).ToArray());
        }

        if (agent.pathPending || agent.path.corners.Length == 1)
            return;
        Rotate(agent.path.corners);
    }
    private void Rotate(Vector3[] waypoints)
    {
        if (waypoints.Length == 0) return;

        Vector3 targetPos = waypoints[1];

        Vector3 lookTarget = waypoints[1];
        if (waypoints.Length > 3 && Vector3.Distance(new(transform.position.x, -100, transform.position.z), targetPos) < arriveDist)
        {
            lookTarget = waypoints[2];
        }

        Vector3 toTarget = lookTarget - transform.position;
        toTarget.y = 0;
        Quaternion desiredRot = Quaternion.LookRotation(
            toTarget);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRot,
            turnSpeed * Time.deltaTime);
    }

}
