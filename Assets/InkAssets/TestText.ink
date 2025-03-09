VAR questCompleted = false

-> questionNPC

===questionNPC===
Will you help me? I seemed to have lost my dog
    +[Uhh Meow?]
        Are you ok my friend?
        ->DONE
    
    +[BARK BARK BARK]
        WHAT THE HELL IS WRONG WITH YOU?
        ->DONE
    
    +[Yea he's right over there!]
        Thank you my friend!
        ->DONE
->END

===giverNPC===
{questCompleted:
Hell yea dude, you did it, I have another quest to give you!
->DONE
-else:
Come back once you've helped my friend
->DONE
}

    
