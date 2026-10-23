using UnityEngine;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextAsset inkJson;

    private Story story;
    private bool dialoguePlaying = false;

    private void Awake()
    {
        story = new Story(inkJson.text);
    }
    private void OnEnable()
    {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue += EnterDialogue;
        GameEventsManager.instance.inputEvents.OnSubmitPressed += SubmitPressed;
        
    }
    private void OnDisable()
    {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
        GameEventsManager.instance.inputEvents.OnSubmitPressed -= SubmitPressed;
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

        ContinueOrExitStory();
    }

    private void ContinueOrExitStory() 
    {
        if (story.canContinue)
        {
            string dialogueLine = story.Continue();

            GameEventsManager.instance.dialogueEvents.DisplayDialogue(dialogueLine);
        }
        else
        {
            ExitDialogue();
        }
    }
    private void ExitDialogue() 
    {
        dialoguePlaying = false;

        GameEventsManager.instance.dialogueEvents.DialogueFinished();

        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DEFAULT);

        story.ResetState();
    }
}
