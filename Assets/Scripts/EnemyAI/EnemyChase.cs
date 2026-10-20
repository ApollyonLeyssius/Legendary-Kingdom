using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [SerializeField] private PlayerDetection playerDetection;
    [SerializeField] private ObstacleMovement movement;
    [SerializeField] private Patrol patrol;
    [SerializeField] private EnemyCombat enemyCombat;

    private bool isChasing;

    private void Update()
    {
        if (!playerDetection.CanSeePlayer())
        {
            StopChasing();
            return;
        }

        if (enemyCombat.IsInAttackRange())
        {
            StopMovementForCombat();
            return;
        }

        ChasePlayer();
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

    private void StopMovementForCombat()
    {
        movement.Stop();
    }

    private void StopChasing()
    {
        if (!isChasing)
        {
            return;
        }

        isChasing = false;

        movement.Stop();
        patrol.ResumePatrol();
    }
}