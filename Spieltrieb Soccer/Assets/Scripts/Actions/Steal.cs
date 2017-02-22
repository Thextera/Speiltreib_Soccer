using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steal{

    float waitDelay = 0;
    float successRoll = 0; //the final weighting that will be rolled to determine if a steal will be successfull.
    float finalRoll = 0;

    float defenderCombinedAttribute;
    float attackerCombinedAttribute;
    float calcualtedRatio;

    public float AISteal(PlayerStatePattern pl, DecisionEntery de)
    {
        Debug.Log("steal triggered by " + pl.playerStats.playerName);
        //return 0;

        attackerCombinedAttribute = pl.playerStats.speed + pl.playerStats.dribble; //attackers will be gaged by speed and dribble.
        defenderCombinedAttribute = de.target.defence + de.target.dribble; //defenders will be gaged by defence and dribble.

        calcualtedRatio = defenderCombinedAttribute / attackerCombinedAttribute;

        if(calcualtedRatio > GameManager.Instance.stealStatThreshold)
        {
            //enemy stats are too high! stealing cant work against this target.
            successRoll = 10;
        }
        else if(calcualtedRatio > GameManager.Instance.stealStatThreshold + GameManager.Instance.stealStatLeanience)
        {
            //stats are nearly tied. its a 50/50
            successRoll = 50;
        }
        else if(calcualtedRatio > GameManager.Instance.stealStatThreshold - GameManager.Instance.stealStatLeanience)
        {
            //stats are nearly tied. its a 50/50
            successRoll = 50;
        }
        else
        {
            //your stats are far supirior, success is nearly garenteed!
            successRoll = 90;
        }

        finalRoll = Random.Range(0, 100);

        if(finalRoll < successRoll)
        {
            //steal successfull!!!
            de.target.SendMessage("BallStolen", true);
            pl.ballReference.UnpossessBall();
            pl.ballReference.PossessBall(pl.gameObject);
            return waitDelay;
        }
        else
        {
            //steal failed!!! D:
            de.target.SendMessage("BallStolen",false);
            waitDelay = 1; //the penalty for failing a steal is waiting and doing nothing for a short time.
            return waitDelay;
        }

        
    }

    public float UserSteal(PlayerStatePattern pl, DecisionEntery de)
    {
        return waitDelay;
    }
}
