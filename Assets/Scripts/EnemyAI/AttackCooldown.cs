using UnityEngine;

public class AttackCooldown : MonoBehaviour
{
    [SerializeField] private float cooldownDuration = 1.5f;

    private float cooldownTimer;

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public bool CanAttack()
    {
        return cooldownTimer <= 0f;
    }

    public void StartCooldown()
    {
        cooldownTimer = cooldownDuration;
    }
}