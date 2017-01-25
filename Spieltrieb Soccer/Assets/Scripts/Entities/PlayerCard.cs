using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a card object used by the UI to help display players to the user.
/// includes only base stats and stats not modified by abilities.
/// </summary>
public class PlayerCard {


    public float speed;
    public float attack;
    public float defence;
    public float shoot;
    public float pass;
    public float dribble;

    public string playerName;
    public int position;
    public int jerseyNumber;

    //add three skills here.

    public PlayerCard(float initSpeed, float initAttack, float initDefence, float initShoot, float initPass, float initDribble, string initPlayerName, int initPosition, int initJersyNumber /* also somehow add skills here!?*/)
    {
        speed = initSpeed;
        attack = initAttack;
        defence = initDefence;
        shoot = initShoot;
        pass = initPass;
        dribble = initDribble;
        playerName = initPlayerName;

        playerName = initPlayerName;
        position = initPosition;
        jerseyNumber = initJersyNumber;
    }
}
