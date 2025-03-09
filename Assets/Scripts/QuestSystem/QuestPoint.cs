using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class QuestPoint : MonoBehaviour
{
    [SerializeField]
    GameObject popup;

    [Header("Quest")]
    [SerializeField]
    QuestInfoSO questInfo;

    [Header("Config")]
    [SerializeField]
    bool bStartPoint = true;
    [SerializeField]
    bool bFinishPoint = true;

    [SerializeField]
    InputReader input;

    private bool bPlayerIsNear = false;
    private string questId;
    private QuestState currentQuestState;

    private void Awake()
    {
        questId = questInfo.id;
    }

    private void OnEnable()
    {
        popup.SetActive(false);
        GameEventManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        input.PirateInteractEvent += InteractPressed;
    }

    private void OnDisable()
    {
        GameEventManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    void InteractPressed(float val)
    {
        if (!bPlayerIsNear) return;

        if(currentQuestState.Equals(QuestState.CAN_START) && bStartPoint)
        {
            GameEventManager.instance.questEvents.StartQuest(questId);
        }
        else if(currentQuestState.Equals(QuestState.CAN_FINISH) && bFinishPoint)
        {
            GameEventManager.instance.questEvents.FinishQuest(questId);
        }
    }
    void QuestStateChange(Quest quest)
    {
        if(quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            popup.SetActive(true);
            bPlayerIsNear = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            popup.SetActive(false);
            bPlayerIsNear = false;
        }
    }
}
