﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChaseBall : IPlayerState {

    int fcount = 9; //calls the MoveTo command every x frames.
    Vector2 ballLocation;
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
            if(player.possessesBall)
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
        distanceFromBallX = player.transform.position.x - player.ballReference.transform.position.x;
        distanceFromBallY = player.transform.position.y - player.ballReference.transform.position.y;

        //if the distance from the ball is small enough then possess the ball.
        if (Mathf.Abs(distanceFromBallX) < player.possessionRange && Mathf.Abs(distanceFromBallY) < player.possessionRange && !player.ballReference.IsPossessed())
        {
            player.possessesBall = true;
            player.ballReference.PossessBall(player.gameObject);
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
        Debug.Log("Left Chase State");
    }

    public void ToActionDesicion()
    {
        player.currentState = player.sPlayerActionDecision;
    }
}
