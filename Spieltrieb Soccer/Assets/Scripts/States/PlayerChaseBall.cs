using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChaseBall : IPlayerState {

    int fcount = 9; //calls the MoveTo command every x frames.
    Vector2 ballLocation;
    float distanceFromBall;
    float distanceFromBallX;
    float distanceFromBallY;

    //what player owns this instance of the state?
    private readonly PlayerStatePattern player;

    //construct with a readonly directive telling what AI owns this instance. could be usefull.
    public PlayerChaseBall(PlayerStatePattern playerStatePatern)
    {
        player = playerStatePatern;
    }

    //default state actions. frame by fram actions.
    public void UpdateState()
    {
        //if the ball has an owner dont bother chasing it anymore.
        if(player.ballReference.IsPossessed())
        {
            //if i have the ball decide what to do with it
            if(player.isPossessing())
            {
                ToActionDesicion();
            }
            else //if anyone else has the ball move to a better position.
            {
                ToMovementDesicion();
            }
        }

        fcount++;
        if(fcount >= 10)
        {
            //grabs the location of the ball then tells the player to move to that location.
            ballLocation = new Vector2(player.ballReference.transform.position.x, player.ballReference.transform.position.y);
            player.movement.stopMove();

            player.movement.MoveTo(Field.Instance.ConvertGlobalToField(ballLocation));
            //reset our frame counter.
            fcount = 0;
        }

        //check the distance from the ball.

        distanceFromBall = Vector2.Distance(player.transform.position, player.ballReference.transform.position);

        //if the distance from the ball is small enough then possess the ball.
        if (distanceFromBall < player.possessionRange  && !player.ballReference.IsPossessed())
        {
            player.possessBall();
            player.ballReference.PossessBall(player.gameObject);
        }
        else if(distanceFromBall > player.playerStats.targetMidRange)//if the ball is too far away break back to default movement.
        {
            ToMovementDesicion();
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

    public string ReturnNameString()
    {
        return "ChaseBall";
    }

    public void ToMovementDesicion()
    {
        player.currentState = player.sPlayerMovementDecision;
        Debug.Log(player.playerStats.playerName + " Left Chase State");
    }

    public void ToActionDesicion()
    {
        player.currentState = player.sPlayerActionDecision;
    }
}
