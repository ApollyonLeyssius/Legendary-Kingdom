using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool loadDeathSceneOnDeath;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private CharacterType characterType;
    public static event Action OnEnemyDeath;
    public static event Action onBossDeath;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f || currentHealth <= 0f)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        Debug.Log(
            gameObject.name +
            " got hit! Damage: " + damage +
            " | Health: " + currentHealth + "/" + maxHealth
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " died!");

        if (characterType == CharacterType.PLAYER)
        {
            if (loadDeathSceneOnDeath)
            {
                if (sceneLoader == null)
                {
                    Debug.LogError("Player Health is missing a SceneLoader!");
                    return;
                }

                sceneLoader.LoadDeathScene();
                return;
            }

            Destroy(gameObject);
            return;
        }

        if (characterType == CharacterType.ENEMY)
        {
            Debug.Log("Enemy death event triggered: " + gameObject.name);

            OnEnemyDeath?.Invoke();

            Destroy(gameObject);
            return;
        }

        if (characterType == CharacterType.BOSS)
        {
            onBossDeath?.Invoke();
            Destroy(gameObject);
            return;
        }
    }


}