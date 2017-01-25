using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionDecision : IPlayerState
{

    public struct DesicionEntry
    {
        public string Id;
        public float Value;

        public DesicionEntry(string id, float val)
        {
            Id = id;
            Value = val;
        }
    }

    //what player owns this instance of the state?
    private readonly PlayerStatePattern player;

    //construct with a readonly directive telling what AI owns this instance. could be usefull.
    public PlayerActionDecision(PlayerStatePattern playerStatePatern)
    {
        player = playerStatePatern;
    }

    //default state actions. frame by fram actions.
    public void UpdateState()
    {
        //find options.
        //decide if user should be involved.
        //choose option.
        //reset option list.
        //trigger action and re-run. 
        //OR
        //Change State.
    }

    //when the player gains possession of the ball trigger these actions.
    public void OnBallPossession()
    {

    }

    public void ToPlayerChaseBall()
    {

    }

    public void ToPlayerDesicion()
    {

    }

    public void ToPlayerRunAndDribble()
    {

    }

    public void ToPlayerWait()
    {

    }

    public void ToPlayerDead()
    {

    }

    /// <summary>
    /// High-cost method that calculates most possible player options in players current state.
    /// </summary>
    private void CalculateOptions()
    {
        if(player.possessesBall)
        {

        }
        else
        {

        }
    }

    private int CheckAttack()
    {
        return 0;
    }

    private int CheckPass()
    {
        return 0;
    }

    private int CheckShoot()
    {
        return 0;
    }

    private int CheckDribble()//state
    {
        return 0;
    }

    private int CheckCross()
    {
        return 0;
    }

    private int CheckSteal()
    {
        return 0;
    }

    private int CheckClear()
    {
        return 0;
    }

    private int CheckDive()
    {
        return 0;
    }

    private int CheckChaseBall()//state
    {
        return 0;
    }

}
