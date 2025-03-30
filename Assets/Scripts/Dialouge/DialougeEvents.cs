using System;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialougeEvents
{
    public event Action<string> onEnterDialogue;
    public event Action<string, List<Choice>, List<string>> onDisplayDialogue;
    public event Action onDialogueStarted;
    public event Action onDialogueFinished;
    public event Action onDialogueSkipped;

    public event Action<int> onUpdateChoiceIndex;

    public event Action<string, Ink.Runtime.Object> onUpdateInkDialogueVars;

    public void EnterDialogue(string knotName)
    {
        onEnterDialogue?.Invoke(knotName);
    }
    public void DisplayDialogue(string dialogueLine, List<Choice> dialogueChoices, List<string> dialogueTags)
    {
        onDisplayDialogue?.Invoke(dialogueLine, dialogueChoices, dialogueTags);
    }
    public void DialogueStarted()
    {
        onDialogueStarted?.Invoke();
    }

    public void DialogueSkipped()
    {
        onDialogueSkipped?.Invoke();
    }

    public void DialougeFinished()
    {
        onDialogueFinished?.Invoke();
    }

    public void UpdateChoiceIndex(int index)
    {
        onUpdateChoiceIndex?.Invoke(index);
    }

    public void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value)
    {
        onUpdateInkDialogueVars?.Invoke(name, value);
    }
}
