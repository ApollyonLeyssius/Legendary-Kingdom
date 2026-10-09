using UnityEngine;

public class DistanceCombat : MonoBehaviour
{
    [SerializeField] private float shortAttackDistance = 2f;
    [SerializeField] private float longAttackDistance = 5f;

    public enum AttackType
    {
        None,
        Short,
        Long
    }

    public AttackType GetAttackType(float distance)
    {
        if (distance <= shortAttackDistance)
        {
            return AttackType.Short;
        }

        if (distance <= longAttackDistance)
        {
            return AttackType.Long;
        }

        return AttackType.None;
    }
}