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
    }

    private void OnEnable()
    {
        //sub to events here.
        EventManager.OnFirstWhistleBlow += EngagePlayerMovement;
        EventManager.OnBallPossessed += PlayerPossession;
    }

    private void OnDisable()
    {
        //unsub to events here.
        EventManager.OnFirstWhistleBlow -= EngagePlayerMovement;
        EventManager.OnBallPossessed -= PlayerPossession;
    }



    // Use this for initialization
    void Start()
    {
        ballReference = FindObjectOfType<Ball>();
        movement = GetComponent<PlayerMove>();
        currentState = sPlayerWait;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //print(currentState + " + " + gameObject);
        state = currentState.ReturnNameString();
        currentState.UpdateState();

        if(possessesBall)
        {
            possessionTimer -= Time.deltaTime;
        }
        else
        {
            possessionTimer = GameManager.Instance.maxPossessionTime;
        }

        if(possessionTimer <= 0)
        {
            if(currentState == sPlayerMovingToPosition)
            {
                movement.stopMove();
            }
            currentState = sPlayerActionDecision;
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
        if (currentState == sPlayerMovingToPosition)
        {
            movement.stopMove();
        }
        //reset state tree. this will represent a dribble action. Later an animator trigger could be hooked up to this.
        currentState = sPlayerWait;

        //if the steal is successfull the other AI will strip you of the ball. 
        if(success)
        {
            //additionally you will be forced to wait a short period as a penalty.
            sPlayerWait.SetCurrentWait(0.33f);
        }
        
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


}
