using System.Runtime.CompilerServices;
using UnityEngine;

public class PAtrol : MonoBehaviour
{
    [SerializeField] private ObstacleMovement movement;
    [SerializeField] private Transform[] patrolPoints;

    private int currentPoint;
   
    void Start()
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
        UpdatePatrol();
    }


    void UpdatePatrol()
    {
        if(patrolPoints.Length == 0)
        {
            return;
        }

        Debug.Log("Checking patrol points");

        if (movement.HasReachedDestination())
        {
            Debug.Log("Reached patrol point");

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
}


