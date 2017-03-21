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

    [Header("UI Elements")]
    public Image timerImage;
    public Text gameTimer;
    public Button attackButt; //   :3
    public Button passButt;
    public Button clearButt;
    public Button shootButt;
    public Text rTeamScore;
    public Text lTeamScore;
    public Image fadeScreenImage;
    public Image countdownREADYImage;
    public Image countdownGOImage;

    public float screenFadeDuration;
    public float readyFadeLength;
    public float readyFullALphaDuration;
    public float GoFadeLength;
    public float countdownMaxDuration;
    public float countdownSingleImageDuration = 0.75f;

    public DecisionEntery userDecision = null;

    private CameraControl ballCamera;

    private int rScore = 0;
    private int bScore = 0;
    private PlayerStatePattern currentPlayer;
    private Ball ballReference;
    private bool fadeLock = false;

    private void Awake()
    {
        DisableAllUIElements();
        ballReference = FindObjectOfType<Ball>();
        ballCamera = FindObjectOfType<CameraControl>();
    }

    private void OnEnable()
    {
        //sub to events here.
        EventManager.OnGoal += TeamScored;
        EventManager.OnGameBegin += InitializeGame;
    }

    private void OnDisable()
    {
        //unsub to events here.
        EventManager.OnGoal -= TeamScored;
        EventManager.OnGameBegin += InitializeGame;
    }

    private void Update()
    {
        gameTimer.text = GameManager.Instance.gameTimer.ToString("F2");
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

    public void TeamScored(int Team)
    {
        if(Team == 0)//right scored
        {
            //add 1 to right team score counter.
            rScore += 1;
            rTeamScore.text = rScore.ToString();
        }
        else
        {
            //add 2 to left team score counter.
            bScore += 1;
            lTeamScore.text = bScore.ToString();
        }

        //fade the screen to white and then reset the game.
        //disable the net's rigidbody so it cant trigger multiple times.
        DisarmNet();
        StartCoroutine(CrossFadeAndReset());
    }

    private IEnumerator CrossFadeAndReset()
    {
        //let the AI wait and cheer for just a little bit then begin the reset proccess.
        yield return new WaitForSeconds(GameManager.Instance.GoalResetDelay);

        //fade the screen to white.
        StartCoroutine(FadeTo(1, screenFadeDuration));
        yield return new WaitForSeconds(screenFadeDuration + 0.1f);

        //while the screen is white move all the players back to where they started. and the ball
        //re-enable net rigidBody.
        EventManager.Instance.ResetPlayerPositions();
        ballReference.resetPosition();
        ballCamera.HardCameraReset();
        ArmNet();

        //now that everything is in place fade the screen back to the game
        StartCoroutine(FadeTo(0, screenFadeDuration));
        yield return new WaitForSeconds(screenFadeDuration + 0.1f);

        //count the game back in again so the player has some time to react.
        StartBeginGameCountdown();
        Invoke("StartPlayers", countdownMaxDuration);
        //whistle sound here?
        

        yield return null;
    }

    /// <summary>
    /// Trigger game Gui when the game begins.
    /// </summary>
    private void InitializeGame()
    {
        //count the game in.
        StartBeginGameCountdown();
        Invoke("StartPlayers", countdownMaxDuration);
        //whistle sound here?
    }

    private void StartPlayers()
    {
        EventManager.Instance.WhistleBlow();
    }


    /// <summary>
    /// Disables scripts on the nets in order to stop nets from counting multiple goals.
    /// </summary>
    private void DisarmNet()
    {
        foreach(GoalZone gz in Field.Instance.gz)
        {
            gz.enabled = false;
        }
    }

    /// <summary>
    /// enables scripts on nets.
    /// </summary>
    private void ArmNet()
    {
        foreach (GoalZone gz in Field.Instance.gz)
        {
            gz.enabled = true;
        }
    }

    /// <summary>
    /// Fades the Entire Screen to specified alpha value
    /// </summary>
    /// <param name="aValue">value to fade to</param>
    /// <param name="aTime">how long the fade should take</param>
    /// <returns></returns>
    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = fadeScreenImage.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            fadeScreenImage.color = newColor;
            yield return null;
        }
    }

    /// <summary>
    /// Fades specific object to specified alpha value
    /// </summary>
    /// <param name="toFade">the object in question to fade. Must have color Param</param>
    /// <param name="initDelay">should the function wait before executing?</param>
    /// <param name="aValue">value to fade to</param>
    /// <param name="aTime">how long the fade should take</param>
    /// <returns></returns>
    IEnumerator FadeTo(Image toFade, float initDelay, float aValue, float aTime)
    {
        //give us a delay
        yield return new WaitForSeconds(initDelay);

        //if we are fading a number out that is already faded pop it in first.
        if(aValue == 0 && toFade.color.a != 1)
        {
            Color newColor = new Color(1, 1, 1, 1);
            toFade.color = newColor;
        }

        //begin the fade process.
        float alpha = toFade.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            toFade.color = newColor;
            yield return null;
        }
    }


    public void StartBeginGameCountdown()
    {
        StartCoroutine(FadeTo(countdownREADYImage, 0, 1, readyFadeLength));
        StartCoroutine(FadeTo(countdownREADYImage, readyFadeLength + 0.05f, 0, readyFullALphaDuration));
        StartCoroutine(FadeTo(countdownGOImage, readyFullALphaDuration+readyFadeLength+0.05f, 0, GoFadeLength));
    }

}
