=== enemyQuestStart ===
{ EnemyQuestState:
- "REQUIREMENTS_NOT_MET": -> requirementsNotMet
- "CAN_START": -> canStart
- "IN_PROGRESS": -> inProgress
- "CAN_FINISH": -> canFinish
- "FINISHED":  -> finished
- else: -> END
}

= requirementsNotMet
H-hey... Ik ben Ace Brandligt and ik woon in Zandvoort...
Betaal eerst even die man voordat je mij helpt...
-> END

= canStart
Hey... die Marokkanen hebben mijn gebost...
Zou je ze voor mij kunnen slimen?
* [Ik help je wel hoor Ace Brandligt uit Zandvoort]
~StartQuest("EnemyQuest")
Dankjewel... Ik ga nu larpen!
* [Dit is niet mijn winkel vriend!]
Fuck jou man! Tyfus lijer!
- -> END

= inProgress
Wat doe je nou?!
Slime ze uit! Pop ze! Maakt niet uit hoe je het doet maar maak ze dood!
-> END

= canFinish
Dankjewel broeder, ik hou van je!
Ik ben je eeuwig dankbaar en ga je voor de komende jaren glazen.
Wil je straight teeth?
~FinishQuest("EnemyQuest")
-> END

= finished
Je bent mijn goat!
-> END