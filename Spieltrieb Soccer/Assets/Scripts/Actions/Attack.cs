using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack {

    float waitDelay = 0.45f;

    public float AIAttack(PlayerStatePattern pl, DecisionEntery de)
    {
        //if desicion entery's special string isnt null then trigger the special.
        //the attack manager will then handle the rest.
        if (de.special != null)
        {
            AttackManager.Instance.ExecuteAbility(de.special, pl.playerStats);
            new Task(CalculateAttack(pl, de, true));
        }
        else
        {
            new Task(CalculateAttack(pl, de, false));
        }

        return waitDelay;
    }

    public IEnumerator CalculateAttack(PlayerStatePattern pl, DecisionEntery de, bool wait)
    {
        if(wait)
        {
            yield return new WaitForSeconds(GameManager.Instance.universalAnimationDuration);
        }

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

        Debug.Log("Attack Target is: " + de.target);
        Debug.Log("Attack triggered by " + pl.playerStats.playerName + " towards: " + kickDirection);

        yield return null;
    }

    public float UserAttack(PlayerStatePattern pl, DecisionEntery de)
    {
        return waitDelay;
    }
}
