using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Player managed. Manages User Interactions. 
/// </summary>
public class PlayerManager : MonoBehaviour {

    #region SINGLETON PATTERN
    //simple singleton pattern. This allows functions in this class to be called globally as THERE CAN BE ONLY ONE!!!
    public static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<PlayerManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("PlayerManager");
                    _instance = container.AddComponent<PlayerManager>();
                }
            }

            return _instance;
        }
    }
    #endregion
    bool calculatingDecision = false;
    bool userPollFlag = false;
    private float slowTimer;

    public Image timerImage;
    public Button attackButt; //   :3
    public Button passButt;
    public Button clearButt;
    public Button shootButt;

    public DecisionEntery userDecision = null;
    private PlayerStatePattern currentPlayer;

    private void Awake()
    {
        DisableAllUIElements();
    }

    public void TriggerGameSlow(PlayerStatePattern p)
    {
        //Debug.LogWarning("IT WAS I, DIO");
        StartCoroutine(p.sPlayerActionDecision.UpdateStateTest());

    }

    public void GetPlayerChoice(PlayerStatePattern p, DecisionEntery[] de)
    {
        if (!userPollFlag)
        {
            StartCoroutine(PollUser(p, de));
        }
        else
        {
            Debug.LogError("Something went wrong. User was polled twice at once.");
        }
    }

    public void ResetTime()
    {
        Time.timeScale = 1;
    }

    public void SlowTime()
    {
        Time.timeScale = 0.25f;
    }

    private IEnumerator PollUser(PlayerStatePattern p, DecisionEntery[] de)
    {
        //engage user poll flag.
        userPollFlag = true;
        currentPlayer = p;
        slowTimer = GameManager.Instance.timeSlowDuration;

        //enable player choice UI. (un-hide or un-grey or whatever needs to be done.)
        //TODO I actually have an incredible amount of flexibility here. I can make the buttons dynamically generated if i so choose.
        foreach (DecisionEntery d in de)
        {
            if (d != null)
            {
                ActivateUIElements(d.name);
            }
        }

        //display options based on player. (If attack show X, if defence show Y)
        //Display Specials as well...?

        yield return null;

        timerImage.fillAmount = 1;

        //engage while loop and wait for user input. (break if user input is not null)
        while (slowTimer > 0 && userDecision == null)
        {
            //Debug.LogWarning("Doing a thing" + slowTimer + Time.deltaTime);
            slowTimer -= Time.deltaTime;
            timerImage.fillAmount = (slowTimer / GameManager.Instance.timeSlowDuration);
            yield return null;
        }
        //Debug.LogWarning("Finished a thing");
        //take user input and send back in the form of a callback.
        p.sPlayerActionDecision.userDecision = userDecision;
        if (p.sPlayerActionDecision.userDecision == null)
        {
            //TOO SLOW!!! ***************************************************************************************************
        }
        //if there is no input or input is invalid simply return null. put UI element on screen saying "TOO SLOW!"

        userPollFlag = false;
        DisableAllUIElements();
        ClearTimer();
        clearAll();
        yield return null;
    }



    void ActivateUIElements(int i)
    {
        //If the existing option is one the player can control then issue the prompt on screen. 
        switch (i)
        {
            case 0://attack
                EnableAttack();
                break;

            case 1://pass
                EnablePass();
                break;

            case 2://shoot
                EnableShoot();
                break;

            case 5://clear
                EnableClear();
                break;

        }
    }

    void EnableAttack()
    {
        attackButt.gameObject.SetActive(true);
    }

    void EnablePass()
    {
        passButt.gameObject.SetActive(true);
    }

    void EnableShoot()
    {
        shootButt.gameObject.SetActive(true);
    }

    void EnableClear()
    {
        clearButt.gameObject.SetActive(true);
    }


    void DisableAllUIElements()
    {
        attackButt.gameObject.SetActive(false);
        passButt.gameObject.SetActive(false);
        shootButt.gameObject.SetActive(false);
        clearButt.gameObject.SetActive(false);
    }

    void ClearTimer()
    {
        timerImage.fillAmount = 0;
    }

    void clearAll()
    {
        userDecision = null;
        currentPlayer = null;
    }

    public void UserAttack()
    {
        //trigger attack aimer.
        if (!calculatingDecision)
        {
            StartCoroutine(calculateDecisionWithTarget(false, currentPlayer, GameManager.Instance.AIActions["Attack"]));
        }
    }

    public void UserPass()
    {
        //trigger pass aimer
        if (!calculatingDecision)
        {
            StartCoroutine(calculateDecisionWithTarget(true, currentPlayer, GameManager.Instance.AIActions["Pass"]));
        }
    }

    public void UserShoot()
    {
        //trigger shot aimer
        if (!calculatingDecision)
        {
            StartCoroutine(calculateDecisionWithTarget(currentPlayer, GameManager.Instance.AIActions["Shoot"]));
        }
    }

    public void UserClear()
    {
        //clear the ball
        userDecision = new DecisionEntery(GameManager.Instance.AIActions["Clear"]);
    }




    private IEnumerator calculateDecisionWithTarget(bool mt, PlayerStatePattern p, int name)
    {
        calculatingDecision = true;
        Player target = null;
        //as long as there is no target and there is still time keep polling for a user generated target. 
        while (slowTimer > 0 && target == null)
        {
            target = ChooseTarget(mt, p);
            yield return null;
        }

        if (target != null)
        {
            userDecision = new DecisionEntery(name);
            userDecision.target = target;
        }

        calculatingDecision = false;
        yield return null;
    }




    private IEnumerator calculateDecisionWithTarget(PlayerStatePattern p, int name)
    {
        calculatingDecision = true;
        Vector2 shotTarget = Vector2.zero;
        //as long as there is no target and there is still time keep polling for a user generated target. 
        while (slowTimer > 0 && shotTarget == Vector2.zero)
        {
            shotTarget = GetLastTouchLocation();
            yield return null;
        }
        if (shotTarget != Vector2.zero)
        {
            userDecision = new DecisionEntery(name);
            userDecision.shotTarget = shotTarget;
        }
        calculatingDecision = false;
        yield return null;
    }


    /// <summary>
    /// Prompts player to pick a target out of a set group of players.
    /// </summary>
    /// <param name="mt">will function include both teams? (If false: only use friendly players.)</param>
    /// <returns></returns>
    private Player ChooseTarget(bool mt, PlayerStatePattern p)
    {
        Player Target = null;
        float lastDistance = 0;
        Vector2 userChoice = Vector2.zero;

        //get position from user. (touch ended phase. or mouse click for debugging purposes.)
        //************************************************************************************************************************************************

        userChoice = GetLastTouchLocation();

        if (userChoice != Vector2.zero)
        {
            //pick closest unit to the end location.
            foreach (Player pl in GameManager.Instance.GetPlayers())
            {
                //check any entery that isnt null and Isnt the calling instance.
                if (pl != null && pl.gameObject.GetInstanceID() != p.gameObject.GetInstanceID())
                {
                    //if we are looking at my team then filter out enemies.
                    if (mt)
                    {
                        if (pl.team == p.playerStats.team)
                        {
                            //check distance
                            float d = Vector2.Distance(pl.gameObject.transform.position, userChoice);
                            //if the last distance is 0 or is greater than this one then replace it and set the target.
                            if (lastDistance == 0 || d < lastDistance)
                            {
                                lastDistance = d;
                                Target = pl;
                            }
                        }
                    }
                    else//if we only want to look at opponants then we need to filter out teammates.
                    {
                        //only proceed for foes.
                        if (pl.team != p.playerStats.team)
                        {
                            //check distance
                            float d = Vector3.Distance(pl.gameObject.transform.position, userChoice);
                            //if the last distance is 0 or is greater than this one then replace it and set the target.
                            if (lastDistance == 0 || d < lastDistance)
                            {
                                lastDistance = d;
                                Target = pl;
                            }
                        }
                    }
                }
            }
        }
        //return player.
        if(Target != null)
        {
            return Target;
        }

        //if no value was found, user was too slow. return null and fail the operation.
        return null;

    }


    private Vector2 GetLastTouchLocation()
    {
        print("SPAAAAAAMM");
        Vector2 touchLocation = Vector2.zero;

        //grab mouse click location.
        if(Input.GetMouseButtonDown(0))
        {
           touchLocation = Input.mousePosition;
        }

        //poll touch screen touches.
        foreach (Touch t in Input.touches)
        {
            if (t.phase == TouchPhase.Ended)
            {
                touchLocation = t.deltaPosition;
            }
        }

        return touchLocation;
    }
}
