using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatePattern : MonoBehaviour {

    public Ball ballReference;
    public PlayerMove movement;
    private bool possessesBall;
    public float possessionRange;
    public Player playerStats;
    public string state;
    private Vector2 startLocation;
    private bool disabledForGoal;
    private bool dead = false;
    public float reviveWaitTime = 1.5f; //how long a player will remain in teh wait state before resuming normal function.

    private float possessionTimer;

    public IPlayerState currentState;
    [HideInInspector] public PlayerWait sPlayerWait;
    [HideInInspector] public PlayerDead sPlayerDead;
    [HideInInspector] public PlayerChaseBall sPlayerChaseBall;
    [HideInInspector] public PlayerRunAndDribble sPlayerRunAndDribble;
    [HideInInspector] public PlayerActionDecision sPlayerActionDecision;
    [HideInInspector] public PlayerMovementDecision sPlayerMovementDecision;
    [HideInInspector] public PlayerMovingToPosition sPlayerMovingToPosition;


    private void Awake()
    {
        playerStats = GetComponent<Player>();

        sPlayerWait = new PlayerWait(this);
        sPlayerDead = new PlayerDead(this);
        sPlayerChaseBall = new PlayerChaseBall(this);
        sPlayerRunAndDribble = new PlayerRunAndDribble(this);
        sPlayerActionDecision = new PlayerActionDecision(this);
        sPlayerMovementDecision = new PlayerMovementDecision(this);
        sPlayerMovingToPosition = new PlayerMovingToPosition(this);

        //load an HP bar from the resources folder.
        GameObject g = (GameObject)Instantiate(Resources.Load("Prefabs/HealthBar"));
        g.SendMessage("SetOwner", playerStats);
       
    }

    private void OnEnable()
    {
        //sub to events here.
        EventManager.OnGameBegin += WaitForWhistle;
        EventManager.OnWhistleBlow += EngagePlayerMovement;
        EventManager.OnBallPossessed += PlayerPossession;
        EventManager.OnGoal += TeamScored;
        EventManager.OnResetAllPlayerLocations += ResetLocation;
    }

    private void OnDisable()
    {
        //unsub to events here.
        EventManager.OnGameBegin -= WaitForWhistle;
        EventManager.OnWhistleBlow -= EngagePlayerMovement;
        EventManager.OnBallPossessed -= PlayerPossession;
        EventManager.OnGoal -= TeamScored;
        EventManager.OnResetAllPlayerLocations -= ResetLocation;
    }



    // Use this for initialization
    void Start()
    {
        startLocation = transform.position;
        ballReference = FindObjectOfType<Ball>();
        movement = GetComponent<PlayerMove>();
        currentState = sPlayerWait;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //manage stares for health
        if (playerStats.currentHealth <= 0)
        {
            //set player state to dead. this can never be overwritten unless the player is alive.
            currentState = sPlayerDead;

            //trigger kill payer only once.
            if (dead == false)
            {
                KillPlayer();
            }
        }

        //print(currentState + " + " + gameObject);
        if (!disabledForGoal)
        {
            state = currentState.ReturnNameString();
            currentState.UpdateState();

            if (possessesBall)
            {
                possessionTimer -= Time.deltaTime;
            }
            else
            {
                possessionTimer = GameManager.Instance.maxPossessionTime;
            }

            if (possessionTimer <= 0)
            {
                StopMovement();//hard
                currentState = sPlayerActionDecision;
            }
        }





    }



    //Engages player into movement logic.
    void EngagePlayerMovement()
    {
        currentState = sPlayerMovementDecision;
    }


    void PlayerPossession()
    {
        if(currentState == sPlayerChaseBall)
        {
            //possibly set state to action here?
            currentState = sPlayerMovementDecision;
        }
    }

    private void AtDestination()
    {
        //print("I'm here");
        if(currentState == sPlayerMovingToPosition)
        {
            currentState = sPlayerActionDecision;
        }
    }

    public void IncomingPass()
    {
        Debug.Log(playerStats.playerName + " is getting passed to!");
        //if you are being passed to immedeately become open to catch the ball.
        currentState = sPlayerChaseBall;
    }

    public void BallStolen(bool success)
    {
        //trigger this when being stolen from.
        //if you are moving stop moving.
        StopMovement();//hard

        //reset state tree. this will represent a dribble action. Later an animator trigger could be hooked up to this.
        currentState = sPlayerWait;

        //if the steal is successfull the other AI will strip you of the ball. 
        if(success)
        {
            //additionally you will be forced to wait a short period as a penalty.
            sPlayerWait.SetCurrentWait(0.33f);
        }
        
    }

    private void TeamScored(int team)
    {
        disabledForGoal = true;
        //sort if a player should cheer based upon who scored.

        StopMovement();//hard
        currentState = sPlayerWait;
        sPlayerWait.WaitUntilDismissed();
        Invoke("AIResume", GameManager.Instance.GoalResetDelay);

        //if statement for future animator.
        if (team == playerStats.team)
        {

        }

    }

    private void WaitForWhistle()
    {
        //Debug.LogWarning("Waiting For Whistle");
        //when the players are waiting for the whistle make them stay in the waiting state.
        StopMovement();//hard
        currentState = sPlayerWait;
        sPlayerWait.WaitUntilDismissed();
    }

    public void possessBall()
    {
        possessesBall = true;
        //print(playerStats.playerName + " gained possession");
    }

    public void dePossessBall()
    {
        possessesBall = false;
        //print(playerStats.playerName + " lost possession");
    }

    public bool isPossessing()
    {
        //print(playerStats.playerName + "'s possession was checked " + possessesBall);
        return possessesBall;
    }

    private void ResetLocation()
    {
        transform.position = startLocation;
    }

    private void StopMovement()
    {
        if (currentState == sPlayerMovingToPosition)
        {
            movement.stopMove();
        }
    }

    private void AIResume()
    {
        disabledForGoal = false;
    }

    /// <summary>
    /// Kills player. Disabling collider and setting HP to a specific value. 
    /// </summary>
    public void KillPlayer()
    {
        //set refernce bool
        dead = true;

        //set health to a safer value.
        playerStats.currentHealth = 0;

        //Halt any movement from the player.
        StopMovement();

        //ensure state is dead. because due dilligence
        currentState = sPlayerDead;

        //disable Collider so that nothing hits teh dead player.
        GetComponent<CircleCollider2D>().enabled = false;
    }

    /// <summary>
    /// Revives player and sets health to a given % of their maximum.
    /// </summary>
    /// <param name="currentHeathPercent">% (1-100) of their maximum health a player should be revived with</param>
    public void RevivePlayer(float currentHeathPercent)
    {
        dead = false;

        //if the health % are outside of bounds correct them
        if(currentHeathPercent > 100)
        {
            currentHeathPercent = 100;
        }
        else if(currentHeathPercent < 1)
        {
            currentHeathPercent = 1;
        }

        //set player's health to be a specified % of their max.
        playerStats.currentHealth = playerStats.maxHealth / (currentHeathPercent /100);

        //turn back on player collider.
        GetComponent<CircleCollider2D>().enabled = true;

        //reset the state tree.
        currentState = sPlayerWait;
        sPlayerWait.SetCurrentWait(reviveWaitTime);
    }
}
