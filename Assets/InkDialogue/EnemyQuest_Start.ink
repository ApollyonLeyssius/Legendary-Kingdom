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
H-hey... Ik ben Ace Brandligt en ik woon in Zandvoort...
Betaal eerst even die man voordat je mij helpt...
-> END

= canStart
H-hey...! Ik... ik ben Ace Brandligt en k woon in Zandvoort...
Ik woon in de buurt van het station en ik hou van larpen...
Hey... die Marokkanen hebben mijn gebost...
Zou je ze voor mij kunnen slimen?
* [Ik help je wel hoor Ace Brandligt uit Zandvoort]
~StartQuest("EnemyQuest")
Dankjewel... Ik ga nu larpen!
* [Dit is niet mijn winkel vriend!]
Hoezo doe je zo?
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