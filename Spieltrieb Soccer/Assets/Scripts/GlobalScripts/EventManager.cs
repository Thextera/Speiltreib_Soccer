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

    public delegate void GoalAction(int team);
    public static event GoalAction OnGoal;

    public delegate void FirstWhistleBlowAction();
    public static event FirstWhistleBlowAction OnFirstWhistleBlow;

    public delegate void GameBeginAction();
    public static event GameBeginAction OnGameBegin;

    public delegate void GameEndAction();
    public static event GameEndAction OnGameOver;

    public delegate void BallPossessedAction();
    public static event BallPossessedAction OnBallPossessed;

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

    public void FirstWhistleBlow()
    {
        if(OnFirstWhistleBlow != null)
        {
            OnFirstWhistleBlow();
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
}
