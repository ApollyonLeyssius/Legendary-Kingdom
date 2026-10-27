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
            Debug.LogWarning("No patrol points assigned.");
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

        UpdatePatrol();
    }

    private void UpdatePatrol()
    {
        if (movement.HasReachedDestination())
        {
            MoveToNextPoint();
        }
    }

    private void MoveToNextPoint()
    {
        movement.MoveTo(patrolPoints[currentPoint].position);

        currentPoint++;

        if (currentPoint >= patrolPoints.Length)
        {
            currentPoint = 0;
        }
    }

    public void StopPatrol()
    {
        isPatrolling = false;
        movement.Stop();
    }

    public void ResumePatrol()
    {
        if (isPatrolling)
        {
            return;
        }

        isPatrolling = true;
        MoveToNextPoint();
    }
}