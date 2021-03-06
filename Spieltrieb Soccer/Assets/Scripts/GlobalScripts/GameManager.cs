﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// class in charge of instancing the 12 players and ball. 
/// additionally manages the game's timer.
/// </summary>
public class GameManager : MonoBehaviour {

    #region SINGLETON PATTERN
    //simple singleton pattern. This allows functions in this class to be called globally as THERE CAN BE ONLY ONE!!!
    public static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("GameManager");
                    _instance = container.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }
    #endregion

    public Ball ballReference;
    GameObject player;
    private Player[] players;
    public Player[] team1;
    public Player[] team2;
    public Dictionary<string, int> positions;
    public Dictionary<string, int> teams;
    public Dictionary<string, int> AIActions;

    public float gameTimer;
    public bool gameTimerRunning = false;

    [Header("GameRules")]
    public int maxPlayersChasingBall = 2; //maximum number of players allowed to be in chasing state per team.
    public float stealStatThreshold = 1.5f; //the % of stats a defender must have to have a chance to avoid a steal. Default is 1.0f.
    public float stealStatLeanience = 0.1f; //the ammount of leaway the steal desicion is given for a "tie" (If the calculated value is within the the above threshold +- this number the desicion is counted as a draw. ) 
    public float gameSpeed = 1;
    public float playerInputWait = 2.5f; //how long will the game wait for players to input.
    public float maxPossessionTime = 1.4f;
    public float timeSlowDuration = 0.75f;
    public float GoalResetDelay = 0.75f;
    public float universalAnimationDuration = 0.33f;
    public float defaultAttackDuration = 1.2f;
    public float gameDuration = 120;

    private float resetTimer;

    public int leftTeamDeath;
    public int rightTeamDeath;

    PlayerInit[] testInitList;

    void Awake()
    {
        positions = new Dictionary<string, int>();
        teams = new Dictionary<string, int>();
        AIActions = new Dictionary<string, int>();

        //dictionary value pairs for all teams and player positions. 
        positions.Add("Defence", 1);
        positions.Add("Forward", 2);
        positions.Add("Goalie", 3);

        teams.Add("Right",0);
        teams.Add("Left",1);

        AIActions.Add("Attack", 0);
        AIActions.Add("Pass", 1);
        AIActions.Add("Shoot", 2);
        AIActions.Add("Dribble", 3);
        AIActions.Add("Cross", 4);
        AIActions.Add("Clear", 5);
        AIActions.Add("Steal", 6);
        AIActions.Add("NotOpen", 7);
        AIActions.Add("Dive", 8);

        ballReference = FindObjectOfType<Ball>();
    }

    void Start()
    {
        //load our prefab on start so we can instance it later.
        player = (GameObject)Resources.Load("Prefabs/Player");
        players = new Player[13];

        //TODO remove this.
        //debug create player :D

        testInitList = new PlayerInit[13];                                                                 
        DebugCreatePlayer("Defence", "Right", 0, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(40, 10)),false,"R1");
        DebugCreatePlayer("Defence", "Right", 1, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(40, 20)),false,"R2");
        DebugCreatePlayer("Defence", "Right", 2, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(40, 30)),false,"R3");
        DebugCreatePlayer("Forward", "Right", 3, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(40, 40)),false,"R4");
        DebugCreatePlayer("Forward", "Right", 4, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(40, 50)),false,"R5");
        DebugCreatePlayer("Forward", "Right", 5, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(40, 60)),false,"R6");
                                                                                                                          
        DebugCreatePlayer("Defence", "Left", 6,  Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(60, 10)), true,"L1");
        DebugCreatePlayer("Defence", "Left", 7,  Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(60, 30)), true,"L2");
        DebugCreatePlayer("Forward", "Left", 8,  Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(60, 50)), true,"L3");
        DebugCreatePlayer("Defence", "Left", 9,  Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(60, 70)), true,"L4");
        DebugCreatePlayer("Forward", "Left", 10, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(60, 90)), true,"L5");
        DebugCreatePlayer("Forward", "Left", 11, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(60, 40)), true,"L6");
        BeginGame(testInitList);
    }

    private void OnEnable()
    {
        //sub to events here.
        EventManager.OnGoal += TeamScored;
        EventManager.OnWhistleBlow += ContinueGame;
    }

    private void OnDisable()
    {
        //unsub to events here.
        EventManager.OnGoal -= TeamScored;
        EventManager.OnWhistleBlow -= ContinueGame;
    }

    void BeginGame(PlayerInit[] playerList)
    {
        //TODO instatiate different AI types here.
        int i = 0;

        foreach(PlayerInit pi in playerList)
        {
            if (pi != null)
            {
                //create player instance
                GameObject g = Instantiate(player);

                //save player instance to list.
                Player p = g.GetComponent<Player>();
                //print(players + "player list");
                //print(p + "player current");
                players[i] = p;

                //run the player's constructor so it correctly sets all its own values.
                //thar be dragons... :(
                p.SetPlayerStartingValues(pi.pCard.speed, pi.pCard.attack, pi.pCard.defence, pi.pCard.shoot, pi.pCard.pass, pi.pCard.dribble, pi.teamNumber, pi.pCard.position, pi.fieldStartPosition, pi.pCard.playerName, pi.AI);
                p.AddAttack("DragonKick",1);
                p.AddAttack("FlameStrike",3);
                //default theDragonKick" players to be waiting.
                //TODO possibly set up a second constructor for visuals.

                i++;
            }
        }

        gameTimer = gameDuration;
        EventManager.Instance.GameBegin();


    }

    //TODO the game end command must clear out all payer instances and the list attached to them.
    //TODO one thing that might be done to enhance loading speeds is to use a pool of players instead of instanceing them.

    private void Update()
    {
        //debug reset scene
        if(Input.GetKey(KeyCode.Space))
        {
            resetTimer++;
        }
        else
        {
            resetTimer = 0;
        }
        if(resetTimer > 15)
        {
            DebugLoadScene();
        }


        //Time.timeScale = gameSpeed;
        //sets a timer for the game. slows with slow-motion.
        if (gameTimerRunning)
        {
            gameTimer -= Time.deltaTime * Time.timeScale;
        }
        
        //if time runs out end the game.
        if(gameTimer <= 0)
        {
            gameTimer = 0;
            EndGame();
        }
    }

    private void DebugCreatePlayer(string position, string team, int index, Vector2 startingPos, bool AI, string name)
    {
        PlayerCard initPCard = new PlayerCard(10, 20, 12, 112, 20, 4, name, GameManager.Instance.positions[position], 99);
        PlayerInit i = new PlayerInit(initPCard, startingPos, GameManager.Instance.teams[team], null, AI);
        testInitList[index] = i;
    }

    public Player[] GetPlayers()
    {
        return players;
    }

    private void TeamScored(int Team)
    {
        gameTimerRunning = false;
    }

    private void ContinueGame()
    {
        gameTimerRunning = true;
    }

    /// <summary>
    /// when called this function ends the game. 
    /// </summary>
    private void EndGame()
    {
        Invoke("DebugLoadScene", 5);
    }

    private void DebugLoadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

}
