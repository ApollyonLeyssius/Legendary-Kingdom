using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuestLogUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private QuestLogScrollingList scrollingList;
    [SerializeField] private TextMeshProUGUI questDisplayNameText;
    [SerializeField] private TextMeshProUGUI questStatusText;
    [SerializeField] private TextMeshProUGUI goldRewardsText;
    [SerializeField] private TextMeshProUGUI experienceRewardsText;
    [SerializeField] private TextMeshProUGUI levelRequirementsText;
    [SerializeField] private TextMeshProUGUI questRequirementsText;

    private Button firstSelectedButton;

    private void OnEnable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.instance.inputEvents.onQuestLogTogglePressed += QuestLogTogglePressed;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        GameEventsManager.instance.inputEvents.onQuestLogTogglePressed -= QuestLogTogglePressed;
    }

    private void QuestLogTogglePressed()
    {
        if (contentParent.activeInHierarchy)
        {
            HideUI();
        }
        else
        {
            ShowUI();
        }
    }

    private void ShowUI() 
    {
        contentParent.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (firstSelectedButton != null)
        {
            firstSelectedButton.Select();
        }
    }

    private void HideUI()
    {
        contentParent?.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void QuestStateChange(Quest quest)
    {
       QuestLogButton questLogButton = scrollingList.CreateButtonIfNotExists(quest, () => { SetQuestLogInfo(quest); });

        if (firstSelectedButton == null)
        {
            firstSelectedButton = questLogButton.button;
        }

        questLogButton.SetState(quest.state);
    }

    private void SetQuestLogInfo(Quest quest)
    {
        questDisplayNameText.text = quest.info.displayName;

        //todo - status
        questStatusText.text = quest.GetfullStatusText();

        levelRequirementsText.text = "Level " + quest.info.levelRequirement;
        questRequirementsText.text = "";

        foreach (QuestSystemSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            questRequirementsText.text += prerequisiteQuestInfo.displayName + "\n";
        }

        goldRewardsText.text = quest.info.goldReward + " Gold";
        experienceRewardsText.text = quest.info.experienceReward + " XP";
    }
}
