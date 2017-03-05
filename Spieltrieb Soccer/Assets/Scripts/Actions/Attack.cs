using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack {

    float waitDelay = 0.45f;

    public float AIAttack(PlayerStatePattern pl, DecisionEntery de)
    {
        //set up the direction of the attack.
        Vector2 kickDirection;

        //base it off of the position of both players then normalize to only get the direction.
        kickDirection = (de.target.transform.position - pl.transform.position);
        kickDirection.Normalize();

        //take the direction and factor in player attack stats.
        kickDirection = new Vector2(kickDirection.x * pl.playerStats.attack * pl.playerStats.attackStrengthMultiplier * 4, kickDirection.y * pl.playerStats.attack * pl.playerStats.attackStrengthMultiplier);

        //FIRE IN THE HOLE!
        GameManager.Instance.GetComponent<BallKick>().KickBall(kickDirection);
        pl.dePossessBall();

        Debug.Log("Attack triggered by " + pl.playerStats.playerName + " towards: " + kickDirection);
        return waitDelay;
    }

    public float UserAttack(PlayerStatePattern pl, DecisionEntery de)
    {
        return waitDelay;
    }
}
