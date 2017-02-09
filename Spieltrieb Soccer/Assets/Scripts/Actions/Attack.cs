using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack {

    float waitDelay = 0;

    public float AIAttack(ref Player pl)
    {
        Debug.Log("Attack triggered");
        return waitDelay;
    }

    public float UserAttack(ref Player pl)
    {
        return waitDelay;
    }
}
