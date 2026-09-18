using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float MaxHealth;
    public float CurrentHealth;

    public int AttackDamage;
    public string CharacterName;




}

public enum CombatState
{
    InCombat,
    OutOfCombat
}
