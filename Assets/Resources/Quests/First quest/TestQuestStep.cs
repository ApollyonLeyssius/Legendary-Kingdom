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
        coinsCollected++;
        if (coinsCollected >= coinsToComplete)
        {
            FinishQuestStep();
        }
    }
}
