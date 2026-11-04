using UnityEngine;
using Ink.Runtime;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextAsset inkJson;

    private Story story;

    private int currentChoiceIndex = -1;
    private bool dialoguePlaying = false;

    private InkExternalFunctions inkExternalFunctions;

    private InkDialogueVariables inkDialogueVariables;

    private void Awake()
    {
        story = new Story(inkJson.text);
        inkExternalFunctions = new InkExternalFunctions();
        inkExternalFunctions.Bind(story);
        inkDialogueVariables = new InkDialogueVariables(story);
    }

    private void OnDestroy()
    {
        inkExternalFunctions.Unbind(story);
    }
    private void OnEnable()
    {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue += EnterDialogue;
        GameEventsManager.instance.inputEvents.OnSubmitPressed += SubmitPressed;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex += UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialogueVariable += UpdateInkDialogueVariable;
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        
    }
    private void OnDisable()
    {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
        GameEventsManager.instance.inputEvents.OnSubmitPressed -= SubmitPressed;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex -= UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialogueVariable -= UpdateInkDialogueVariable;
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void QuestStateChange(Quest quest)
    {
        string variableName = quest.info.id + "State";
        Debug.Log($"Updating Ink var '{variableName}' to '{quest.state}'");
        GameEventsManager.instance.dialogueEvents.UpdateInkDialogueVariable(
            variableName,
            new StringValue(quest.state.ToString())
        );
    }

    private void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value)
    {
        inkDialogueVariables.UpdateVariableState(name, value);
    }

    private void UpdateChoiceIndex(int choiceIndex)
    {
        this.currentChoiceIndex = choiceIndex;
    }

    private void SubmitPressed(InputEventContext inputEventContext)
    {
        if (!inputEventContext.Equals(InputEventContext.DIALOGUE))
        {
            return;
        }
        ContinueOrExitStory();
    }
    private void EnterDialogue(string knotName)
    {
        if (dialoguePlaying)
        {
            return;
        }
        dialoguePlaying = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameEventsManager.instance.dialogueEvents.DialogueStarted();

        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DIALOGUE);

        if (!knotName.Equals(""))
        {
            story.ChoosePathString(knotName);
        }
        else
        {
            Debug.LogWarning("Knot name was the empty string when entering dialogue.");
        }

        inkDialogueVariables.SyncVariablesAndStartListening(story);

        ContinueOrExitStory();
    }

    private void ContinueOrExitStory() 
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
                string voiceId = GetTagValue(story.currentTags, "voice");
                GameEventsManager.instance.dialogueEvents.RequestVoiceLine(voiceId);
                GameEventsManager.instance.dialogueEvents.DisplayDialogue(dialogueLine, story.currentChoices);
            }
        }
        else if (story.currentChoices.Count == 0)
        {
            ExitDialogue();
        }
    }
    private void ExitDialogue() 
    {
        dialoguePlaying = false;

        GameEventsManager.instance.dialogueEvents.RequestVoiceLine(string.Empty);

        GameEventsManager.instance.dialogueEvents.DialogueFinished();

        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DEFAULT);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inkDialogueVariables.StopListening(story);

        story.ResetState();
    }

    private bool IsLineBlank(string dialogueLine)
    {
        return dialogueLine.Trim().Equals("") || dialogueLine.Trim().Equals("\n");
    }

    private string GetTagValue(List<string> tags, string tagName)
    {
        foreach (string tag in tags)
        {
            int separatorIndex = tag.IndexOf(':');

            if (separatorIndex == -1) 
            {
                continue;
            }

            string key = tag.Substring(0, separatorIndex).Trim();
            string value = tag.Substring(separatorIndex + 1).Trim();

            if (key.Equals(tagName, System.StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }
        }
        return string.Empty;
    }
}
