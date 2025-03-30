===questionNPC===
Will you help me? I seemed to have lost my dog #speaker: Marcus #voice: YV_Booty
*{CollectCoinsQuestState == "FINISHED"}    [Yea he's right over there!]
        Thank you my friend!
        ...
        Why did that take you so long to figure out?
        ->END
    *[Uhh Meow?]
    ->Meow
    
    *[BARK BARK BARK]
        WHAT THE HELL IS WRONG WITH YOU?
        ->END
    
->END

=Meow
Are you okay my friend?
*[Do I look okay?]
    No, No you do not
    ->END
*[I'm perfectly fine why?]
    UHHHH? MEOW????
->END