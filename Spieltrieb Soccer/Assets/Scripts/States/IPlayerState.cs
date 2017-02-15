using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// interface for what states should look like. 
/// </summary>
public interface IPlayerState {
        
        //default state actions. frame by fram actions.
        void UpdateState();
        
        //when the player gains possession of the ball trigger these actions.
        void OnBallPossession();

        void ToPlayerChaseBall();

        void ToPlayerDesicion();

        void ToPlayerRunAndDribble();

        void ToPlayerWait();

        string ReturnNameString();
}
