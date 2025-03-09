using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObjects/QuestInfoSO", order = 1)]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField]
    public string id { get; private set; }

    [Header("General")]
    public string displayName;

    [Header("Requirements")]
    public int levelRequirement; //For testing purpose
    public QuestInfoSO[] questPrerequisites;

    [Header("Steps  ")]
    public GameObject[] questStepPrefab;

    [Header("Rewards")]
    public int goldReward;
    public int experienceReward;

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
