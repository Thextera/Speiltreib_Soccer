using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dribble{

    float waitDelay = 0;

    public float AIDribble(ref Player pl)
    {
        Debug.Log("Dribble triggered");
        return waitDelay;
    }

    public float UserDribble(ref Player pl)
    {
        return waitDelay;
    }
}
