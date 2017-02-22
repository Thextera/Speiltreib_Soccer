using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovingToPosition : IPlayerState {

    //private Vector2 destination;
    //private float distance;
    //private float minDistance = 2;

    //what player owns this instance of the state?
    private readonly PlayerStatePattern player;

    //construct with a readonly directive telling what AI owns this instance. could be usefull.
    public PlayerMovingToPosition(PlayerStatePattern playerStatePatern)
    {
        player = playerStatePatern;
    }

    //default state actions. frame by fram actions.
    public void UpdateState()
    {
        if(!player.movement.movingToDestination)
        {
            player.currentState = player.sPlayerMovementDecision;
        }
        //Debug.Log(destination + " + " + Field.Instance.ConvertGlobalToField(player.transform.position));
        ////if somehow this state was entered without informing it of a destination then default to an action desicion.
        //if(destination != null)
        //{
        //    //find the distance to our destination
        //    distance = Vector2.Distance(Field.Instance.ConvertGlobalToField(player.transform.position),destination);
        //
        //    //if we are close to that destination wait for a moment then make a desicion.
        //    if(distance <= minDistance)
        //    {
        //        player.currentState = player.sPlayerActionDecision;
        //    }
        //}
        //else
        //{
        //    player.currentState = player.sPlayerActionDecision;
        //}
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

    public void SetPosition(Vector2 pos)
    {
        //destination = pos;
    }

    public string ReturnNameString()
    {
        return "MovingToPosition";
    }
}

