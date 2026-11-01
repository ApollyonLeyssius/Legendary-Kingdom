VAR Dialoguechanger = 0

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
Jij bent hayek zwak! Kom terug wanneer je sterk genoeg bent!
-> END

= canStart
Ewa Zebi!!!!!
Kan je mij die hayek grote coins geven?
* [Ewa ja!]
    ~ StartQuest("CollectCoinsQuest")
    Dankjewel niffo!
* [Kifesh met jou vriend!]
    Ik heb ook niks aan jou!
- -> END

= inProgress
Oi! Pak die coins dan! Of moet ik je bossen?
Dacht ik al.
-> END

= canFinish
Breng het naar mijn niffo daar, ik ben het hem nog verschuldigd.
-> END

= finished
Kifesh doe je hier?!
~Dialoguechanger = 1
-> END
