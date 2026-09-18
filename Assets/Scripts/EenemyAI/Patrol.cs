using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] private ObstacleMovement movement;
    [SerializeField] private Transform[] patrolPoints;

    private int currentPoint;
    private bool isPatrolling = true;

    private void Start()
    {
        if (patrolPoints.Length == 0)
        {
            return;
        }

        MoveToNextPoint();
    }

    private void Update()
    {
        if (!isPatrolling)
        {
            return;
        }

        if (movement.HasReachedDestination())
        {
            MoveToNextPoint();
        }
    }

    public void StartPatrol()
    {
        if (isPatrolling)
        {
            return;
        }

        isPatrolling = true;

        MoveToNextPoint();
    }

    public void StopPatrol()
    {
        isPatrolling = false;
    }

    private void MoveToNextPoint()
    {
        if (patrolPoints.Length == 0)
        {
            return;
        }

        movement.MoveTo(
            patrolPoints[currentPoint].position
        );

        currentPoint++;

        if (currentPoint >= patrolPoints.Length)
        {
            currentPoint = 0;
        }
    }
}