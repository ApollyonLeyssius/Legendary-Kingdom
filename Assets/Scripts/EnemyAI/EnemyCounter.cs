
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    [Header("Counterattack")]
    [SerializeField] private Transform player;
    [SerializeField] private float counterDamage = 15f;
    [SerializeField] private float counterRange = 3f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Health playerHealth;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (animator == null)
        {
            Debug.LogWarning(
                "EnemyCounter: Animator is missing!"
            );
        }

        if (player == null)
        {
            Debug.LogError(
                "EnemyCounter: Player not assigned!"
            );
            return;
        }

        playerHealth = player.GetComponent<Health>();

        if (playerHealth == null)
        {
            Debug.LogError(
                "EnemyCounter: Player has no Health!"
            );
        }
    }

    public bool CounterAttack()
    {
        if (player == null || playerHealth == null)
        {
            Debug.LogWarning(
                "Counter failed: Missing Player or Health!"
            );
            return false;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        Debug.Log("Counter distance: " + distance);

        if (distance > counterRange)
        {
            Debug.LogWarning(
                "Counter failed: Player out of range!"
            );
            return false;
        }

        PlayCounterAnimation();

        Debug.Log("Enemy performed a COUNTERATTACK!");

        playerHealth.TakeDamage(counterDamage);

        return true;
    }

    private void PlayCounterAnimation()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetTrigger("IsCrossPunching");
    }
}
