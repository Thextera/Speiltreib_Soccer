using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public float chaseSpeed = 5;
    public float slowMoChaseSpeed = 15;
    public Ball ball;
    private float transformX;
    private float transformY;
    // Use this for initialization
    void Start () {
        ball = FindObjectOfType<Ball>();
	}

    private void Update()
    {
        if(Time.timeScale == 1)
        {
            transformX = Mathf.Lerp(transform.position.x, ball.transform.position.x, Mathf.SmoothStep(0.0f, 1.0f, Time.smoothDeltaTime * chaseSpeed));
            transformY = Mathf.Lerp(transform.position.y, ball.transform.position.y, Mathf.SmoothStep(0.0f, 1.0f, Time.smoothDeltaTime * chaseSpeed));
            //TODO do a smooth lerp? or lerp velocities instead? currently looks jumpy. same as soulbound actually >.<
            transform.position = new Vector2(transformX, transformY);
        }
        else
        {
            transformX = Mathf.Lerp(transform.position.x, ball.transform.position.x, Mathf.SmoothStep(0.0f, 1.0f, Time.smoothDeltaTime * slowMoChaseSpeed));
            transformY = Mathf.Lerp(transform.position.y, ball.transform.position.y, Mathf.SmoothStep(0.0f, 1.0f, Time.smoothDeltaTime * slowMoChaseSpeed));
            //TODO do a smooth lerp? or lerp velocities instead? currently looks jumpy. same as soulbound actually >.<
            transform.position = new Vector2(transformX, transformY);
        }

    }
}
