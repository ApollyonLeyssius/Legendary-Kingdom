using System;
using UnityEngine;

public class EnemyDeathEvent : MonoBehaviour
{
    public static event Action<EnemyDeathEvent> OnEnemyKilled;

    public void NotifyDeath()
    {
        Debug.Log("Enemy death event triggered: " + gameObject.name);

        OnEnemyKilled?.Invoke(this);
    }
}