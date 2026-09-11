using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{

    public PlayerData PlayerData;
    [SerializeField] private float AttackCooldown = 1f;
    [SerializeField] private GameObject Attacks;
    private float lastAttackTime = -Mathf.Infinity;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void PlayerAttacking()
    {
        Instantiate(Attacks, transform.position, transform.rotation);
    }
    private void TakeDamage()
    {
       PlayerData.CurrentHealth -= PlayerData.AttackDamage;
    }
    private void OnTriggerEnter(Collider other)
    {

    }
    public void AttackPlayer(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (Time.time - lastAttackTime < AttackCooldown) return;

        PlayerAttacking();
        lastAttackTime = Time.time;
    }
}
