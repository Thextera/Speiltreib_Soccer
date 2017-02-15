using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack {

    float waitDelay = 0;

    public float AIAttack(PlayerStatePattern pl, DecisionEntery de)
    {
        Vector2 kickDirection;

        kickDirection = (de.target.transform.position - pl.transform.position);
        kickDirection.Normalize();

        kickDirection = new Vector2(kickDirection.x * pl.playerStats.attack * pl.playerStats.attackStrengthMultiplier, kickDirection.y * pl.playerStats.attack * pl.playerStats.attackStrengthMultiplier);

        GameManager.Instance.GetComponent<BallKick>().KickBall(kickDirection);
        pl.possessesBall = false;

        Debug.Log(de.target.playerName);
        Debug.Log("Attack triggered by " + pl.playerStats.playerName + " towards: " + kickDirection);
        return waitDelay;
    }

    public float UserAttack(PlayerStatePattern pl, DecisionEntery de)
    {
        return waitDelay;
    }
}
