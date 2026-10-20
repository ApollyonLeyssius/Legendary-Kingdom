using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask enemyLayer;

    private void Update()
    {
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            Attack();
        }
    }

    private void Attack()
    {
        Collider[] enemiesHit = Physics.OverlapSphere(
            transform.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider enemyCollider in enemiesHit)
        {
            Health enemyHealth =
                enemyCollider.GetComponentInParent<Health>();

            if (enemyHealth == null)
            {
                continue;
            }

            AdaptiveAI adaptiveAI =
                enemyHealth.GetComponent<AdaptiveAI>();

            if (adaptiveAI != null)
            {
                adaptiveAI.RegisterAction(
                    AdaptiveAI.PlayerAction.LightAttack
                );
            }

            enemyHealth.TakeDamage(attackDamage);

            Debug.Log(
                "Player attacked " + enemyHealth.gameObject.name
            );
        }
    }
}