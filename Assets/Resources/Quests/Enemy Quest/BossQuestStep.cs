using Unity.VisualScripting;
using UnityEngine;

public class BossQuestStep : QuestStep
{
    private int BossesToKill = 1;
    private int Bosseskilled = 0;
    
    private void OnEnable()
    {
        GameEventsManager.instance.enemyEvents.onBossKilled += BossesKilled;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.enemyEvents.onBossKilled -= BossesKilled;
    }

    private void Start()
    {
        UpdateState();
    }

    private void BossesKilled()
    {
        FinishQuestStep();
    }

    private void UpdateState()
    {
        string state = Bosseskilled.ToString();
        string status = "Kill the boss roaming the map";
        Changestate(state, status);
    }
    protected override void SetQuestStepState(string state)
    {
        this.BossesToKill = System.Int32.Parse(state);
        UpdateState();
    }
}
