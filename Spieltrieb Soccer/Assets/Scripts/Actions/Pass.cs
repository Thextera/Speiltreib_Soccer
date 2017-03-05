﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pass{

    float waitDelay = 0.75f;

    public float AIPass(PlayerStatePattern pl, DecisionEntery de)
    {
        Vector2 kickDirection;

        kickDirection = (de.target.transform.position - pl.transform.position);
        kickDirection.Normalize();

        kickDirection = new Vector2(kickDirection.x * pl.playerStats.pass * pl.playerStats.attackStrengthMultiplier, kickDirection.y * pl.playerStats.pass * pl.playerStats.attackStrengthMultiplier);

        GameManager.Instance.GetComponent<BallKick>().KickBall(kickDirection);
        de.target.SendMessage("IncomingPass");
        pl.dePossessBall();

        Debug.Log("pass triggered");
        return waitDelay;
    }

    public float UserPass(PlayerStatePattern pl, DecisionEntery de)
    {
        return waitDelay;
    }
}
