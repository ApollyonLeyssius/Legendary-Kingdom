=== collectCoinsFinish ===
{ CollectCoinsQuestState:
-"FINISHED": -> finished
- else: -> default
}

= finished
Ik ben kanker stoned.
-> END

= default
Die guy daar moet mij nog 5 euro geven
* [Niet mijn probleem neef. Ik ben tantoe skeer.]
-> END
* { CollectCoinsQuestState == "CAN_FINISH"} [Hier broeder ga maar assie kopen.]
~ FinishQuest("CollectCoinsQuest")
Ey bro dankjewel.
-> END