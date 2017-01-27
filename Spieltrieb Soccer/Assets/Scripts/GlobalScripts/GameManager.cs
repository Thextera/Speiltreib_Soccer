using System.Collections;
using System.Collections.Generic;
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

    GameObject player;
    public Player[] players;
    public Dictionary<string, int> positions;
    public Dictionary<string, int> teams;

    PlayerInit[] testInitList;

    void Awake()
    {
        positions = new Dictionary<string, int>();
        teams = new Dictionary<string, int>();

        //dictionary value pairs for all teams and player positions. 
        positions.Add("Goalie", 0);
        positions.Add("Defence", 1);
        positions.Add("Forward", 2);

        teams.Add("Right",0);
        teams.Add("Left",1);
    }

    void Start()
    {
        //load our prefab on start so we can instance it later.
        player = (GameObject)Resources.Load("Prefabs/Player");
        players = new Player[12];

        //TODO remove this.
        //debug create player :D

        testInitList = new PlayerInit[12];
        DebugCreatePlayer("Defence", "Right", 0, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(10, 10)));
        DebugCreatePlayer("Defence", "Right", 1, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(10, 20)));
        DebugCreatePlayer("Defence", "Right", 2, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(10, 30)));
        DebugCreatePlayer("Forward", "Right", 3, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(10, 40)));
        DebugCreatePlayer("Forward", "Right", 4, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(10, 50)));
        DebugCreatePlayer("Forward", "Right", 5, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(10, 60)));
                                                 
        DebugCreatePlayer("Defence", "Left", 6,  Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(90, 10)));
        DebugCreatePlayer("Defence", "Left", 7,  Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(90, 30)));
        DebugCreatePlayer("Defence", "Left", 8,  Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(90, 50)));
        DebugCreatePlayer("Forward", "Left", 9,  Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(90, 70)));
        DebugCreatePlayer("Forward", "Left", 10, Field.Instance.ConvertFieldCoordinateToGlobal(new Vector2(90, 90)));
        BeginGame(testInitList);
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
                p.SetPlayerStartingValues(pi.pCard.speed, pi.pCard.attack, pi.pCard.defence, pi.pCard.shoot, pi.pCard.pass, pi.pCard.dribble, pi.teamNumber, pi.pCard.position, pi.fieldStartPosition, pi.pCard.playerName);

                //TODO possibly set up a second constructor for visuals.

                i++;
            }
        }
    }

    //TODO the game end command must clear out all payer instances and the list attached to them.
    //TODO one thing that might be done to enhance loading speeds is to use a pool of players instead of instanceing them.

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) == true)
        {
            EventManager.Instance.FirstWhistleBlow();
        }
    }

    private void DebugCreatePlayer(string position, string team, int index, Vector2 startingPos)
    {
        PlayerCard initPCard = new PlayerCard(10, 20, 12, 112, 20, 4, "bob", GameManager.Instance.positions[position], 99);
        PlayerInit i = new PlayerInit(initPCard, startingPos, GameManager.Instance.teams[team], null);
        testInitList[index] = i;
    }

}
