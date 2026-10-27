using UnityEngine;
using System;

public class EnemyEvents
{
    public event Action EnemyKilled;
    public event Action onBossKilled;
    public EnemyEvents()
    {
        Health.OnEnemyDeath += HandleEnemyDeath;
        Health.onBossDeath += HandleEnemyDeath;
    }
    public void Dispose()
    {
        Health.OnEnemyDeath -= HandleEnemyDeath;
        Health.onBossDeath -= HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        EnemyKilled?.Invoke();
    }


}