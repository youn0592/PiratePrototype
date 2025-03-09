using System;
using UnityEngine;

public class QuestEvents
{
    public event Action<string> onStartQuest;
    public event Action<string> onAdvanceQuest;
    public event Action<string> onFinishQuest;
    public event Action<Quest> onQuestStateChange;

    public void StartQuest(string id)
    {
        if (onStartQuest != null)
        {
            onStartQuest(id);
        }
    }
    public void AdvanceQuest(string id)
    {
        if (onAdvanceQuest != null)
        {
            onAdvanceQuest(id);
        }
    }
    public void FinishQuest(string id)
    {
        if (onFinishQuest != null)
        {
            onFinishQuest(id);
        }
    }

    public void FinishQuest(Quest quest)
    {
        if (onQuestStateChange != null)
        {
            onQuestStateChange(quest);
        }
    }

    public void QuestStateChanged(Quest quest)
    {
        if (onQuestStateChange != null)
            onQuestStateChange(quest);
    }


}
