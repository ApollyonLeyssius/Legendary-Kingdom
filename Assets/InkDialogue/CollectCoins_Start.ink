=== collectCoinsStart ===
{ CollectCoinsQuestState:
- "REQUIREMENTS_NOT_MET": -> requirementsNotMet
- "CAN_START": -> canStart
- "IN_PROGRESS": -> inProgress
- "CAN_FINISH": -> canFinish
- "FINISHED":  -> finished
- else: -> END
}

= requirementsNotMet
Kanker homo jij bent fucking zwak kom terug wanneer je het waard bent!
-> END

= canStart
Ewa Zebi!!!!!
Kan je mij die hayek grote coins geven?
* [Ewa ja!]
    ~ StartQuest("CollectCoinsQuest")
    Dankjewel niffo!
* [Kifesh met jou vriend!]
    Kanker op!
- -> END

= inProgress
Kut neger pak die coins dan. Of moet ik je bossen?
Dacht ik al.
-> END

= canFinish
Breng het naar mijn niffo daar, ik ben het hem nog verschuldigd.
-> END

= finished
Kyfesh doe je hier?
-> END