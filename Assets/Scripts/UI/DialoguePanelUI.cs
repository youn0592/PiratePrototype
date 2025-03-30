using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.ComponentModel;
using Unity.VisualScripting;

public class DialoguePanelUI : MonoBehaviour
{
    public static DialoguePanelUI instance { get; private set; }

    [Header("Components")]
    [SerializeField] GameObject contentParent;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI displayNameText;
    [SerializeField] DialogueChoiceButton[] choiceButtons;
    [SerializeField] float typingSpeed = 0.04f;
    [SerializeField] GameObject continueStoryIcon;

    [Header("Audio")]
    AudioClip[] dialogueTypingSoundClips;
    AudioSource audioSource;
    bool bStopAudioSource = true;
    float minPitch = 0.5f;
    float maxPitch = 3f;

    bool bIsPlayingDialogue = false;

    private Coroutine displayLineCoroutine;

    private const string SPEAKER_TAG = "speaker";
    private const string VOICE_TAG = "voice";

    private void Awake()
    {
        continueStoryIcon.SetActive(false);
        contentParent.SetActive(false);
        audioSource = gameObject.AddComponent<AudioSource>();
        if (instance != null) Debug.LogError("Multiple Instances of DialoguePanel exist");
        instance = this;
        ResetPanel();
    }

    private void OnEnable()
    {
        GameEventManager.instance.dialougeEvents.onDialogueStarted += DialogueStarted;
        GameEventManager.instance.dialougeEvents.onDialogueFinished += DialogueFinished;
        GameEventManager.instance.dialougeEvents.onDisplayDialogue += DisplayDialouge;
        GameEventManager.instance.dialougeEvents.onDialogueSkipped += SubmitHit;
    }

    private void OnDisable()
    {
        GameEventManager.instance.dialougeEvents.onDialogueStarted -= DialogueStarted;
        GameEventManager.instance.dialougeEvents.onDialogueFinished -= DialogueFinished;
        GameEventManager.instance.dialougeEvents.onDisplayDialogue -= DisplayDialouge;
        GameEventManager.instance.dialougeEvents.onDialogueSkipped -= SubmitHit;
    }

    void DialogueStarted()
    {
        contentParent.SetActive(true);
    }

    void DialogueFinished()
    {
        dialogueTypingSoundClips = null;
        contentParent.SetActive(false);
        ResetPanel();
    }

    void DisplayDialouge(string dialogueLine, List<Choice> dialogueChoices, List<string> dialogueTags)
    {
        if (displayLineCoroutine != null)
        {
            bIsPlayingDialogue = false;
            StopCoroutine(displayLineCoroutine);
        }
        HandleTags(dialogueTags);
        bIsPlayingDialogue = true;
        displayLineCoroutine = StartCoroutine(DisplayLine(dialogueLine));

        if (dialogueChoices.Count > choiceButtons.Length)
        {
            Debug.LogError("More Dialogue choices then supported");
        }

        foreach (DialogueChoiceButton choiceButton in choiceButtons)
        {
            choiceButton.gameObject.SetActive(false);
        }
        int choiceButtonIndex = dialogueChoices.Count - 1;

        for (int i = 0; i < dialogueChoices.Count; i++)
        {
            Choice choice = dialogueChoices[i];
            DialogueChoiceButton choiceButton = choiceButtons[i];

            choiceButton.gameObject.SetActive(true);
            choiceButton.SetChoiceText(choice.text);
            choiceButton.SetChoiceIndex(i);

            if (i == 0)
            {
                choiceButton.SelectButton();
                GameEventManager.instance.dialougeEvents.UpdateChoiceIndex(0);
            }
            choiceButtonIndex++;
        }
    }

    void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTags = tag.Split(':');
            if (splitTags.Length != 2)
            {
                Debug.LogError("Tag could not be parsed");
            }

            string tagKey = splitTags[0].Trim();
            string tagValue = splitTags[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    break;
                case VOICE_TAG:
                    SetPirateVoice(tagValue);
                    break;
                default:
                    Debug.LogWarning("Tag Parsed but not recognized: " + tag);
                    break;
            }
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        dialogueText.text = "";
        continueStoryIcon.SetActive(false);
        bool bFirstPass = true;
        foreach (char letter in line.ToCharArray())
        {
            if (!bIsPlayingDialogue)
            {
                dialogueText.text = line;
                continueStoryIcon.SetActive(true);
                StopCoroutine(DisplayLine(line));
                break;
            }
            if (letter.ToString() == " " || bFirstPass)
            {
                bFirstPass = false;
                PlayDialogueSound(letter);
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        continueStoryIcon.SetActive(true);
        bIsPlayingDialogue = false;
    }

    void SetPirateVoice(string name)
    {
        YarkinVoiceDefinition yarkin = YarkinManager.instance.GetVoiceDefinition(name);
        if (yarkin == null)
        {
            Debug.LogError("yarkin was null in DialougePanelUL");
        }

        dialogueTypingSoundClips = yarkin.GetAudioClips();

        minPitch = yarkin._pitchMinMax.x;
        maxPitch = yarkin._pitchMinMax.y;
    }
    void PlayDialogueSound(int currentDisplayedCharCount)
    {
        if (bStopAudioSource)
        {
            audioSource.Stop();
        }
        if(dialogueTypingSoundClips == null)
        {
            SetPirateVoice("YV_Arr");
        }
        if(audioSource.isPlaying)
        {
            return;
        }
        int index = Random.Range(0, dialogueTypingSoundClips.Length);
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(dialogueTypingSoundClips[index]);
    }

    public bool IsPlayingDialogue()
    {
        return bIsPlayingDialogue;
    }

    void SubmitHit()
    {
        if (bIsPlayingDialogue)
        {
            bIsPlayingDialogue = false;
        }
    }

    void ResetPanel()
    {
        dialogueText.text = "";
    }
}
