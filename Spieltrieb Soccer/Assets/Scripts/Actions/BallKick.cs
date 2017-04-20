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

    Ball ball;
    Rigidbody2D rb;

    //adds force to the ball as if it was kicked.
    public void KickBall(Vector2 force)
    {
        print("Kick");
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

}
