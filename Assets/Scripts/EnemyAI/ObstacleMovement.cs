
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ObstacleMovement : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private float movementThreshold = 0.1f;

    [Header("Debugging")]
    [SerializeField] private bool showDebugMessages = true;

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

    private void Start()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                gameObject.name + " is NOT on the NavMesh!"
            );
            return;
        }

        Debug.Log(
            gameObject.name + " is correctly on the NavMesh!"
        );

        Debug.Log(
            gameObject.name + " NavMeshAgent Speed: " + agent.speed
        );

        if (agent.isStopped)
        {
            Debug.LogWarning(
                gameObject.name + " has a STOPPED NavMeshAgent!"
            );
        }

        if (agent.speed <= 0f)
        {
            Debug.LogWarning(
                gameObject.name + " has a NavMeshAgent speed of 0!"
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
            animator.SetBool("IsWalking", false);
            return;
        }

        bool isWalking =
            agent.velocity.sqrMagnitude >
            movementThreshold * movementThreshold;

        animator.SetBool("IsWalking", isWalking);
    }

    public void MoveTo(Vector3 target)
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning(
                gameObject.name +
                " cannot move: Not on NavMesh!"
            );
            return;
        }

        if (agent.isStopped)
        {
            if (showDebugMessages)
            {
                Debug.LogWarning(
                    gameObject.name +
                    " was stopped! Resuming movement."
                );
            }

            agent.isStopped = false;
        }

        bool pathRequested = agent.SetDestination(target);

        if (!pathRequested)
        {
            Debug.LogWarning(
                gameObject.name +
                " failed to request a path!"
            );
            return;
        }

        if (showDebugMessages)
        {
            Debug.Log(
                gameObject.name +
                " moving towards: " + target
            );
        }
    }

    public void Stop()
    {
        if (!agent.isOnNavMesh)
        {
            return;
        }

        agent.ResetPath();

        if (showDebugMessages)
        {
            Debug.Log(
                gameObject.name + " stopped moving."
            );
        }
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
