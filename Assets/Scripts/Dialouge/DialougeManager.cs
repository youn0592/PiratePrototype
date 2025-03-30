using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialougeManager : MonoBehaviour
{
    [Header("Ink Story")]
    [SerializeField] private TextAsset inkJson;

    private Story story;
    int currentChoiceIndex = -1;

    private bool bDialoguePlaying = false;

    private InkExternalFunctions inkExternals;
    private InkDialogueVars inkDialogueVars;

    private void Awake()
    {
        story = new Story(inkJson.text);
        inkExternals = new InkExternalFunctions();
        inkExternals.Bind(story);
        inkDialogueVars = new InkDialogueVars(story);
    }

    private void OnDestroy()
    {
        inkExternals.Unbind(story);
    }

    private void OnEnable()
    {
        GameEventManager.instance.dialougeEvents.onEnterDialogue += EnterDialogue;
        GameEventManager.instance.dialougeEvents.onUpdateChoiceIndex += UpdateChoiceIndex;
        GameEventManager.instance.dialougeEvents.onUpdateInkDialogueVars += UpdateInkDialogueVariable;
        //GameEventManager.instance.inputEvents.PirateSubmitEvent += SubmitPressed;
        GameEventManager.instance.inputEvents.UISubmitEvent += SubmitPressed;
        GameEventManager.instance.questEvents.onQuestStateChange += QuestStateChange;
    }
    private void OnDisable()
    {
        GameEventManager.instance.dialougeEvents.onEnterDialogue -= EnterDialogue;
        GameEventManager.instance.dialougeEvents.onUpdateChoiceIndex -= UpdateChoiceIndex;
        GameEventManager.instance.dialougeEvents.onUpdateInkDialogueVars -= UpdateInkDialogueVariable;
        //GameEventManager.instance.inputEvents.PirateSubmitEvent -= SubmitPressed;
        GameEventManager.instance.inputEvents.UISubmitEvent -= SubmitPressed;
        GameEventManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    void QuestStateChange(Quest quest)
    {
        GameEventManager.instance.dialougeEvents.UpdateInkDialogueVariable(quest.info.id + "State", new StringValue(quest.state.ToString()));
    }

    void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value)
    {
        inkDialogueVars.UpdateVariableState(name, value);
    }
    void UpdateChoiceIndex(int index)
    {
        currentChoiceIndex = index;
    }

    void SubmitPressed(float val)
    {
        if (!bDialoguePlaying) return; //TODO: Hook up to UI Input
        if(DialoguePanelUI.instance.IsPlayingDialogue())
        {
            GameEventManager.instance.dialougeEvents.DialogueSkipped();
            return;
        }

        ContinueStory();
    }

    void EnterDialogue(string knotName)
    {
        if (bDialoguePlaying) return;

        bDialoguePlaying = true;

        GameEventManager.instance.dialougeEvents.DialogueStarted();

        if (!knotName.Equals(""))
        {
            story.ChoosePathString(knotName);
        }
        else
        {
            Debug.LogWarning("Knot name was empty when entering dialogue");
        }

        inkDialogueVars.SyncVariableAndStartListening(story);

        ContinueStory();
    }

    void ContinueStory()
    {
        if (story.currentChoices.Count > 0 && currentChoiceIndex != -1)
        {
            story.ChooseChoiceIndex(currentChoiceIndex);
            currentChoiceIndex = -1;
        }

        if (story.canContinue)
        {
            string dialogueLine = story.Continue();
            while (IsLineBlank(dialogueLine) && story.canContinue)
            {
                dialogueLine = story.Continue();
            }
            if (IsLineBlank(dialogueLine) && !story.canContinue)
            {
                ExitDialogue();
            }
            else
            {
                GameEventManager.instance.dialougeEvents.DisplayDialogue(dialogueLine, story.currentChoices, story.currentTags);
            }
        }
        else if (story.currentChoices.Count == 0)
        {
            StartCoroutine(ExitDialogue());
        }
    }

    IEnumerator ExitDialogue()
    {
        yield return null;
        GameEventManager.instance.dialougeEvents.DialougeFinished();
        bDialoguePlaying = false;
        inkDialogueVars.StopListening(story);
        story.ResetState();
    }

    bool IsLineBlank(string dialogueLine)
    {
        return dialogueLine.Trim().Equals("") || dialogueLine.Trim().Equals("\n");
    }
}
