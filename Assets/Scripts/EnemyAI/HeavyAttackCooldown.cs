using UnityEngine;

public class HeavyAttackCooldown : MonoBehaviour
{
    [SerializeField] private float cooldownDuration = 5f;

    private float cooldownTimer;

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public bool CanUseHeavyAttack()
    {
        return cooldownTimer <= 0f;
    }

    public void StartCooldown()
    {
        cooldownTimer = cooldownDuration;
    }
}