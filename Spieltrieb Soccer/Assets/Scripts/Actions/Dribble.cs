using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dribble{

    float waitDelay = 0;

    public float AIDribble(PlayerStatePattern pl, DecisionEntery de)
    {
        Debug.Log("Dribble triggered");
        return waitDelay;
    }

    public float UserDribble(PlayerStatePattern pl, DecisionEntery de)
    {
        return waitDelay;
    }
}
