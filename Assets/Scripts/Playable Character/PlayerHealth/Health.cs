using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool loadDeathSceneOnDeath;
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

        if (loadDeathSceneOnDeath)
        {
            if (sceneLoader == null)
            {
                Debug.LogError("Player Health is missing a SceneLoader!");
                return;
            }

            sceneLoader.LoadDeathScene();
        }
        else
        {
            Debug.Log("Destroying: " + gameObject.name);
            Destroy(gameObject);
        }
    }
}