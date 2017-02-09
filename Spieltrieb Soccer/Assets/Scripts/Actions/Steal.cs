using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steal{

    float waitDelay = 0;

    public float AISteal(ref Player pl)
    {
        Debug.Log("steal triggered");
        return waitDelay;
    }

    public float UserSteal(ref Player pl)
    {
        return waitDelay;
    }
}
