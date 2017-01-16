using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatePattern : MonoBehaviour {

    public bool possessesBall;

    [HideInInspector] public IPlayerState currentState;
    [HideInInspector] public PlayerWait sPlayerWait;
    [HideInInspector] public PlayerChaseBall sPlayerChaseBall;
    [HideInInspector] public PlayerDecision sPlayerDecision;
    [HideInInspector] public PlayerRunAndDribble sPlayerRunAndDribble;

    private void Awake()
    {
        sPlayerWait = new PlayerWait(this);
        sPlayerDecision = new PlayerDecision(this);
        sPlayerChaseBall = new PlayerChaseBall(this);
        sPlayerRunAndDribble = new PlayerRunAndDribble(this);


    }

    // Use this for initialization
    void Start()
    {
        currentState = sPlayerWait;
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState();
    }

}
