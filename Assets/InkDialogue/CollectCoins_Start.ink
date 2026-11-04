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
Jij bent hayek zwak! Kom terug wanneer je sterk genoeg bent! # voice:coin_start_01
-> END

= canStart
Ewa Zebi!!!!! # voice:coin_start_02
Kan je mij die hayek grote coins geven? # voice:coin_start_03
* [Ewa ja!]
    ~ StartQuest("CollectCoinsQuest")
    Dankjewel niffo! # voice:coin_start_04
* [Kifesh met jou vriend!]
    Ewa Sahbi ik heb niks aan jou! # voice:coin_start_05
- -> END

= inProgress
Oi! Pak die coins dan! Of moet ik je bossen vriend? # voice:coin_start_06
-> END

= canFinish
Breng het naar mijn niffo daar, ik ben hem nog floes verschuldigd. # voice:coin_start_07
-> END

= finished
Kifesh doe je hier?! # voice:coin_start_08
~Dialoguechanger = 1
-> END
