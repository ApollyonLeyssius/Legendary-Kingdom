using UnityEngine;

public class Quest
{
    public QuestSystemSO info;
    public QuestState state;

    private int QuestStepIndex;

    public Quest(QuestSystemSO questInfo)
    {
        this.info = questInfo;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.QuestStepIndex = 0;
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
            questStep.InitializeQuestStep(info.id);
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
}
