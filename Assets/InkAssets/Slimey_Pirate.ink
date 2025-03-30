===basicNPC===

{CollectCoinsQuestState: 
    -"REQUIREMENTS_NOT_MET": ->requirementsNotMet 
    -"CAN_START": ->canStart
    -"IN_PROGRESS": ->inProgress
    -"CAN_FINISH":->canFinish   
    -"FINISHED":->finished
    -else: ->END
    }

=requirementsNotMet
Come back when your smarter and i'll teach about the P mechanic #speaker: Gravy Jones #voice: YV_Arr
->END

=canStart
I need you to hit the "P" key five times for me #speaker: Gravy Jones #voice: YV_Arr
*[Uhh okay?]
    Hip Hip Horray!  
    ~StartQuest("CollectCoinsQuest")
    ->END
*[Nah i'm good]
    AHHHHHHHH! Come back later then
    
-   ->END

=inProgress
Huzzah, I'm so excited to see what your able to accomplish once P has been hit 5 times! #speaker: Gravy Jones #voice: YV_Arr
->END

=canFinish
YOU DID IT, YOU MAD LAD, YOU ACTUALLY HIT THE BUTTON 5 TIMES, YOU ABSOULTE IDIOT, NOW I CAN DOMINATE THE WORLD #speaker: Gravy Jones #voice: YV_Arr
NEXT TIME DON'T LISTEN TO WHAT A SLIMEY PIRATE TELLS YOU!
~FinishQuest("CollectCoinsQuest")
->END

=finished
Thanks for letting me abolish the hierarchy #speaker: Gravy Jones #voice: YV_Arr
->END


