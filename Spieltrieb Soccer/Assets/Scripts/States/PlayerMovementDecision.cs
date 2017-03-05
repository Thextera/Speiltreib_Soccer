using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementDecision : IPlayerState {


    //what player owns this instance of the state?
    private readonly PlayerStatePattern player;

    private Player ballOwner;
    private Player[] teamPlayers;
    private Player[] enemyPlayers;

    public AnimationCurve movementBiasX;
    public AnimationCurve movementBiasY;

    //reuseable variables to save on memory.
    private float distanceX;
    private float distanceY;
    private float distance;

    private int iTeam;
    private int iFoe;

    private float XMin;
    private float XMax;
    private float YMin;
    private float YMax;

    private float playerAvergeX;
    private float playerAvergeY;


    private int suitableplayercount;

    //construct with a readonly directive telling what AI owns this instance. could be usefull.
    public PlayerMovementDecision(PlayerStatePattern playerStatePatern)
    {
        player = playerStatePatern;
    }

    //default state actions. frame by fram actions.
    public void UpdateState()
    {
        teamPlayers = new Player[GameManager.Instance.GetPlayers().Length];
        enemyPlayers = new Player[GameManager.Instance.GetPlayers().Length];
        iTeam = 0;
        iFoe = 0;


        //iterate through the list of players
        foreach(Player p in GameManager.Instance.GetPlayers())
        {
            //ignore the player with the same ObjectID as myself and ignore dead players.
            if(p != null && p.GetInstanceID() != player.GetInstanceID() && p.psp.currentState != p.psp.sPlayerDead)
            {
                //does this player have the ball?
                if (p.psp.isPossessing())
                {
                    ballOwner = p;
                }

                //add player to internal team list.
                if (p.team == player.playerStats.team)
                {
                    teamPlayers[iTeam] = p;
                    iTeam++;
                }
                else
                {
                    enemyPlayers[iFoe] = p;
                    iFoe++;
                }

            }
        }

        //if a teammate has teh ball 
        if(ballOwner != null && ballOwner.team == player.playerStats.team)
        {
            //Debug.Log("team ball check");
            //move to a better position***************************************************
            GoToPosition(CalculatePosition(teamPlayers, enemyPlayers, true));
        }
        //if enemy has ball
        else if (ballOwner != null && ballOwner.team != player.playerStats.team)
        {
            //Debug.Log("foe ball check");
            //check where it is
            distance = Vector2.Distance(player.ballReference.transform.position, player.transform.position);

            //If the player is within my engage dsistance AND im an defender.
            if (distance < player.playerStats.engageDistance && player.playerStats.position == GameManager.Instance.positions["Defence"])
            {
                //if its close, move in to engage.(go to player moving to position state headed toward enemy player. that state should begin the engage desicion.)
                player.movement.stopMove();
                //use location of enemy holding ball as target.
                Vector2 EngageLocation = Field.Instance.ConvertGlobalToField(new Vector2(ballOwner.transform.position.x, ballOwner.transform.position.y));
                player.movement.MoveTo(EngageLocation);

                //change to appropreate state.
                ToPlayerMovingToPosition(EngageLocation);
            }
            else
            {
                //if its far, move to a better position***************************************
                GoToPosition(CalculatePosition(teamPlayers,enemyPlayers,false));
            }

            distance = 0;
        }
        //if noone has the ball (It must be a loose ball.)
        else
        {

            //Debug.Log("loose ball check");

            //grab my distance from the ball
            distance = Vector2.Distance(player.ballReference.transform.position, player.transform.position);
            int closerPlayers = 0;

            //compare this distance against all of the other players. 
            foreach (Player p in teamPlayers)
            {
                if (p != null)
                {
                    //if they are better suited to get the ball then by all means let them get it. (closer/already chasing)
                    if (Vector2.Distance(player.ballReference.transform.position, p.transform.position) < distance || p.psp.currentState == p.psp.sPlayerChaseBall)
                    {
                        closerPlayers++;
                    }
                }
            }

            //if we have a sufficient number of players going for the ball already ignore it. otherwise chase.
            if(closerPlayers >= GameManager.Instance.maxPlayersChasingBall)
            {
                GoToPosition(CalculatePosition(teamPlayers, enemyPlayers, true));
            }
            else
            {
                ToPlayerChaseBall();
            }
        }
        ballOwner = null;
    }

    //when the player gains possession of the ball trigger these actions.
    public void OnBallPossession()
    {

    }

    public void ToPlayerChaseBall()
    {
        player.currentState = player.sPlayerChaseBall;
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

    public void ToPlayerMovingToPosition(Vector2 destination)
    {
        //player.sPlayerMovingToPosition.SetPosition(destination);
        player.currentState = player.sPlayerMovingToPosition;
    }

    public string ReturnNameString()
    {
        return "MovementDecision";
    }

    //TODO reveiw this method and refactor/improve if needed. Its an ugly mess and i dont like it.
    /// <summary>
    /// Calculates a position on the field for a player to move to based upon the location of other players.
    /// </summary>
    /// <param name="teamPos">list of teammate locations.</param>
    /// <param name="foePos">list of enemy locations.</param>
    /// <param name="onAttack">is the move an ofenceive move going forward or defenceive retreat?</param>
    private Vector2 CalculatePosition(Player[] team, Player[] foe, bool onAttack)
    {
        float posX = CalculatePosX(team, foe, onAttack);
        float posY = CalculatePosY(team, foe);

        return new Vector2(posX, posY);
        /*
         
        //x is the distance parallel to the goals, y is the distance perpendicular to the goals.
        XMin = 0;
        XMax = 100;
        YMin = 0;
        YMax = 100;

        //grab the averged coordinates of teammates.
        foreach(Player p in team)
        {
            if (p != null)
            {
                playerAvergeX += Field.Instance.ConvertGlobalToField(new Vector2(p.transform.position.x, p.transform.position.y)).x;
                playerAvergeY += Field.Instance.ConvertGlobalToField(new Vector2(p.transform.position.x, p.transform.position.y)).y;
            }
        }
        playerAvergeX /= team.Length;
        playerAvergeY /= team.Length;

 
            if(player.playerStats.position == GameManager.Instance.positions["Forward"])
            {
                if(onAttack)
                {
                    //if your a forward on attack
                    if(playerAvergeX < 60)
                    {
                        //60 - 100
                        XMin = 60;
                    }
                    else
                    {
                        //avrg - 100
                        XMin = playerAvergeX;
                    }

                    if(playerAvergeY > 50)
                    {
                        //50-100
                        YMin = 50;
                    }
                    else
                    {
                        //0 - 50
                        YMax = 50;
                    }
                }
                else //on attack
                {
                    //if your a forward on defence
                    XMin = 30;
                    XMax = 65;

                    if (playerAvergeY > 50)
                    {
                        //50-100
                        YMin = 50;
                    }
                    else
                    {
                        //0 - 50
                        YMax = 50;
                    }
            }
            }
            else if(player.playerStats.position == GameManager.Instance.positions["Defence"])
            {
                if (onAttack)
                {
                    XMin = 30;
                    XMax = 65;

                    //if your a defender on attack
                    if (playerAvergeY > 50)
                    {
                        //50-100
                        YMin = 50;
                    }
                    else
                    {
                        //0 - 50
                        YMax = 50;
                    }
            }
                else //on attack
                {

                    //if your a defender on defence
                    if (playerAvergeX > 40)
                    {
                        //60 - 100
                        XMax = 40;
                    }
                    else
                    {
                        //avrg - 100
                        XMax = playerAvergeX;
                    }

                    if (playerAvergeY > 50)
                    {
                        //50-100
                        YMin = 50;
                    }
                    else
                    {
                        //0 - 50
                        YMax = 50;
                    }
                }
            }
    
       //If the players are shooting for the opposite net then reverse the values.
        if (player.playerStats.team == GameManager.Instance.teams["Left"])
        {
            XMin = 100 - XMin;
            XMax = 100 - XMax;
            YMin = 100 - YMin;
            YMax = 100 - YMax;
        }


        Vector2 rv = Vector2.zero;
        rv.x = Random.Range(XMin, XMax);
        rv.y = Random.Range(YMin, YMax);

        return rv;
        */

        

    }

    private float CalculatePosX(Player[] team, Player[] foe, bool onAttack)
    {
        float posX = 0;
        //must consider the net the players are shooting on.
        //does not consider clumping much as this would involve a psudo midfeild position... 
        //If the player wants to let 5 attckers cluster up for an attack so be it.

        //TODO consider extra clump prevention methods such as using a player position averge.

        //consider if ball is on attacking side or defending side. (onAttack bool)
        if(onAttack)//attack
        {
            if (player.playerStats.position == GameManager.Instance.positions["Forward"])
            {
                //attacker on attack (random within forward third)
                posX = UnityEngine.Random.Range(66,99);
            }
            else
            {
                //defender on attack.(random within middle third)
                posX = UnityEngine.Random.Range(33, 66);
            }
        }
        else//fall back
        {
            if (player.playerStats.position == GameManager.Instance.positions["Forward"])
            {
                //attacker on defence. (random within middle third)
                //Debug.Log("Attacker on Def: " + player.playerStats.name);
                posX = UnityEngine.Random.Range(33, 66);
            }
            else
            {
                //defender on defence. (random within rear third)
                posX = UnityEngine.Random.Range(1, 33);
            }
        }

        //check team 
        if (player.playerStats.team == GameManager.Instance.teams["Left"])
        {
            //if team is left invert the value.
            posX = 100 - posX;
        }

        return posX;
    }

    private float CalculatePosY(Player[] team, Player[] foe)
    {
        //ignores which net the players are shooting on.

        int plTop = 0; //number of players found to be on the top half of the feild.
        int plBot = 0; //number of players found to be on the bottom half of the feild.
        float posY = 0;

        //look at team roster. count players on top and players on bottom.
        foreach(Player p in team)
        {
            //make sure player isnt null and exclude myself.
            if(p != null && p.gameObject.GetInstanceID() != player.gameObject.GetInstanceID() /* && p.position == player.playerStats.position */)
            {
                //check the location of each player,
                if(Field.Instance.ConvertGlobalToField(p.gameObject.transform.position).y > 50)
                {
                   //if they are above the center line
                   plTop++;
                }
                else
                {
                    //if they are below the center line.
                    plBot++;
                }
            }
        }

        //situations
        //if even attemtp to random closer to center.
        if(plTop + plBot == 0)
        {
            //center random
            posY = UnityEngine.Random.Range(25, 75);
        }
        else if(plTop > plBot)
        {
            //bot random
            posY = UnityEngine.Random.Range(1, 50);
        }
        else if(plTop < plBot)
        {
            //top random
            posY = UnityEngine.Random.Range(50, 99);
        }
        else if(plTop == plBot)
        {
            //center random
            posY = UnityEngine.Random.Range(25, 75);
        }
        else
        {
            //should be unreachable.
            //default to the same random choice as if its a tie.
            //center random
            Debug.LogWarning("Vertical movement went wrong. defaulting to moving to center.");
            posY = UnityEngine.Random.Range(25, 75);
        }

        return posY;
    }

    /// <summary>
    /// Execute movement based on calculated position
    /// </summary>
    private void GoToPosition(Vector2 position)
    {
        //movement check, if the ball is within a certain range of me then stop moving and wait. 
        float d = Vector2.Distance(player.transform.position, player.ballReference.transform.position);
        if (d > player.playerStats.targetMidRange && d < player.playerStats.targetLongRange)
        {
            player.currentState = player.sPlayerWait;
            player.sPlayerWait.SetCurrentWait(0.1f);
            return;
        }

        //Debug.Log(position);
        player.movement.stopMove();
        player.movement.MoveTo(position);
        player.currentState = player.sPlayerMovingToPosition;
        //player.sPlayerMovingToPosition.SetPosition(position);
    }

}


