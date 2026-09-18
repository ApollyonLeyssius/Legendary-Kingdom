using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float fieldOfView = 90f;

    [Header("Obstacles")]
    [SerializeField] private LayerMask obstacleLayer;

    private void Update()
    {
        if (CanSeePlayer())
        {
            Debug.Log("Player detected!");
        }
    }

    public bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }

        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        
        if (distanceToPlayer > detectionRange)
        {
            return false;
        }

        directionToPlayer.Normalize();

        
        float angleToPlayer = Vector3.Angle(
            transform.forward,
            directionToPlayer
        );

        if (angleToPlayer > fieldOfView / 2f)
        {
            return false;
        }

        
        Vector3 eyePosition = transform.position + Vector3.up;

        bool obstacleDetected = Physics.Raycast(
            eyePosition,
            directionToPlayer,
            distanceToPlayer,
            obstacleLayer
        );

        if (obstacleDetected)
        {
            return false;
        }

        return true;
    }
}