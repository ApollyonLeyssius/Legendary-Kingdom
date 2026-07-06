using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    public PlayerData PlayerData;
    [SerializeField] private GameObject Attack;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            PlayerAttacking();
        }
        
    }
    private void PlayerAttacking()
    {
        Instantiate(Attack);
    }
    private void TakeDamage()
    {
       PlayerData.CurrentHealth -= PlayerData.AttackDamage;
    }
    private void OnTriggerEnter(Collider other)
    {

    }
}
