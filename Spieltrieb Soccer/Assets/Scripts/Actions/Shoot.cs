using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot {

    float waitDelay = 0;

    public float AIShoot(ref Player pl)
    {
        Debug.Log("shoot triggered");
        return waitDelay;
    }

    public float UserShoot(ref Player pl)
    {
        return waitDelay;
    }

}
