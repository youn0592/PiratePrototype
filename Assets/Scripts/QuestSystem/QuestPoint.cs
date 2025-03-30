using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class QuestPoint : MonoBehaviour
{

    [Header("Dialogue")]
    [SerializeField] string dialogueKnotName;


    [Header("Quest")]
    [SerializeField] QuestInfoSO questInfo;

    [Header("Config")]
    [SerializeField] bool bStartPoint = true;
    [SerializeField] bool bFinishPoint = true;

    private string questId;
    private QuestState currentQuestState;

    private void Awake()
    {
        questId = questInfo.id;
    }

    private void OnEnable()
    {
        GameEventManager.instance.questEvents.onQuestStateChange += QuestStateChange;
    }

    private void OnDisable()
    {
        GameEventManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    public void InteractEngaged()
    {

        if(!dialogueKnotName.Equals(""))
        {
            GameEventManager.instance.dialougeEvents.EnterDialogue(dialogueKnotName);
        }
        else
        {
            if (currentQuestState.Equals(QuestState.CAN_START) && bStartPoint)
            {
                GameEventManager.instance.questEvents.StartQuest(questId);
            }
            else if (currentQuestState.Equals(QuestState.CAN_FINISH) && bFinishPoint)
            {
                GameEventManager.instance.questEvents.FinishQuest(questId);
            }
        }

    }
    void QuestStateChange(Quest quest)
    {
        if(quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
        }
    }

}
