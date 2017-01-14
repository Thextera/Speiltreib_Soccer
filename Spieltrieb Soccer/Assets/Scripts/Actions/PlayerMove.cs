using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour{


    //TODO GRAB ALL OF THESE VALUES FROM PLAYER CARD CLASS!!!
    public float maxVelocity = 3.5f; //the speed of a player. bassed off of speed stat.
    public float maxAcceleration = 10;
    public float targetRadius = 1; //if we are this close to our target we have arrived.
    public float timeToTarget = 0.1f; //how long we want to take to reach max speed
    public float finalStopVelThreashold = 0.2f;//the minimum velocity the player can travel before they have to stop.
    private Rigidbody2D rb;

    Field field;

    bool atDestination;

    //vlaue to hold a temporary vector. 
    Vector2 destinationVector;


    public int gridy = -1;
    public int gridx = -1;
    public int gridyb = -1;
    public int gridxb = -1;

    //temporary. remove when done testing.
    void Update()
    {
        if(gridy != -1 && gridx != -1)
        {
            MoveTo(new Vector2(gridx, gridy));
            gridx = -1;
            gridy = -1;
        }
        if (gridyb != -1 && gridxb != -1)
        {
            stopMove();
            MoveTo(new Vector2(gridxb, gridyb));
            gridxb = -1;
            gridyb = -1;
        }
    }

    void Start()
    {
        field = FindObjectOfType<Field>();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    /// <summary>
    /// Method to be called when the player needs to immediately stop moving.
    /// </summary>
    public void stopMove()
    {
        //tell the AI it is at its destination so it breaks from its movement coroutine.
        StopAllCoroutines();
        atDestination = true; 
    }


    /// <summary>
    /// moves player to specific grid location. uses previously gathered speed to do calculations.
    /// </summary>
    /// <param name="GridLocation"></param>
    void MoveTo(Vector2 GridLocation)
    {
        //convert grid location to something that the rigidbody can use.
        Vector2 destination = field.ConvertFieldCoordinateToGlobal(GridLocation);

        //begin the moving proccess.
        print("starting routine.");
        StartCoroutine(Move(destination));
    }



    //coroutine that handles mnipulating velocities outside of an update function.  
    IEnumerator Move(Vector2 destination)
    {
        atDestination = false;
        //if the player is NOT at their destination move towards it.
        while(!atDestination)
        {
            print("not at destination");
            print(transform.position.x - destination.x);
            print(transform.position.y - destination.y);
            print(targetRadius);
            //TODO INVESTIGATE POSITIVE/NEGATIVE COORDINATE ISSUES...
            if (Mathf.Abs(transform.position.x - destination.x) > targetRadius && Mathf.Abs(transform.position.y - destination.y) > targetRadius)
            {
                print("Moving");
                //alter velocity here.
                //desired_velocity = normalize(target - position) * max_velocity
                //steering = desired_velocity - velocity
                destinationVector = new Vector2(destination.x - transform.position.x, destination.y - transform.position.y);
                rb.velocity = destinationVector * maxVelocity;
                
            }
            else
            {
                atDestination = true;
            }
            yield return null;

        }
            //if the player is not almost stopped then slow them quickly
            while (Mathf.Abs(rb.velocity.x) > finalStopVelThreashold ||  Mathf.Abs(rb.velocity.y) > finalStopVelThreashold)
            {
            print("slowing");
            //alter velocity here.
            rb.velocity = Vector2.Lerp(rb.velocity,Vector2.zero,maxAcceleration);

                //if somehow we are no longer at destination break from this loop and end the coroutine.
                if(!atDestination)
                {
                    break;
                }
                yield return null;
            }

        //stop the player all together if we are at our destination.
        if (atDestination)
        {
            print("stopping");
            rb.velocity = Vector2.zero;
        }
    }

}
