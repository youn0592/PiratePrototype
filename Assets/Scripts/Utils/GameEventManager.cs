using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager instance { get; private set; }

    public InputReader inputEvents;
    public QuestEvents questEvents;
    public DialougeEvents dialougeEvents;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple instances of Event Manager");
        }
        instance = this;
        questEvents = new QuestEvents();
        dialougeEvents = new DialougeEvents();
    }
}
