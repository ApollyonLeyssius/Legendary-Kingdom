using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Damage")]
    [SerializeField] private float shortAttackDamage = 20f;
    [SerializeField] private float longAttackDamage = 10f;

    private DistanceCombat distanceCombat;
    private AttackCooldown attackCooldown;
    private Health playerHealth;

    private bool isInAttackRange;

    private void Awake()
    {
        distanceCombat = GetComponent<DistanceCombat>();
        attackCooldown = GetComponent<AttackCooldown>();

        if (distanceCombat == null)
        {
            Debug.LogError("Enemy is missing DistanceCombat!");
        }

        if (attackCooldown == null)
        {
            Debug.LogError("Enemy is missing AttackCooldown!");
        }

        if (player == null)
        {
            Debug.LogError("Player has not been assigned to EnemyCombat!");
            return;
        }

        playerHealth = player.GetComponent<Health>();

        if (playerHealth == null)
        {
            Debug.LogError("Player is missing Health!");
        }
    }

    private void Update()
    {
        if (player == null ||
            playerHealth == null ||
            distanceCombat == null ||
            attackCooldown == null)
        {
            return;
        }

        UpdateCombat();
    }

    private void UpdateCombat()
    {
        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position
        );

        DistanceCombat.AttackType attackType =
            distanceCombat.GetAttackType(distanceToPlayer);

        isInAttackRange =
            attackType != DistanceCombat.AttackType.None;

        if (!isInAttackRange)
        {
            return;
        }

        if (!attackCooldown.CanAttack())
        {
            return;
        }

        switch (attackType)
        {
            case DistanceCombat.AttackType.Short:
                ShortAttack();
                break;

            case DistanceCombat.AttackType.Long:
                LongAttack();
                break;
        }
    }

    public bool IsInAttackRange()
    {
        return isInAttackRange;
    }

    private void ShortAttack()
    {
        Debug.Log("Enemy used SHORT attack!");

        playerHealth.TakeDamage(shortAttackDamage);
        attackCooldown.StartCooldown();
    }

    private void LongAttack()
    {
        Debug.Log("Enemy used LONG attack!");

        playerHealth.TakeDamage(longAttackDamage);
        attackCooldown.StartCooldown();
    }
}