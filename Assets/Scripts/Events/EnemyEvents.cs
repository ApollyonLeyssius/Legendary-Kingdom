using UnityEngine;
using System;

public class EnemyEvents
{
    public event Action EnemyKilled;
    public EnemyEvents()
    {
        Health.OnEnemyDeath += HandleEnemyDeath;
    }
    public void Dispose()
    {
        Health.OnEnemyDeath -= HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        EnemyKilled?.Invoke();
    }
}