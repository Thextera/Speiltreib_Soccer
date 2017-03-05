using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross{

    float waitDelay = 0.1f;

    public float AICross(PlayerStatePattern pl, DecisionEntery de)
    {
        Debug.Log("cross triggered");
        return waitDelay;
    }

    public float UserCross(PlayerStatePattern pl, DecisionEntery de)
    {
        return waitDelay;
    }
}
