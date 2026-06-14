using UnityEngine;

[System.Serializable]
public class PlayerData : MonoBehaviour
{
    private int MaxHealth;
    private int CurrentHealth;

    private int AttackDamage;
    private string CharacterName;




}

public enum CombatState
{
    InCombat,
    OutOfCombat
}
