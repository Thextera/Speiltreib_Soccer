using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWait : IPlayerState {

    private float waitDelay;
    private bool waitForTimer;
    private Rigidbody2D rb;
    //what player owns this instance of the state?
    private readonly PlayerStatePattern player;

    //construct with a readonly directive telling what AI owns this instance. could be usefull.
    public PlayerWait(PlayerStatePattern playerStatePatern)
    {
        player = playerStatePatern;
        rb = player.GetComponent<Rigidbody2D>();
    }

    //default state actions. frame by fram actions.
    public void UpdateState()
    {
        //reduce the wait time by how much time has passed since the last frame. this value is effected by timescale as slowdowns exist in this game.
        waitDelay -= Time.deltaTime * Time.timeScale;
        //TODO use of timescale could mess with durations! test this later.

        //if the delay is less than 0 never break from this. it means some external command will do it for you.
        if(waitDelay <= 0 && waitForTimer)
        {
            waitDelay = 0;
            ToPlayerMovementDecision();
        }

        //if the player is moving in the wait state stop it. (this will likely work against me later... TODO lerp to a quick stop later OR EVEN BETTER tell movement to lerp to a slow stop.)
        if(rb.velocity != Vector2.zero)
        {
            rb.velocity = Vector2.zero;
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
        waitForTimer = true;
    }

    public void WaitUntilDismissed()
    {
        waitForTimer = false;
    }

    public string ReturnNameString()
    {
        return "Wait";
    }
}
