using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{

    private bool bIsFinished = false;


    private string questId;

    public void InitQuestStep(string questID)
    {
        questId = questID;
    }
    protected void FinishQuestStep()
    {
        if(!bIsFinished)
        {
            bIsFinished = true;
            GameEventManager.instance.questEvents.AdvanceQuest(questId);
            Destroy(gameObject);
        }
    }
}
