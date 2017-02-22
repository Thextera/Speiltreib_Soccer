using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dribble{

    float waitDelay = 0;

    public float AIDribble(PlayerStatePattern pl, DecisionEntery de)
    {
        Debug.Log("Dribble triggered");
        //for now im going to leave this blank. The end goal is to make the player do a shorter than averge move command that adds a defensive stat bonus for its duration so players cant steal from a dribbling player.
        return waitDelay;
    }

    public float UserDribble(PlayerStatePattern pl, DecisionEntery de)
    {
        return waitDelay;
    }
}
