using Unity.VisualScripting;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public EnemyData EnemyData;
    private PlayerData playerData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TakeDamage() 
    {
        EnemyData.CurrentHealth -= playerData.AttackDamage;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("Attack"))
        {
            TakeDamage();
        }
    }
}
