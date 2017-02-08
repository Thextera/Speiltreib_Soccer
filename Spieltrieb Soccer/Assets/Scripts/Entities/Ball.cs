﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<Summary>
///container for all information related to the ball.
///</summary>
public class Ball : MonoBehaviour {

    [Header("physics values")]
    public float frictionDragPersentage;
    public float frictionDragFlat;
    public float angularFriction; //friction that effects the rotation of the ball.
    public float energyLostOnCollision;
    public float angVelLostOnCollision;

    [Header("Vel Stop Thresholds")]
    public float velXStop = 0.1f;
    public float velYStop = 0.1f;
    public float VelAngStop = 1;
    public float rotationalThreshold = 25;

    private bool possesed; //is a player possessing the ball?
    Rigidbody2D rb;


	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        #region LooseBall
        if (possesed == false)
        {
            //natural drag below. as a ball normally would this ball must slow down when rolling. unity's physics 
            //only applies when a player is not possesing the ball.
            //cant do this for us so It is done below.
            //x and y portions of this must be done seperate.
            #region velocityX

            if (rb.velocity.x > velXStop || rb.velocity.x < -velXStop)
            {
                //if our velocity is fast slow it a little.
                if (rb.velocity.x > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x * frictionDragPersentage - frictionDragFlat, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x * frictionDragPersentage + frictionDragFlat, rb.velocity.y);
                }
            }
            else if(rb.velocity.x < frictionDragFlat || rb.velocity.x > -frictionDragFlat)
            {
                rb.velocity = new Vector2(0,rb.velocity.y);
            }


            #endregion

            #region velocityY

            if (rb.velocity.y > velYStop || rb.velocity.y < -velYStop)
            {
                //if our velocity is fast slow it a little.
                if (rb.velocity.y > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * frictionDragPersentage - frictionDragFlat);
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * frictionDragPersentage + frictionDragFlat);
                }
            }
            else if (rb.velocity.y < frictionDragFlat || rb.velocity.y > -frictionDragFlat)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }

            #endregion

            #region velocityAng
            //going to do the same thing with rotational velocity too. 
            if (rb.angularVelocity > VelAngStop)
            {
                rb.angularVelocity = rb.angularVelocity * angularFriction;
            }
            else
            {
                rb.angularVelocity = 0;
            }

            #endregion

            //print(rb.velocity);
            //EXPERIMENTAL
            //ading a small ammount of force based upon the angular momentum of the ball.

            // i guess ill figure this out later... this is apperntly quite the conundrum... O.o

            //in addition to this maybe add height. expand size to give the illusion height.
            
            #endregion
        }
    }

    //when a ball hits an object, it makes a sound and generates heat. this means lost energy, 
    //this function adds that energy loss to the balls physics.
    private void OnCollisionEnter2D(Collision2D col)
    {
        rb.velocity = new Vector2(rb.velocity.x * energyLostOnCollision, rb.velocity.y * energyLostOnCollision);
        //rb.angularVelocity = rb.angularVelocity * angVelLostOnCollision;
    }

    public void PossessBall(GameObject possessingUnit)
    {
        possesed = true;
        rb.isKinematic = true;
        transform.parent = possessingUnit.transform;
        EventManager.Instance.BallPossessed();
    }

    public void UnpossessBall()
    {
        possesed = false;
        rb.isKinematic = false;
        transform.parent = null;
    }

    public bool IsPossessed()
    {
        return possesed;
    }
}
