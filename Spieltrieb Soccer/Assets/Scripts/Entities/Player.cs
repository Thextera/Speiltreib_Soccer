using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// holds all information about a player inside the player object. passes and parses information when instantiating.
/// </summary>
public class Player : MonoBehaviour {

    public PlayerMove pm;
    public PlayerStatePattern psp;
    public SpriteRenderer IDRing;

    public float maxHealth = 100;
    public float currentHealth;

    public float speed;
    public float attack;
    public float defence;
    public float shoot;
    public float pass;
    public float dribble;
    public string playerName;
    public int position;
    public int team;

    public Dictionary<string, int> SPAttacks;

    public float attackStrengthMultiplier;

    public bool AI;

    public float engageDistance = 5;
    public float ballEngageDistance = 7.5f;

    //target ranges to make desicions off of. 
    public float targetLongRange = 10;
    public float targetMidRange = 5;
    public float targetShortRange = 1;

    public Vector2 StartLocation;

    /// <summary>
    /// initializes player. MUST BE CALLED WHEN INSTANCING A PLAYER OR ELSE DEFAULT VALUES WILL BE GIVEN.
    /// </summary>
    /// <param name="initSpeed">starting speed</param>
    /// <param name="initAttack">starting attack</param>
    /// <param name="initDefence">starting defence</param>
    /// <param name="initShoot">starting shooting skill</param>
    /// <param name="initPass">starting passing skill</param>
    /// <param name="initDribble">starting dribbling skill</param>
    /// /// <param name="initPlayerName">players name for UI reference</param>
    public void SetPlayerStartingValues(float initSpeed, float initAttack, float initDefence, float initShoot, float initPass, float initDribble, int initTeam, int initPosition, Vector2 initLocation, string initPlayerName, bool initAI)
    {
        //set our class variables.
        speed = initSpeed;
        attack = initAttack;
        defence = initDefence;
        shoot = initShoot;
        pass = initPass;
        team = initTeam;
        dribble = initDribble;
        position = initPosition;
        playerName = initPlayerName;
        StartLocation = initLocation;
        AI = initAI;

        //set player health
        currentHealth = maxHealth;

        //name the game object the same name as ME! this makes troublshooting easier.
        this.gameObject.name = initPlayerName;
        

        //find our movement class.
        pm = GetComponent<PlayerMove>();

        //find our AI controller
        psp = GetComponent<PlayerStatePattern>();

        // if we found the movement class set its speed to relate to the cards speed stat. otherwise throw an error.
        if (pm != null)
        {
            UpdateSpeed(speed);
            UpdateUI();
        }
        else
        {
            throw new UnityException("PlayerMove Script not found. (Initialization) Please consult programmer for troubleshooting help.");
        }

        //move player into specified starting position.
        transform.position = initLocation;

        SPAttacks = new Dictionary<string, int>();

    }

    //set an array of attacks to be used later.
    public void AddAttack(string attack, int uses)
    {
        SPAttacks.Add(attack,uses);
    }

    public void UpdateSpeed(float uSpeed)
    {
        //gram the movement script if its not already found.
        if (pm == null)
        {
            pm = GetComponent<PlayerMove>();
        }

        //check if the script was found. update values if it was, error if it wasn't.
        if(pm != null)
        {
            //TODO tweak speed formulas here. these are currently just temporary.
            //pm.velocity = 0;
            //pm.movementAcceleration = 0;
            //pm.targetRadius = 0;
            //pm.minVelocity = 0;
            //print("speed updated! :D");
        }
        else
        {
            throw new UnityException("PlayerMove Script not found. (Speed Update) Please consult programmer for troubleshooting help.");
        }

    }

    public void UpdateAttack(float uAttack)
    {

    }

    public void UpdateDefence(float uDefence)
    {

    }

    public void UpdateShoot(float uShoot)
    {

    }

    public void UpdatePass(float uPass)
    {

    }

    public void UpdateDribble(float uDribble)
    {

    }

    public void UpdateUI()
    {
        if (team == 0)
        {
            if (position == GameManager.Instance.positions["Forward"])
            {
                IDRing.color = Color.blue;
            }
            else
            {
                IDRing.color = Color.cyan;
            }
        }
        else
        {
            if (position == GameManager.Instance.positions["Forward"])
            {
                IDRing.color = Color.red;
            }
            else
            {
                IDRing.color = Color.yellow;
            }
        }
    }




}
