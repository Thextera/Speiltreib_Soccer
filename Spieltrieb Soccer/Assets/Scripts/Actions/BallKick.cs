using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this class if you want to tell the ball to be kicked. 
/// later on different speeds can result in different things such as the ball being on fire or other effects. 
/// if you want to do that, add it in this class.
/// </summary>
public class BallKick : MonoBehaviour {

    public float debugKickX;
    public float debugKickY;
    public float debugKickAng;

    public bool slow;
    Ball ball;
    Rigidbody2D rb;

    //adds force to the ball as if it was kicked.
    public void KickBall(Vector2 force)
    {
        //tell the ball that it is loose and no one has possession of it.
        ball.UnpossessBall();
        //add the force of the kick to the ball.
        rb.AddForce(force);
    }

    //adds force to the ball AND spin as if it was kicked at an angle. 
    void KickBall(Vector2 force, float angSpeed)
    { 
        //tell the ball that it is loose and no one has possession of it.
        ball.UnpossessBall();
        //add the force of the kick to the ball.
        rb.AddForce(force);
        //add rotation if needed.
        rb.angularVelocity = angSpeed;
    }


    // Use this for initialization
    void Start () {
        ball = FindObjectOfType<Ball>();
        rb = ball.gameObject.GetComponent<Rigidbody2D>();
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
        //quick n dirty method to test if the kick function is working.
        if(debugKickX != 0 || debugKickY != 0 || debugKickAng != 0)
        {
            //kick then reset.
            KickBall(new Vector2(debugKickX, debugKickY), debugKickAng);
            debugKickX = 0;
            debugKickY = 0;
            debugKickAng = 0;
        }


        //debug slowing method.
        if (slow)
        {
            Time.timeScale = 0.2f;
        }
        else
        {
            Time.timeScale = 1;
        }
	}
}
