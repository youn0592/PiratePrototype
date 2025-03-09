using UnityEngine;
using UnityEngine.UIElements;

public class Quest
{
    public QuestInfoSO info;

    public QuestState state;

    private int currentQuestStepIndex;

    public Quest(QuestInfoSO questInfo)
    {
        info = questInfo;
        state = QuestState.REQUIREMENTS_NOT_MET;
        currentQuestStepIndex = 0;
    }

    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return (currentQuestStepIndex < info.questStepPrefab.Length);
    }

    public void CreateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPB = GetCurrentQuestStepPrefab();
        if(questStepPB != null)
        {
            Debug.Log("Hit");
            QuestStep questStep = Object.Instantiate<GameObject>(questStepPB, parentTransform).GetComponent<QuestStep>();
            questStep.InitQuestStep(info.id);
        }
    }

    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPB = null;
        if(CurrentStepExists())
        {
            questStepPB = info.questStepPrefab[currentQuestStepIndex];
        }
        else
        {
            Debug.LogWarning("Quest step prefab was out of range, QuestID=" + info.id + " , stepIndex=" + currentQuestStepIndex);
        }
        return questStepPB;
    }
}
