using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross{

    float waitDelay = 0;

    public float AICross(ref Player pl)
    {
        Debug.Log("cross triggered");
        return waitDelay;
    }

    public float UserCross(ref Player pl)
    {
        return waitDelay;
    }
}
