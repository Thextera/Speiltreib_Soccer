using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalZone : MonoBehaviour {

    public int team;
    public Vector2 goalPostOne;
    public Vector2 goalPostTwo;
    public Vector2 goalPostOneField;
    public Vector2 goalPostTwoField;
    //facing right variable used to tell the net what direction its facing. this is used when calculating shots for the AI. (they use the goalposts as references... they will later use the goalies location as well.)
    public bool facingRight = true;

    void Start()
    {
        //TODO changing the sprite might have an odd effect on these calculations. may have to find a better way to do them once art comes in.

        //grap teh sprite renderer and get the size of the object.
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if(!facingRight)
        {
            sr.color = Color.red;
        }
        float sizex = sr.bounds.size.x;
        float sizey = sr.bounds.size.y;

        //use the sizes of the object to locate the x / y values of boundaries of the objects.
        float topFace = transform.position.y + (sizey / 2);
        float bottomFace = transform.position.y - (sizey / 2);
        float rightFace = transform.position.x + (sizex / 2);
        float leftFace = transform.position.x - (sizex / 2);

        //use the boundary values to calculate teh coordinates of the outward facing goalposts.
        goalPostOne.y = topFace;
        goalPostTwo.y = bottomFace;

        //values will differ if the goal is on the right or left side of the feild.
        if (facingRight)
        {
            team = GameManager.Instance.teams["Right"];
            goalPostOne.x = rightFace;
            goalPostTwo.x = rightFace;
        }
        else
        {
            team = GameManager.Instance.teams["Left"];
            goalPostOne.x = leftFace;
            goalPostTwo.x = leftFace;
        }

        //convert goal locations to gridspace for easy access later.
        //goalPostOneField = Field.Instance.ConvertGlobalToField(goalPostOne);
        //goalPostTwoField = Field.Instance.ConvertGlobalToField(goalPostTwo);
    }

    //wait untill something bumps the goal zone.
    void OnTriggerEnter(Collider col)
    {
        //check if the object that collided was the ball. if so then call a goal.
        if(col.gameObject.GetComponent<Ball>() != null)
        {
            EventManager.Instance.Goal(team);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (goalPostOne != Vector2.zero && goalPostTwo != Vector2.zero)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(goalPostOne, goalPostTwo);
        }
    }
}
