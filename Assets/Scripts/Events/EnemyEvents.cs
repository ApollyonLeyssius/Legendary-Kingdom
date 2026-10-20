using UnityEngine;

public class EnemyKillQuestListener : MonoBehaviour
{
    [SerializeField] private int requiredKills = 3;

    private int currentKills;

    private void OnEnable()
    {
        Health.OnEnemyDeath += HandleEnemyDeath;
    }

    private void OnDisable()
    {
        Health.OnEnemyDeath -= HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        currentKills++;

        Debug.Log("Enemies defeated: " + currentKills);

        if (currentKills >= requiredKills)
        {
            Debug.Log("Quest completed!");
        }
    }
}