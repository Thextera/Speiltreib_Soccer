using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pass{

    float waitDelay = 0;

    public float AIPass(ref Player pl)
    {
        Debug.Log("pass triggered");
        return waitDelay;
    }

    public float UserPass(ref Player pl)
    {
        return waitDelay;
    }
}
