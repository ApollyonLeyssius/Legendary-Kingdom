using UnityEngine;

public class EnemyQuest : QuestStep
{
    private int EnemiesToKill = 0;
    private int EnemiesKilled = 2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateState();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void EnemyKilled()
    {

    }
    private void UpdateState()
    {
        string state = EnemiesToKill.ToString();
        string status = "Collected " + EnemiesToKill + " / " + EnemiesKilled + " coins.";
        Changestate(state, status);
    }
    protected override void SetQuestStepState(string state)
    {
        this.EnemiesToKill = System.Int32.Parse(state);
        UpdateState();
    }
}
