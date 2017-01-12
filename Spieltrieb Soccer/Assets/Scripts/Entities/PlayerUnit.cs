using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour {

    public bool hasBall;
    public bool isOpenForPass;
    public PlayerUnit[] unitsOpenForPassing;


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}


    //-----------------------AI-------------------------

    void UpdateState()
    {

    }

    //---------------------Actions----------------------

    //for the system to move players in between plays. eg. reading the feild after a foul.
    public void teleportTo(float x, float y)
    {

    }

    void RunTo(float x, float y)
    {

    }

    void RunToBall()
    {

    }

    void ClaimBall()
    {

    }

    void PassBall()
    {

    }

    void ClearBall()
    {

    }

    void ShootBall()
    {

    }


}
