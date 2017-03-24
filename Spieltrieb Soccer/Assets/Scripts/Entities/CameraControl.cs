using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    private Dictionary<string, int> camDict;
    private int cameraMode;
    public float chaseSpeed = 5;
    public float slowMoChaseSpeed = 15;
    public Ball ball;
    private float transformX;
    private float transformY;
    // Use this for initialization
    void Start () {
        ball = FindObjectOfType<Ball>();

        camDict = new Dictionary<string, int>();
        camDict.Add("ChaseBall",0);
        camDict.Add("TeamSelect",1);

        cameraMode = camDict["ChaseBall"];
	}

    /// <summary>
    /// forces the camera to the balls position. 
    /// </summary>
    public void HardCameraResetToBall()
    {
        transform.position = ball.transform.position;
    }

    /// <summary>
    /// changes the camera mode. modes dictate the behavior of the camera and can be found in the camDict.
    /// </summary>
    /// <param name="mode">Name of the camera mode to change to.</param>
    public void ChangeCameraMode(string mode)
    {
        cameraMode = camDict[mode];
    } 

    private void Update()
    {
        //only use certain behavior if the camera is told to be in a certain mode.
        switch (cameraMode)
        {
            //the camera is told to follow the ball. lets play!
            case 0:
        if (Time.timeScale == 1)
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
        break;
                //the camera is in GUI mode. It will show a portion of the feild and remain fixed.
            case 1:
                break;
    }

    }
}
