using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steal{

    float waitDelay = 0;

    public float AISteal(PlayerStatePattern pl, DecisionEntery de)
    {
        Debug.Log("steal triggered by " + pl.playerStats.playerName);
        return waitDelay;
    }

    public float UserSteal(PlayerStatePattern pl, DecisionEntery de)
    {
        return waitDelay;
    }
}
