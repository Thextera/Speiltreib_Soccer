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

    bool userPollFlag = false;
    bool slowFlag = false;
    private float slowTimer;

    public Image timerImage;

    public void TriggerGameSlow(PlayerStatePattern p)
    {
        Debug.LogWarning("IT WAS ME, DIO");
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

        slowTimer = GameManager.Instance.timeSlowDuration;

        //enable player choice UI. (un-hide or un-grey or whatever needs to be done.)
        //display options based on plaer. (If attack show X, if defence show Y)
        //Display Specials as well...?
        //engage while loop and wait for user input.
        yield return null;
        timerImage.fillAmount = 1;

        while (slowTimer > 0)
        {
            Debug.LogWarning("Doing a thing" + slowTimer + Time.deltaTime);
            slowTimer -= Time.deltaTime;
            timerImage.fillAmount = (slowTimer / GameManager.Instance.timeSlowDuration);
            yield return null;
        }
        Debug.LogWarning("Finished a thing");
        //take user input and send back in the form of callback.
        //if there is no input or input is invalid simply return null. put UI element on screen saying "TOO SLOW!"

        userPollFlag = false;
        yield return null;
    }
}
