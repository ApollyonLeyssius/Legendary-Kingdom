
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ObstacleMovement : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float movementThreshold = 0.1f;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogWarning(
                gameObject.name + ": Animator is missing!"
            );
        }
    }

    private void Update()
    {
        UpdateMovementAnimation();
    }

    private void UpdateMovementAnimation()
    {
        if (animator == null)
        {
            return;
        }

        if (!agent.isOnNavMesh)
        {
            animator.speed = 0f;
            return;
        }

        bool isWalking =
            agent.velocity.sqrMagnitude >
            movementThreshold * movementThreshold;

        animator.speed = isWalking ? 1f : 0f;
    }

    public void MoveTo(Vector3 target)
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }

        if (agent.isStopped)
        {
            agent.isStopped = false;
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
        if (!agent.isOnNavMesh || agent.pathPending)
        {
            return false;
        }

        return agent.remainingDistance <= agent.stoppingDistance;
    }
}
