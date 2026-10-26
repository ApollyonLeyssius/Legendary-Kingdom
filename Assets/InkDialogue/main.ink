EXTERNAL StartQuest(questId)
EXTERNAL AdvanceQuest(questId)
EXTERNAL FinishQuest(questId)

//Quest names (questId + "Id" for variable name) 
VAR CollectCoinsQuestId = "CollectCoinsQuest"
VAR EnemyQuestQuestId = "EnemyQuestQuest"

// quest states (quest id + "State" for variable name)
VAR CollectCoinsQuestState = "REQUIREMENTS_NOT_MET"
VAR EnemyQuestState = "REQUIREMENTS_NOT_MET"

INCLUDE CollectCoins_Start.ink
INCLUDE CollectCoinQuest_Finish.ink
INCLUDE EnemyQuest_Start.ink