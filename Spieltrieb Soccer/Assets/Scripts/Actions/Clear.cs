using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clear {

    float waitDelay = 0;

    public float AIClear(ref Player pl)
    {
        Debug.Log("clear triggered");
        return waitDelay;
    }

    public float UserClear(ref Player pl)
    {
        return waitDelay;
    }
}
