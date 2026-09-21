using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private SceneLoader sceneLoader;

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
            gameObject.name + " got hit! Damage: " + damage +
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

        if (sceneLoader == null)
        {
            Debug.LogError("Health is missing a SceneLoader reference!");
            return;
        }

        sceneLoader.LoadDeathScene();
    }
}