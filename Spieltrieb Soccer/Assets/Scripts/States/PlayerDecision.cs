using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDecision : IPlayerState
{

    //what player owns this instance of the state?
    private readonly PlayerStatePattern player;

    //construct with a readonly directive telling what AI owns this instance. could be usefull.
    public PlayerDecision(PlayerStatePattern playerStatePatern)
    {
        player = playerStatePatern;
    }

    //default state actions. frame by fram actions.
    public void UpdateState()
    {

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
}
