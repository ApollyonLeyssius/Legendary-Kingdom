using UnityEngine;

public class EnemyQuest : QuestStep
{
    private int EnemiesToKill = 2;
    private int EnemiesKilled = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateState();
    }
    private void OnEnable()
    {
        GameEventsManager.instance.enemyEvents.EnemyKilled += EnemyKilled;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.enemyEvents.EnemyKilled -= EnemyKilled;
    }
    private void EnemyKilled()
    {
        if (EnemiesKilled >= EnemiesToKill)
        {
            return;
        }
        EnemiesKilled++;

        UpdateState();

        if (EnemiesKilled >= EnemiesToKill)
        {
            FinishQuestStep();
        }
    }
    private void UpdateState()
    {
        string state = EnemiesKilled.ToString();
        string status = "Defeated " + EnemiesKilled + " / " + EnemiesToKill + " enemies";
        Changestate(state, status);
    }
    protected override void SetQuestStepState(string state)
    {
        this.EnemiesToKill = System.Int32.Parse(state);
        UpdateState();
    }
}
