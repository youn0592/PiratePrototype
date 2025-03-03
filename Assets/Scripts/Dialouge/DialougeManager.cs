using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialougeManager : MonoBehaviour
{
    public static DialougeManager instance { get; private set; }
    [Header("Dialogue UI")]
    [SerializeField]
    GameObject dialoguePanel;
    [SerializeField]
    TextMeshProUGUI dialogueText;

    [Header("ChoicesUI")]
    [SerializeField]
    private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    Story currentStory;
    public bool bDialoguePlaying { get; private set; }

    [SerializeField]
    InputReader input;


    private void Start()
    {
        if (instance == null)
            instance = this;

        dialoguePanel.SetActive(false);
        bDialoguePlaying = false;

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach(GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            ++index;
        }

        input.PirateSubmitEvent += HandleSubmit;
    }

    private void OnDestroy()
    {
        input.PirateSubmitEvent -= HandleSubmit;
    }

    void Update()
    {
        if (!bDialoguePlaying)
            return;


    }

    public void TriggerDialogue(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        bDialoguePlaying = true;
        dialoguePanel.SetActive(true);
        ContinueStory();
    }

    public void ExitDialogue()
    {
        bDialoguePlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
            DisplayChoices();
        }
        else
        {
            ExitDialogue();
        }
    }

    void DisplayChoices()
    {
       List<Choice> currentChoices = currentStory.currentChoices;

        if(currentChoices.Count > choices.Length)
        {
            Debug.LogError("There are too many choices available then what the UI can Handle!");
        }

        int index = 0;
        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            ++index;
        }
        for(int i = index; i < choices.Length; ++i)
        {
            choices[i].SetActive(false);
        }

        //StartCoroutine(SelectFirstChoice());
    }
    void HandleSubmit(float val)
    {
        if (bDialoguePlaying)
        {
            ContinueStory();
        }
    }

    //private IEnumerator SelectFirstChoice()
    //{
    //    EventSystem.current.SetSelectedGameObject(null);
    //    yield return new WaitForEndOfFrame();
    //    EventSystem.current.SetSelectedGameObject(choices[0]);
    //}

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }
}
