using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    [Header("Counterattack")]
    [SerializeField] private Transform player;
    [SerializeField] private float counterDamage = 15f;
    [SerializeField] private float counterRange = 3f;

    private Health playerHealth;

    private void Awake()
    {
        if (player == null)
        {
            Debug.LogError("EnemyCounter: Player not assigned!");
            return;
        }

        playerHealth = player.GetComponent<Health>();

        if (playerHealth == null)
        {
            Debug.LogError("EnemyCounter: Player has no Health!");
        }
    }

    public bool CounterAttack()
    {
        if (player == null || playerHealth == null)
        {
            Debug.LogWarning("Counter failed: Missing Player or Health!");
            return false;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        Debug.Log("Counter distance: " + distance);

        if (distance > counterRange)
        {
            Debug.LogWarning("Counter failed: Player out of range!");
            return false;
        }

        Debug.Log("Enemy performed a COUNTERATTACK!");

        playerHealth.TakeDamage(counterDamage);

        return true;
    }
}