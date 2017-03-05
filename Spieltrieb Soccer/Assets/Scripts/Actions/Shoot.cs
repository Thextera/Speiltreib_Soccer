using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot {

    float waitDelay = 0.66f;

    public float AIShoot(PlayerStatePattern pl, DecisionEntery de)
    {
        //create the direction we want to shoot the ball.
        Vector2 kickDirection = Vector2.zero;

        //check the goal zones.
        foreach (GoalZone gz in Field.Instance.gz)
        {
            //if the goal zone is not our own then create a vector between it an the player.
            if (gz.team != pl.playerStats.team)
            {
                kickDirection = (gz.transform.position - pl.transform.position);
            }
        }

        //normalize the kick direction and remove its Y component. we want to make it cross feild, not anything else.
        kickDirection.Normalize();
        kickDirection = new Vector2(kickDirection.x * pl.playerStats.shoot * pl.playerStats.attackStrengthMultiplier, kickDirection.y * pl.playerStats.shoot * pl.playerStats.attackStrengthMultiplier);

        //finally shoot the ball when we are ready!
        GameManager.Instance.GetComponent<BallKick>().KickBall(kickDirection);
        pl.dePossessBall();

        Debug.Log("shoot triggered");
        return waitDelay;
    }

    public float UserShoot(PlayerStatePattern pl, DecisionEntery de)
    {
        return waitDelay;
    }

}
