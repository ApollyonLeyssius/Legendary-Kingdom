using UnityEngine;

public class TestQuestStep : QuestStep
{
    private int coinsCollected = 0;
    private int coinsToComplete = 5;

    private void OnEnable()
    {
        SubscribeAllCoins();
    }

    private void OnDisable()
    {
        UnsubscribeAll();
    }

    private void SubscribeAllCoins()
    {
        foreach (var coin in FindObjectsByType<CointCollection>(FindObjectsSortMode.None))
            coin.CoinCollect += OnCoinCollected;

    }

    private void UnsubscribeAll()
    {
        foreach (var coin in FindObjectsByType<CointCollection>(FindObjectsSortMode.None))
            coin.CoinCollect -= OnCoinCollected;
    }

    private void OnCoinCollected()
    {
        if (coinsCollected < coinsToComplete)
        {
            coinsCollected++;
            UpdateState();
        }

        if (coinsCollected >= coinsToComplete)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = coinsCollected.ToString();
        Changestate(state);
    }

    protected override void SetQuestStepState(string state)
    {
        this.coinsCollected = System.Int32.Parse(state);
        UpdateState();
    }
}
