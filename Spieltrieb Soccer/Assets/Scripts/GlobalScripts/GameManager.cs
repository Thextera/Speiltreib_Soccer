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
        PlayerCard initPCard = new PlayerCard(10,20,12,112,20,4,"bob",GameManager.Instance.positions["Forward"],99);
        PlayerInit i = new PlayerInit(initPCard,Vector2.zero,GameManager.Instance.teams["Right"],null);
        PlayerInit[] testInitList = new PlayerInit[1];
        testInitList[0] = i;

        BeginGame(testInitList);
    }

    void BeginGame(PlayerInit[] playerList)
    {
        //TODO instatiate different AI types here.
        int i = 0;

        foreach(PlayerInit pi in playerList)
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
            p.SetPlayerStartingValues(pi.pCard.speed,  pi.pCard.attack,  pi.pCard.defence,  pi.pCard.shoot,  pi.pCard.pass,  pi.pCard.dribble,  pi.teamNumber,  pi.pCard.position,  pi.fieldStartPosition,  pi.pCard.playerName);

            //TODO possibly set up a second constructor for visuals.

            i++;
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

}
