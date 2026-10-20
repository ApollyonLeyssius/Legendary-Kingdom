using UnityEngine;
using UnityEngine.AI;

public class ObstacleMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 target)
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }
        agent.SetDestination(target);
    }

    public void Stop()
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }

        agent.ResetPath();
    }

    public bool HasReachedDestination()
    {
        if (agent.pathPending)
        {
            return false;
        }
        return agent.remainingDistance <= agent.stoppingDistance;
    }
}
