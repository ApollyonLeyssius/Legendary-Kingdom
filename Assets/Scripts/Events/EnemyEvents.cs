using UnityEngine;
using System;

public class EnemyEvents
{
    public event Action EnemyKilled;
    public event Action onBossKilled;
    public EnemyEvents()
    {
        Health.OnEnemyDeath += HandleEnemyDeath;
        Health.onBossDeath += HandleBossDeath;
    }
    public void Dispose()
    {
        Health.OnEnemyDeath -= HandleEnemyDeath;
        Health.onBossDeath -= HandleBossDeath;
    }

    private void HandleEnemyDeath()
    {
        EnemyKilled?.Invoke();
    }

    private void HandleBossDeath()
    {
        onBossKilled?.Invoke();
    }


}