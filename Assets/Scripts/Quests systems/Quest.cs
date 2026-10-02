using UnityEngine;

public class Quest
{
    public QuestSystemSO info;
    public QuestState state;

    private int QuestStepIndex;
    private QuestStepState[] questStepStates;

    public Quest(QuestSystemSO questInfo)
    {
        this.info = questInfo;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.QuestStepIndex = 0;
        this.questStepStates = new QuestStepState[info.questStepPrefabs.Length];
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
    }

    public Quest(QuestSystemSO questInfo, QuestState questState, int questStepIndex, QuestStepState[] questStepStates)
    {
        this.info = questInfo;
        this.state = questState;
        this.QuestStepIndex = questStepIndex;
        this.questStepStates = questStepStates;

        if (this.questStepStates.Length != this.info.questStepPrefabs.Length)
        {
            Debug.LogWarning("Quest Step Prefabs and Quest Step States are of different lengths. This indicates something changed with the QuestInfo and the saved date is now out of sync. Reset your data - as this might cause issues. QuestId: " + this.info.id);
        }
    }

    public void MoveTeNextStep()
    {
        QuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return (QuestStepIndex < info.questStepPrefabs.Length);
    }

    public void instantiateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            QuestStep questStep = Object.Instantiate<GameObject>(questStepPrefab, parentTransform)
                .GetComponent<QuestStep>();
            questStep.InitializeQuestStep(info.id, QuestStepIndex, questStepStates[QuestStepIndex].state);
        }
    }

    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExists())
        {
            questStepPrefab = info.questStepPrefabs[QuestStepIndex];
        }
        else
        {
            Debug.LogWarning("Tried to get quest prefab, but stepIndex was out of range indicating that " + "there's no current step: QuestID=" + info.id + ", stepIndex=" + QuestStepIndex);
        }
        return questStepPrefab;

    }

    public void StoreQuestStepStates(QuestStepState questStepState, int stepIndex)
    {
        if (stepIndex < questStepStates.Length)
        {
            questStepStates[stepIndex].state = questStepState.state;
        }
        else
        {
            Debug.LogWarning("Treid to acces quest step data, but stepIndex was out of range: " + "Quest Id = " + info.id + ", Step Index =" + stepIndex);
        }
    }

    public QuestData GetQuestData()
    {
        return new QuestData(state, QuestStepIndex, questStepStates);
    }
}
