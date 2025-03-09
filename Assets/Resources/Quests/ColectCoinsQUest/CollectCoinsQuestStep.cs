using Ink.UnityIntegration;
using UnityEngine;

public class CollectCoinsQuestStep : QuestStep
{
    private int coinsCollected = 0;
    private int coinsToCollect = 5;

    public InputReader input;

    private void CoinCollected(float var)
    {
        if(coinsCollected < coinsToCollect)
        {
            coinsCollected++;
        }
        if(coinsCollected >= coinsToCollect)
        {
            FinishQuestStep();
        }
        Debug.Log(coinsCollected);
    }

    private void OnEnable()
    {
        input.PirateTestEvent += CoinCollected;
    }
}
