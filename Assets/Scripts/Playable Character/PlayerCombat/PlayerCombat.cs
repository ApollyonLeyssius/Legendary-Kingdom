using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask enemyLayer;

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
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
            Health enemyHealth = enemyCollider.GetComponent<Health>();

            if (enemyHealth == null)
            {
                continue;
            }

            enemyHealth.TakeDamage(attackDamage);

            Debug.Log("Player attacked " + enemyCollider.gameObject.name);
        }
    }
}