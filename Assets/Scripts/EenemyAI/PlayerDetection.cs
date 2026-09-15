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

    void Start()
    {
        if (())
        {
            Debug.Log("Can see player");
        }
    }

    
    void Update()
    {
        
    }
}
