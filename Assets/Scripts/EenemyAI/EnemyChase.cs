using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [SerializeField] private PlayerDetection playerDetection;
    [SerializeField] private ObstacleMovement movement;
    [SerializeField] private Patrol patrol;

    private bool isChasing;

    private void Update()
    {
        if (playerDetection.CanSeePlayer())
        {
            

            ChasePlayer();
        }
        else
        {
            StopChasing();
        }
    }

    private void ChasePlayer()
    {
        if (!isChasing)
        {
            isChasing = true;
            patrol.StopPatrol();

            
        }

        movement.MoveTo(
            playerDetection.GetPlayerPosition()
        );
    }

    private void StopChasing()
    {
        if (!isChasing)
        {
            return;
        }

        isChasing = false;
        patrol.StartPatrol();

        
    }
}