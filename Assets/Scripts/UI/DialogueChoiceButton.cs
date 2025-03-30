using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DialogueChoiceButton : MonoBehaviour, ISelectHandler
{
    [Header("Components")]
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI choiceText;

    int choiceIndex = -1;

    public void SetChoiceText(string choiceTextString)
    {
        choiceText.text = choiceTextString;
    }

    public void SetChoiceIndex(int index)
    {
        choiceIndex = index;
    }

    public void SelectButton()
    {
        button.Select();
    }

    public void OnSelect(BaseEventData eventData)
    {
        GameEventManager.instance.dialougeEvents.UpdateChoiceIndex(choiceIndex);
    }
}
