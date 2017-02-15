using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot {

    float waitDelay = 0;

    public float AIShoot(PlayerStatePattern pl, DecisionEntery de)
    {
        Debug.Log("shoot triggered");
        return waitDelay;
    }

    public float UserShoot(PlayerStatePattern pl, DecisionEntery de)
    {
        return waitDelay;
    }

}
