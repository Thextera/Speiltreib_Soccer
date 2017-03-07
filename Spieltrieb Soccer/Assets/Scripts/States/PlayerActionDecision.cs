using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionDecision : IPlayerState
{
    public DecisionEntery[] options;
    public DecisionEntery chosenOption;
    public Player[] team;
    public Player[] foe;

    //private LayerMask fieldMask;
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
    private bool updateExecuting;
    private float waitDelay;

    private float userWaitTimer;
    public DecisionEntery userDecision;

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

        //fieldMask = Field.Instance.gameObject.layer;
        playerMask = player.gameObject.layer;
        //BallMask = player.ballReference.gameObject.layer;

        options = new DecisionEntery[10];

        teamsArentSet = true;
    }

    public void UpdateState()
    {
        if (!updateExecuting)
        {
            updateExecuting = true;
            PlayerManager.Instance.TriggerGameSlow(player);
        }
    }

    //default state actions. frame by fram actions.
    public IEnumerator UpdateStateTest()
    {
        //Debug.Log(player.playerStats.playerName + " Decision began.");
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
        //Debug.Log(player.playerStats.playerName + " Calculating options");
        CalculateOptions(options);
        i = 0;
        j = 0;


        //decide if user should be involved.******************************************************************************************************
        //TODO add antitheft flag?
        //TODO consider higher probabilities effecting player choice time?

        //if im not an AI consider letting the player do stuff. dont trigger for any desicion that doesnt involve the ball.
        if(!player.playerStats.AI && player.isPossessing())
        {
            //if there are 3 options at 100% then ALWAYS grab the user
            //Heavily favor shooting. 
            //if there are 3 options at 0% then NEVER grab the user
            n = 0;//averge weight
            j = 0;//number of valid enteries
            foreach (DecisionEntery de in options)
            {
                if(de != null)
                {
                    n += de.weight;
                    j++;
                }
            }
            n /= j; //the total weight / number of options = averge weight.
            //j has now changed to a random float for us.
            j = Random.Range(1, 100);

            //Debug.LogWarning("Averge Weight: " + n);
            //Debug.LogWarning("random chosen: " + j);
            if (j > n)
            {
                //call user
                //Debug.LogWarning("SUPER CALLED O.O");


                //if the player is involved
                
                //open player gui & display options
                Debug.LogWarning("Wait for iiiiiittttt!");

                //slow the game
                PlayerManager.Instance.SlowTime();

                //wait for player responce (display timer)
                PlayerManager.Instance.GetPlayerChoice(player, options);

                // our timer. count down to zero OR break when a user desicion has been made.
                userWaitTimer = GameManager.Instance.timeSlowDuration;
                while (userWaitTimer > 0 && userDecision == null)
                {
                    userWaitTimer -= Time.deltaTime; 
                    yield return null; 
                }
                userWaitTimer = 0;

                Debug.LogWarning("Done!!!!");
                //reset game speed
                PlayerManager.Instance.ResetTime();

                //deploy user selected option -OR- choose WORST option
                if(userDecision == null)
                {
                    ChooseOption(false);
                }
                else//user chose an option! *************************************************************************************
                {
                    //perhaps change this?
                    chosenOption = userDecision;
                }
            }
        }

        //If the AI hasn't Been given an option choose one.
        if (chosenOption == null)
        {
            chosenOption = ChooseOption(true);
        }

        if (chosenOption != null)//if it skips this code then no option was seen. this is more than likely an error.
        {
            //TODO fix this complete hack. :(

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

        //move to wait state.
        //Debug.Log(player.playerStats.playerName + " Decision completed.");
        ToPlayerWait(waitDelay);
        updateExecuting = false;
        yield return null;
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
        if(player.isPossessing())
        {
            if (player.playerStats.position == GameManager.Instance.positions["Forward"])
            {
                //attacker only desicions go here.
                deList[index] = CheckPass();
                index++;

                deList[index] = CheckShoot();
                index++;

            }
            else if(player.playerStats.position == GameManager.Instance.positions["Defence"])
            {
                //defender only desicions go here.
                deList[index] = CheckClear();
                index++;
            }

                //neutral desicions go here.
            deList[index] = CheckAttack();
            index++;

            deList[index] = CheckDribble();
            index++;

            deList[index] = CheckCross();
            index++;
        }
        else//i dont have the ball
        {
            if (player.playerStats.position == GameManager.Instance.positions["Defence"])
            {
                //defender only desicions go here.
                deList[index] = CheckSteal();
                index++;
            }
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

                //TODO review this section.
                f = Mathf.Abs(Vector2.Distance(player.transform.position, p.transform.position));
                //impossing a weighting penalty on anyone behind me. avoids shooting away from the net basically.
                Vector2 net;
                if (player.playerStats.team == 1)
                {
                    net = Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(0, 50));
                }
                else
                {
                    net = Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(100, 50));
                }
                if(Vector3.Distance(p.transform.position,net) > Vector3.Distance(player.transform.position,net))
                {
                    f *= 1.33f;
                }


                //if the current number is closer to 1 than the previous AND the player has a line of sight to the target then save the pair.
                if (n == 0 || f < n)
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
        Player badTarget = null;
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
                    else
                    {
                        badTarget = p;
                    }
                }
            }
        }
        //if there is a pass target then create a weight from the distance.
        if (pl != null)
        {
            i = CreateWeightingFromDistance(player.playerStats.targetMidRange, n, i);
        }
        else //If there is NOT a SONGLE TARGET then create a weight that can never be chosen. 
        {
            i = -1;
            pl = badTarget;
        }

        if (pl == null)
        {
            Debug.LogError(player.playerStats.playerName + " DOESNT KNOW WHO HE IS PASSING TO!!!");
        }

        de.target = pl;
        de.weight = i;
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
                if ((n == 0 || f < n ) )
                {
                    //TODO this favors the closest player...
                    n = f;
                    pl = p;
                }
            }
        }

        i = CreateWeightingFromDistance(player.playerStats.targetShortRange, n, i);

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
            if (p != null && p.psp.isPossessing())
            {
                //find the distance between me and the target.
                n = Mathf.Abs(Vector2.Distance(player.transform.position, p.transform.position));
                pl = p;
            }
        }

        if (pl != null)
        {
            i = CreateWeightingFromDistance(player.playerStats.targetShortRange, n, i);
        }
        else
        {
            i = -1;
        }

        de.target = pl;
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
        chosenOption = null;
    }


    private bool CheckLine(Vector2 positionOne, Vector2 positionTwo, LayerMask lm)
    {
        //create a linecast.
        //TODO remove hitarray. its a debug line.
        //[] hitarray = Physics2D.LinecastAll(positionOne, positionTwo);
        foreach (RaycastHit2D hit in Physics2D.LinecastAll(positionOne, positionTwo))
                {
                    if (hit.collider.gameObject.GetInstanceID() != GameManager.Instance.ballReference.gameObject.GetInstanceID())
                    {
                        //check everything hit by the ray. Ignoring me
                        if (hit.collider.gameObject.GetInstanceID() != player.gameObject.GetInstanceID() && hit.collider.gameObject.layer == lm.value)
                        {
                            //If it is a player and isnt me then count it as a hit.
                            return true;
                        }
                    }
                }

        return false;
    }

    private bool CheckLine(Vector2 positionOne, Vector2 positionTwo, GameObject toIgnore, LayerMask lm)
    {

        //create a linecast.
        //TODO remove hitarray. its a debug line.
        //RaycastHit2D[] hitarray = Physics2D.LinecastAll(positionOne, positionTwo);
        foreach (RaycastHit2D hit in Physics2D.LinecastAll(positionOne, positionTwo))
        {
            //exclude the ball for all raycasts.
            if (hit.collider.gameObject.GetInstanceID() != GameManager.Instance.ballReference.gameObject.GetInstanceID())
            {
                //check everything hit by the ray. Ignoreing me and the object to ignore.
                if (hit.collider.gameObject.GetInstanceID() != player.gameObject.GetInstanceID() && hit.collider.gameObject.GetInstanceID() != toIgnore.GetInstanceID() && hit.collider.gameObject.layer == lm.value)
                {
                    //If it is a player and isnt me then count it as a hit.
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// takes a list of decision enteries and chooses an option. true for the best option false for the worst.
    /// </summary>
    /// <param name="best">Take the best option in the list if true, take the worst option if false.</param>
    /// <returns></returns>
    private DecisionEntery ChooseOption(bool best)
    {
        DecisionEntery chosenOption = null;
        //choose option.
        //iterate through all options
        foreach (DecisionEntery de in options)
        {
            if (de != null)
            {
                if (best)
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
                else
                {
                    //select the lowest weighted option that is above zero. 
                    if (chosenOption == null || (chosenOption.weight > de.weight && de.weight > 0))
                    {
                        chosenOption = de;
                    }
                    else if (chosenOption != null && chosenOption.weight == de.weight)//tiebreaker
                    {
                        //do a 50/50 to see which option to choose.
                        //TODO be more deliberate with this. 50/50 isnt acceptable.
                        int rand = Random.Range(0, 1);
                        if (rand == 0)
                        {
                            Debug.Log("TIE WAS BROKEN");
                            chosenOption = de;
                        }
                    }
                }
            }
        }
        return chosenOption;
    }

    /// <summary>
    /// and external way to set this classes chosen option variable.
    /// </summary>
    /// <param name="de"></param>
    public void SetChosenOption(DecisionEntery de)
    {
        chosenOption = de;
    }

}
