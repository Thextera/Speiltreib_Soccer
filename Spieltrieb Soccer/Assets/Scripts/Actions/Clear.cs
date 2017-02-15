using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clear {

    float waitDelay = 0;

    public float AIClear(PlayerStatePattern pl, DecisionEntery de)
    {
        Debug.Log("clear triggered");
        return waitDelay;
    }

    public float UserClear(PlayerStatePattern pl, DecisionEntery de)
    {
        return waitDelay;
    }
}
