using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    //events of note: game begin. game end. goal.
    //game timer will call game begin and end.
    //the ball will call goal when it is in teh goal zone.

    #region SINGLETON PATTERN
    //simple singleton pattern. This allows functions in this class to be called globally as THERE CAN BE ONLY ONE!!!
    public static EventManager _instance;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<EventManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("EventManager");
                    _instance = container.AddComponent<EventManager>();
                }
            }

            return _instance;
        }
    }
    #endregion

    //when a goal is scored
    public delegate void GoalAction(int team);
    public static event GoalAction OnGoal;

    //tell players theat the ref's whistle ahas been blown (TRiggers Initial movement)
    public delegate void WhistleBlowAction();
    public static event WhistleBlowAction OnWhistleBlow;

    //triggered when game begins. call any initializations from this.
    public delegate void GameBeginAction();
    public static event GameBeginAction OnGameBegin;

    //triggered when game ends. use this for cleanup and moving back to main gui.
    public delegate void GameEndAction();
    public static event GameEndAction OnGameOver;

    //remove this?
    public delegate void BallPossessedAction();
    public static event BallPossessedAction OnBallPossessed;

    //trigger when its time to reset the locations of all the players.
    public delegate void PlayerLocationResetAction();
    public static event PlayerLocationResetAction OnResetAllPlayerLocations;

    /*
    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, 5, 100, 30), "Click"))
        {
            if (OnClicked != null)
                OnClicked();
        }
    }
    */

    //if any function calls goal and gives a team it triggers evey obeject subscribed to respond to said goal.
    public void Goal(int team)
    {
        //check if anyone is subscribed. 
        if (OnGoal != null)
        {
            OnGoal(team);
        }
    }

    public void WhistleBlow()
    {
        if(OnWhistleBlow != null)
        {
            OnWhistleBlow();
        }
    }

    public void BallPossessed()
    {
       // if(OnBallPossessed != null)
       // {
       //     Debug.Log("POSSESED");
       //     OnBallPossessed();
       // }
    }

    public void ResetPlayerPositions()
    {
        if(OnResetAllPlayerLocations != null)
        {
            OnResetAllPlayerLocations();
        }
    }

    public void GameBegin()
    {
        if(OnGameBegin != null)
        {
            OnGameBegin();
        }
    }
}
