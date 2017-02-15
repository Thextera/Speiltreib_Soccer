using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWait : IPlayerState {

    private float waitDelay;
    //what player owns this instance of the state?
    private readonly PlayerStatePattern player;

    //construct with a readonly directive telling what AI owns this instance. could be usefull.
    public PlayerWait(PlayerStatePattern playerStatePatern)
    {
        player = playerStatePatern;
    }

    //default state actions. frame by fram actions.
    public void UpdateState()
    {
        //reduce the wait time by how much time has passed since the last frame. this value is effected by timescale as slowdowns exist in this game.
        waitDelay -= Time.deltaTime * Time.timeScale;

        if(waitDelay <= 0)
        {
            waitDelay = 0;
            ToPlayerMovementDecision();
        }
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

    public void ToPlayerMovementDecision()
    {
        player.currentState = player.sPlayerMovementDecision;
    }

    public void SetCurrentWait(float wd)
    {
        waitDelay = wd;
    }

    public string ReturnNameString()
    {
        return "Wait";
    }
}
