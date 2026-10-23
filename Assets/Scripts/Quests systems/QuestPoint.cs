using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider))]
public class QuestPoint : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private string dialogueKnotName;
    [Header("Quest")]
    [SerializeField] private QuestSystemSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;
    private bool playerIsNear = false;
    private string questId;
    private QuestState currentQuestState;

    private QuestIcon questIcon;

    private void Awake()
    {
        questId = questInfoForPoint.id;
        questIcon = GetComponentInChildren<QuestIcon>();
    }
    private void BeginQuest(InputEventContext inputEventContext)
    {
        if (!playerIsNear || !inputEventContext.Equals(InputEventContext.DEFAULT))
        {
            return;
        }

        if (!dialogueKnotName.Equals(""))
        {
            GameEventsManager.instance.dialogueEvents.EnterDialogue(dialogueKnotName);
        }

        else
        {
            if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
            {
                Debug.Log("started");
                GameEventsManager.instance.questEvents.StartQuest(questId);
            }

            else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
            {
                GameEventsManager.instance.questEvents.FinishQuest(questId);
            }
        }

    }

    private void OnEnable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.instance.inputEvents.OnSubmitPressed += BeginQuest;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        GameEventsManager.instance.inputEvents.OnSubmitPressed -= BeginQuest;
    }

    private void QuestStateChange(Quest quest)
    {
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
            questIcon.SetState(currentQuestState, startPoint, finishPoint);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
            Debug.Log(playerIsNear);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}
