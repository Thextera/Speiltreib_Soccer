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
                if (p.psp.possessesBall)
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
            //move to a better position***************************************************
            GoToPosition(CalculatePosition(teamPlayers, enemyPlayers, true));
        }
        //if enemy has ball
        else if (ballOwner != null && ballOwner.team != player.playerStats.team)
        {
            //check where it is
            distanceX = player.ballReference.transform.position.x - player.transform.position.x;
            distanceY = player.ballReference.transform.position.y - player.transform.position.y;

            if (distanceX < player.playerStats.engageDistance && distanceY < player.playerStats.engageDistance)
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

            distanceX = 0;
            distanceY = 0;
        }
        //if noone has the ball (It must be a loose ball.)
        else
        {
            distanceX = player.ballReference.transform.position.x - player.transform.position.x;
            distanceY = player.ballReference.transform.position.y - player.transform.position.y;

            //if i am close then check for other players chasing / closer.
            if (distanceX < player.playerStats.ballEngageDistance && distanceY < player.playerStats.ballEngageDistance)
            {
                // check who is close to the ball and if anyone is already chasing
                foreach (Player p in teamPlayers)
                {
                    if (p != null)
                    {
                        distanceX = 0;
                        distanceY = 0;

                        distanceX = player.ballReference.transform.position.x - p.transform.position.x;
                        distanceY = player.ballReference.transform.position.y - p.transform.position.y;
                        if (distanceY < p.ballEngageDistance && distanceX < p.ballEngageDistance && p.psp.currentState == p.psp.sPlayerChaseBall)
                        {
                            //add all players that are chasing the ball or very close to it here.
                            suitableplayercount++;
                        }
                    }
                }
                //TODO change this rule. it is currently defaulted to forcing 2 AIs to chase however setting it to the below would work more dynamically.
                //3 or less attackers, 2 people chase. 4 or more attackers, 3 people chase.
                if (suitableplayercount < 2)
                {
                    Debug.Log("Movement decided to chase." + suitableplayercount + teamPlayers);
                    ToPlayerChaseBall();
                }
                suitableplayercount = 0;
            }
            else
            {
                //If i am not close then move to a better position.*************************************
                GoToPosition(CalculatePosition(teamPlayers, enemyPlayers, true));

            }

            distanceX = 0;
            distanceY = 0;
        }

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
        player.sPlayerMovingToPosition.SetPosition(destination);
        player.currentState = player.sPlayerMovingToPosition;
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
    }

    /// <summary>
    /// Execute movement based on calculated position
    /// </summary>
    private void GoToPosition(Vector2 position)
    {
        //Debug.Log(position);
        player.movement.stopMove();
        player.movement.MoveTo(position);
        player.currentState = player.sPlayerMovingToPosition;
        player.sPlayerMovingToPosition.SetPosition(position);
    }

}


