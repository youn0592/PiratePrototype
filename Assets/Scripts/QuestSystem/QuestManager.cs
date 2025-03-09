using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap;

    int currentPlayerLevel = 1;

    private void Awake()
    {
        questMap = CreateQuestMap();
    }
    private void OnEnable()
    {
        GameEventManager.instance.questEvents.onStartQuest += StartQuest;
        GameEventManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventManager.instance.questEvents.onFinishQuest += FinishQuest;
    }

    private void OnDisable()
    {
        GameEventManager.instance.questEvents.onStartQuest -= StartQuest;
        GameEventManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventManager.instance.questEvents.onFinishQuest -= FinishQuest;
    }

    private void Start()
    {
        foreach(Quest quest in questMap.Values)
        {
            GameEventManager.instance.questEvents.QuestStateChanged(quest); 
        }
    }

    void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestByID(id);
        quest.state = state;
        GameEventManager.instance.questEvents.QuestStateChanged(quest);
    }

    bool CheckRequirements(Quest quest)
    {
        bool bMeetsRequirements = true;

        if(currentPlayerLevel < quest.info.levelRequirement)
        {
            bMeetsRequirements = false;
            return bMeetsRequirements;
        }

        foreach(QuestInfoSO preReqQuestInfo in quest.info.questPrerequisites)
        {
            if(GetQuestByID(preReqQuestInfo.id).state != QuestState.FINISHED)
            {
                bMeetsRequirements = false;
                break;
            }
        }

        return bMeetsRequirements;
    }

    void RequirementChange()
    {
        foreach(Quest quest in questMap.Values)
        {
            if(quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirements(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    private void Update()
    {
        //This will be removed.
        RequirementChange();
    }

    void StartQuest(string id)
    {
        Debug.Log("Quest Started");
        Quest quest = GetQuestByID(id);
        quest.CreateCurrentQuestStep(transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
    }

    void AdvanceQuest(string id)
    {
        Quest quest = GetQuestByID(id);
        quest.MoveToNextStep();

        if(quest.CurrentStepExists())
        {
            quest.CreateCurrentQuestStep(transform);
        }
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
            Debug.Log("Quest Can Finish");
        }
    }

    void FinishQuest(string id)
    {
        Quest quest = GetQuestByID(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
        Debug.Log("Quest Complete");
    }

    void ClaimRewards(Quest quest)
    {
        //TODO
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        //Load all Scriptable objects under the Assets/Resources/Quests folder
        QuestInfoSO[] allQuest = Resources.LoadAll<QuestInfoSO>("Quests");

        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach(QuestInfoSO questInfo in allQuest)
        {
            if(idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when making map: " + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }

        return idToQuestMap;
    }

    private Quest GetQuestByID(string id)
    {
        Quest quest = questMap[id];
        if(quest == null)
        {
            Debug.LogError("ID not found in the QuestMap: " + id);
        }
        return quest;
    }
}
