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
        
    }
    private void OnDisable()
    {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
    }
    private void EnterDialogue(string knotName)
    {
        if (dialoguePlaying)
        {
            return;
        }
        dialoguePlaying = true;

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

            Debug.Log(dialogueLine);
        }
        else
        {
            ExitDialogue();
        }
    }
    private void ExitDialogue() 
    {
        Debug.Log("Exiting Dialogue");

        dialoguePlaying = false;

        story.ResetState();
    }
}
