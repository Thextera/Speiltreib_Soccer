using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatePattern : MonoBehaviour {

    public Ball ballReference;
    public PlayerMove movement;
    public bool possessesBall;
    public float possessionRange;
    public Player playerStats;

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
    void Update()
    {
        //print(currentState + " + " + gameObject);
        currentState.UpdateState();
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
            print(this + ", " + currentState);
            currentState = sPlayerMovementDecision;
            print(this + ", " + currentState);
        }
    }

    private void AtDestination()
    {
        print("I'm here");
        if(currentState == sPlayerMovingToPosition)
        {
            currentState = sPlayerActionDecision;
        }
    }


}
