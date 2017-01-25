using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public float chaseSpeed = 0.1f;
    private Ball ball;
	// Use this for initialization
	void Start () {
        ball = FindObjectOfType<Ball>();
	}

    private void FixedUpdate()
    {
        //TODO do a smooth lerp? or lerp velocities instead? currently looks jumpy. same as soulbound actually >.<
        transform.position = Vector3.Lerp(transform.position, ball.transform.position, chaseSpeed);
    } 
}
