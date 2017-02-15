﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionDecision : IPlayerState
{
    public DecisionEntery[] options;
    public DecisionEntery[] forwardOptions;
    public DecisionEntery[] defenceOptions;
    public DecisionEntery chosenOption;
    public Player[] team;
    public Player[] foe;

    private LayerMask fieldMask;
    private LayerMask playerMask;
    //private LayerMask BallMask;

    private Attack attack;
    private Shoot shoot;
    private Dribble dribble;
    private Cross cross;
    private Clear clear;
    private Pass Pass;
    private Steal steal;

    //reusable values. be sure to clear before using.
    private int i;
    private int j;
    private Player pl;

    private float f;
    private float n;

    private bool teamsArentSet; //a value that allows the system to set who is on what team once and once only.
    private float waitDelay;

    //shoot pass attack.
    //steal pass attack.
    //goalie?

    //what player owns this instance of the state?
    private readonly PlayerStatePattern player;

    //construct with a readonly directive telling what AI owns this instance. could be usefull.
    public PlayerActionDecision(PlayerStatePattern playerStatePatern)
    {
        player = playerStatePatern;

        attack = new Attack();
        steal = new Steal();
        Pass = new Pass();
        dribble = new Dribble();
        cross = new Cross();
        clear = new Clear();
        shoot = new Shoot();

        fieldMask = Field.Instance.gameObject.layer;
        playerMask = player.gameObject.layer;
        //BallMask = player.ballReference.gameObject.layer;

        options = new DecisionEntery[10];
        defenceOptions = new DecisionEntery[options.Length];
        forwardOptions = new DecisionEntery[options.Length];

        teamsArentSet = true;
    }

    //default state actions. frame by fram actions.
    public void UpdateState()
    {
        //make sure all shared values are cleared at the beginning of operations. just as a precaution.
        ClearAll();
        //make sure team arrays are populated for future use.
        if(teamsArentSet)
        {
            //quickly set our team arrays.
            setTeamArrays();
            teamsArentSet = false;
        }
        //weigh options.
        CalculateOptions(options);
        i = 0;
        j = 0;
        //narrow options based on if user is attack or defence.
        foreach (DecisionEntery de in options)
        {
            if (de != null)
            {
                if (de.decisionPosition == 0 || de.decisionPosition == GameManager.Instance.positions["Defence"])
                {
                    defenceOptions[i] = de;
                    i++;
                }
                else if (de.decisionPosition == 0 || de.decisionPosition == GameManager.Instance.positions["Forward"])
                {
                    forwardOptions[j] = de;
                    j++;
                }
            }

        }

        //decide if user should be involved.******************************************************************************************************
        //TODO

        //choose option.
        if(player.playerStats.position == GameManager.Instance.positions["Forward"]) //if im a forward
        {
            //iterate through all forward options
            foreach(DecisionEntery de in forwardOptions)
            {
                if (de != null)
                {
                    //select the highest weighted option 
                    if (chosenOption == null || chosenOption.weight < de.weight)
                    {
                        chosenOption = de;
                    }
                    else if (chosenOption != null && chosenOption.weight == de.weight)//tiebreaker
                    {
                        //do a 50/50 to see which option to choose.
                        //TODO be more deliberate with this. 50/50 isnt acceptable.
                        int rand = Random.Range(0, 1);
                        if (rand == 1)
                        {
                            Debug.Log("TIE WAS BROKEN");
                            chosenOption = de;
                        }
                    }
                }
            }
        }
        else //if im a defender
        {
            //iterate through all defender options
            foreach (DecisionEntery de in defenceOptions)
            {
                if (de != null)
                {
                    //select the highest weighted option (ignore empty array enteries.)
                    if (chosenOption == null || de.weight > chosenOption.weight)
                    {
                        chosenOption = de;
                    }
                    else if (chosenOption != null && chosenOption.weight == de.weight)//tiebreaker
                    {
                        //do a 50/50 to see which option to choose.
                        //TODO be more deliberate with this. 50/50 isnt acceptable.
                        int rand = Random.Range(0, 1);
                        if (rand == 1)
                        {
                            chosenOption = de;
                            Debug.Log("TIE WAS BROKEN");
                        }
                    }
                }
            }
        }

        if (chosenOption != null)//if it skips this code then no option was seen. this is more than likely an error.
        {

            //call action here
            //I WOULDVE RATHERED USE A SWITCH STATEMENT BUT I GUESS C# DOESNT LIKE USING THOSE WITH DICTIONARIES >.> #sass
            if (chosenOption.name == GameManager.Instance.AIActions["Attack"])
            {
                waitDelay = attack.AIAttack(player, chosenOption);
            }
            else if (chosenOption.name == GameManager.Instance.AIActions["Pass"])
            {
                waitDelay = Pass.AIPass(player, chosenOption);
            }
            else if (chosenOption.name == GameManager.Instance.AIActions["Shoot"])
            {
                waitDelay = shoot.AIShoot(player, chosenOption);
            }
            else if (chosenOption.name == GameManager.Instance.AIActions["Dribble"])
            {
                waitDelay = dribble.AIDribble(player, chosenOption);
            }
            else if (chosenOption.name == GameManager.Instance.AIActions["Cross"])
            {
                waitDelay = cross.AICross(player, chosenOption);
            }
            else if (chosenOption.name == GameManager.Instance.AIActions["Clear"])
            {
                waitDelay = clear.AIClear(player, chosenOption);
            }
            else if (chosenOption.name == GameManager.Instance.AIActions["Steal"])
            {
                waitDelay = steal.AISteal(player, chosenOption);
            }
            else if (chosenOption.name == GameManager.Instance.AIActions["NotOpen"])
            {
                Debug.Log(player.playerStats.playerName + " has decided he is open");
            }
            else if (chosenOption.name == GameManager.Instance.AIActions["Dive"])
            {

            }
            else
            {
                Debug.LogError("Something went wrong when choosing an action. Chosen option name is invalid. Please check chosen option name.");
            }

        }

        //reset option list.
        options = new DecisionEntery[options.Length];
        forwardOptions = new DecisionEntery[forwardOptions.Length];
        defenceOptions = new DecisionEntery[defenceOptions.Length];

        //move to wait state.
        ToPlayerWait(waitDelay);
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

    public string ReturnNameString()
    {
        return "ActionDesicion";
    }

    public void ToPlayerWait(float wd)
    {
        //set the delay on the wait then engage the state.
        player.sPlayerWait.SetCurrentWait(wd);
        player.currentState = player.sPlayerWait;
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
    private DecisionEntery[] CalculateOptions(DecisionEntery[] deList)
    {
        int index = 0;
        //if the player has the ball dont even bother checking the options that involve the ball.
        if(player.possessesBall)
        {
            deList[index] = CheckAttack();
            index++;
            deList[index] = CheckPass();
            index++;
            deList[index] = CheckShoot();
            index++;
            deList[index] = CheckDribble();
            index++;
            deList[index] = CheckCross();
            index++;
            deList[index] = CheckClear();
            index++;
        }
        else//i dont have the ball
        {
            deList[index] = CheckSteal();
            index++;
            deList[index] = CheckOpen();
            index++;
            deList[index] = CheckDive();
            index++;
        }

        return deList;
    }




    private DecisionEntery CheckAttack()//b
    {
        DecisionEntery de = new DecisionEntery(GameManager.Instance.AIActions["Attack"]);
        pl = null;
        i = 0;
        n = 0;
        //based on distance from foes. (favor mid attacks.)
        foreach (Player p in foe)
        {
            if (p != null)
            {
                //find the distance between me and the target.(med range)
                f = Mathf.Abs(Vector2.Distance(player.transform.position, p.transform.position));

                //if the current number is closer to 1 than the previous AND the player has a line of sight to the target then save the pair.
                if (n == 0 || f - player.playerStats.targetMidRange < n - player.playerStats.targetMidRange)
                {
                    //TODO this favors the closest player...
                    n = f;
                    pl = p;
                }
            }
        }

        i = CreateWeightingFromDistance(player.playerStats.targetMidRange, n, i);

        de.target = pl;
        de.weight = i;
        return de;
    }


    private DecisionEntery CheckPass()//b
    {
        DecisionEntery de = new DecisionEntery(GameManager.Instance.AIActions["Pass"]);
        pl = null;
        i = 0;
        n = 0;
        //based on distance from teammates. (favor mid passes)
        foreach (Player p in team)
        {
            //Ignore nulls in the array and me. because I dont want to pass to me.
            if (p != null && p.gameObject.GetInstanceID() != player.gameObject.GetInstanceID())
            {
                //find the distance between me and the pass target.(med range)
                f = Mathf.Abs(Vector2.Distance(player.transform.position, p.transform.position));

                
                Debug.DrawLine(player.transform.position, p.transform.position, Color.red, 0.75f);

                //TODO something is wrong with my use of variables here... figure it out later.
                //if the current number is closer to 1 than the previous 
                if ((n == 0 || f - player.playerStats.targetMidRange < n - player.playerStats.targetMidRange))
                {
                    //Check line of sight. 
                    if(!CheckLine(player.transform.position, p.transform.position, p.gameObject, playerMask))
                    { 
                        //TODO this favors the closest player...
                        n = f;
                        pl = p;
                    }
                }
            }
        }

        i = CreateWeightingFromDistance(player.playerStats.targetMidRange, n, i);

        de.target = pl;
        de.weight = i;
        de.decisionPosition = GameManager.Instance.positions["Forward"];
        return de;
    }

    private DecisionEntery CheckShoot()//b
    {
        DecisionEntery de = new DecisionEntery(GameManager.Instance.AIActions["Shoot"]);
        i = 0;
        n = 0;
        Vector2 v ;

        //based on distance from net (favor mid)
        foreach(GoalZone gz in Field.Instance.gz)
        {
            if(player.playerStats.team != gz.team)
            {

                //create a position representing the center of the net.
                v = new Vector2(gz.goalPostOne.x, gz.goalPostTwo.y + Vector2.Distance(gz.goalPostOne, gz.goalPostTwo)/2);
                //calc distanc between me and the center of the net.
                n = Vector2.Distance(player.transform.position, v);
                //create desicion base weighting from that weight.
                i = CreateWeightingFromDistance(player.playerStats.targetMidRange, n, i);

                //based on raycasts to net. (checks center of net and both posts.)
                //if ray is obstructed reduce weighting by an ammount.
                Debug.DrawLine(player.transform.position, gz.goalPostOne, Color.cyan, 0.75f);
                Debug.DrawLine(player.transform.position, gz.goalPostTwo, Color.magenta, 0.75f);
                Debug.DrawLine(player.transform.position, v, Color.yellow, 0.75f);


                if(CheckLine(player.transform.position, gz.goalPostOne, playerMask))
                {
                    //lower shot weight if a hit was detected.
                    i -= 10;
                }

                if (CheckLine(player.transform.position, gz.goalPostTwo, playerMask))
                {
                    //lower shot weight if a hit was detected.
                    i -= 10;
                }

                if (CheckLine(player.transform.position, v, playerMask))
                {
                    //lower shot weight if a hit was detected.
                    i -= 10;
                }
            }
        }

        if(i < 1)
        {
            i = 0;
        }
        de.decisionPosition = GameManager.Instance.positions["Forward"];
        de.weight = i;
        return de;
    }

    private DecisionEntery CheckDribble()//state
    {
        DecisionEntery de = new DecisionEntery(GameManager.Instance.AIActions["Dribble"]);
        i = 0;
        //based on distance from enemies.
        foreach (Player p in foe)
        {
            if (p != null)
            {
                //find the distance between me and the target.(med range)
                f = Mathf.Abs(Vector2.Distance(player.transform.position, p.transform.position));

                //if the current number is closer to 1 than the previous then save the pair.
                if ((n == 0 || f - player.playerStats.targetMidRange < n - player.playerStats.targetMidRange) )
                {
                    //TODO this favors the closest player...
                    n = f;
                    pl = p;
                }
            }
        }

        i = CreateWeightingFromDistance(player.playerStats.targetMidRange, n, i);

        //based on position (looking exclusively at x axis or which teams side of the feild we are on.)
        float fl = Field.Instance.ConvertGlobalToField(player.gameObject.transform.position).x;

        //sometimes needs to fail to be interesting
        i = CreateWeightingFromPositionMult(i, GameManager.Instance.teams["Right"], fl);
        de.weight = i;
        return de;
    }

    //*******************************************************************************************************
    private DecisionEntery CheckCross()
    {
        DecisionEntery de = new DecisionEntery(GameManager.Instance.AIActions["Cross"]);
        pl = null;
        i = 1;
        //based on raycasts to the net
        //based on team location
        //may not be implemented
        de.target = pl;
        de.weight = i;
        de.decisionPosition = GameManager.Instance.positions["Forward"];
        return de;
    }

    //consider making stealing its own state... 
    private DecisionEntery CheckSteal()//no b
    {
        DecisionEntery de = new DecisionEntery(GameManager.Instance.AIActions["Steal"]);
        i = 0;
        n = 0;

        //based on distance from enemies
        foreach (Player p in foe)
        {
            if (p != null && p.psp.possessesBall)
            {
                //find the distance between me and the target.
                n = Mathf.Abs(Vector2.Distance(player.transform.position, p.transform.position));
            }
        }

        i = CreateWeightingFromDistance(player.playerStats.targetShortRange, n, i);

        de.decisionPosition = GameManager.Instance.positions["Defence"];
        de.weight = i;
        return de;
    }

    private DecisionEntery CheckClear()
    {
        DecisionEntery de = new DecisionEntery(GameManager.Instance.AIActions["Clear"]);
        i = 0;
        n = 0;
        //based on current location
        float fl = Field.Instance.ConvertGlobalToField(player.gameObject.transform.position).x;
        i = CreateWeightingFromPositionAdd(GameManager.Instance.teams["Right"], fl);

        fl = 0;
        //based on teammate averge location
        foreach (Player p in foe)
        {
            if (p != null)
            {
                //add up all the players positions here.
                fl += Field.Instance.ConvertGlobalToField(p.transform.position).x;
                n++;
            }
        }

        //divide the players total positions by the number of players. this gets their averge location. 
        fl /= n++;
        i = CreateWeightingFromPositionMult(i,GameManager.Instance.teams["Right"], fl);
        de.weight = i;
        de.decisionPosition = GameManager.Instance.positions["Defence"];
        return de;
    }

    //**********************************************************************************************
    private DecisionEntery CheckDive()//goalie
    {
        DecisionEntery de = new DecisionEntery(GameManager.Instance.AIActions["Dive"]);
        i = 0;
        //might not be implemented
        de.weight = i;
        return de;
    }

    private DecisionEntery CheckOpen()
    {
        DecisionEntery de = new DecisionEntery(GameManager.Instance.AIActions["NotOpen"]);
        i = 0;
        //based on raycast to ball
        if (CheckLine(player.transform.position, player.ballReference.transform.position, playerMask))
        {
            //triggered when player is no longer open.
            i = 0;
        }
        else
        {
            //weighting needs to be balanced against steals weight. Lots of tweaks may need to be made here in teh future.
            i = 60;//triggered when player is open. 
        }

        de.weight = i;
        return de;
    }




    private int CreateWeightingFromDistance(float range, float dist, int weight)
    {
        float fDist;
        fDist = dist / range;
        //based upon the distance from the player set the weighting of this desicion.
        if(fDist <= 0)
        {
            weight = 0;
        }
        if (fDist < 0.5f)
        {
            weight = 25;
        }
        else if (fDist < 1.5f)
        {
            weight = 75;
        }
        else if (fDist < 2)
        {
            weight = 50;
        }
        else if (fDist < 3)
        {
            weight = 25;
        }
        else//fDist>3
        {
            weight = 0;
        }

        return weight;
    }







    private int CreateWeightingFromPositionMult(int toMult, int team, float position)
    {
        //based on position (looking exclusively at x axis or which teams side of the feild we are on.)
        
        float tf = 0;
        if (player.playerStats.team == team)//if we are the team on the right.
        {
            if (position < 25)//0 - 25
            {
                tf = toMult * 0;
            }
            else if (position < 50)//25 - 50
            {
                tf = toMult * 0.5f;
            }
            else if (position < 75)//50-75
            {
                tf = toMult;
            }
            else//75-100
            {
                tf = toMult * 0.25f;
            }
        }
        else//if we are on the team on the left.
        {
            if (position < 25)//0 - 25
            {
                tf = toMult * 0.25f;
            }
            else if (position < 50)//25 - 50
            {
                tf = toMult;
            }
            else if (position < 75)//50-75
            {
                tf = toMult * 0.5f;
            }
            else//75-100
            {
                tf = toMult * 0;
            }
        }

        return (int)tf;
    }

    private int CreateWeightingFromPositionAdd(int team, float position)
    {
        //based on position (looking exclusively at x axis or which teams side of the feild we are on.)

        int ti = 0;
        if (player.playerStats.team == team)//if we are the team on the right.
        {
            if (position < 25)//0 - 25
            {
                ti = 0;
            }
            else if (position < 50)//25 - 50
            {
                ti = 50;
            }
            else if (position < 75)//50-75
            {
                ti = 100;
            }
            else//75-100
            {
                ti = 25;
            }
        }
        else//if we are on the team on the left.
        {
            if (position < 25)//0 - 25
            {
                ti = 25;
            }
            else if (position < 50)//25 - 50
            {
                ti = 100;
            }
            else if (position < 75)//50-75
            {
                ti = 50;
            }
            else//75-100
            {
                ti = 0;
            }
        }

        return (int)ti;
    }



    private void setTeamArrays()
    {
        team = new Player[GameManager.Instance.GetPlayers().Length];
        foe = new Player[GameManager.Instance.GetPlayers().Length];

        j = 0;
        i = 0;
        foreach (Player p in GameManager.Instance.GetPlayers())
        {
            //standard check for empty enteries.
            if (p != null)
            {
                //if player is on the same team as myself then add them to team list.
                if (p.team == player.playerStats.team)
                {
                    team[i] = p;
                    i++;
                }
                else//otherwise add them to enemy list.
                {
                    foe[j] = p;
                    j++;
                }
            }
        }
    }


    private void ClearAll()
    {
        i = 0;
        j = 0;
        options = new DecisionEntery[10];
        defenceOptions = new DecisionEntery[options.Length];
        forwardOptions = new DecisionEntery[options.Length];
    }


    private bool CheckLine(Vector2 positionOne, Vector2 positionTwo, LayerMask lm)
    {
        //create a linecast.
        //TODO remove hitarray. its a debug line.
        RaycastHit2D[] hitarray = Physics2D.LinecastAll(positionOne, positionTwo);
        foreach (RaycastHit2D hit in Physics2D.LinecastAll(positionOne, positionTwo))
                {
                    //check everything hit by the ray. Ignoring me
                    if(hit.collider.gameObject.GetInstanceID() != player.gameObject.GetInstanceID() && hit.collider.gameObject.layer == lm.value)
                    {
                    //If it is a player and isnt me then count it as a hit.
                    return true;
                    }
                }

        return false;
    }

    private bool CheckLine(Vector2 positionOne, Vector2 positionTwo, GameObject toIgnore, LayerMask lm)
    {

        //create a linecast.
        //TODO remove hitarray. its a debug line.
        RaycastHit2D[] hitarray = Physics2D.LinecastAll(positionOne, positionTwo);
        foreach (RaycastHit2D hit in Physics2D.LinecastAll(positionOne, positionTwo))
        {
            //check everything hit by the ray. Ignoreing me and the object to ignore.
            if (hit.collider.gameObject.GetInstanceID() != player.gameObject.GetInstanceID() && hit.collider.gameObject.GetInstanceID() != toIgnore.GetInstanceID() && hit.collider.gameObject.layer == lm.value)
            {
                //If it is a player and isnt me then count it as a hit.
                return true;
            }
        }

        return false;
    }



}
