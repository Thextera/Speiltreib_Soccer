using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pass{

    float waitDelay = 0;

    public float AIPass(PlayerStatePattern pl, DecisionEntery de)
    {
        Debug.Log("pass triggered");
        return waitDelay;
    }

    public float UserPass(PlayerStatePattern pl, DecisionEntery de)
    {
        return waitDelay;
    }
}
