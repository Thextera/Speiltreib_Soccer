using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that is used to help initialize a player. effectively a player template.
/// team/position etc.
/// </summary>
public class PlayerInit{

    public readonly PlayerCard pCard;
    public readonly Vector2 fieldStartPosition;
    public readonly int teamNumber;
    public readonly Sprite playerSprite;
    public readonly bool AI;

    /// <summary>
    /// non-defualt constructor.
    /// </summary>
    /// <param name="initpc">player card</param>
    /// <param name="initFeildStartPosition">starting location</param>
    /// <param name="initTeamNumber">team this player starts on</param>
    public PlayerInit(PlayerCard initPCard, Vector2 initFieldStartPosition, int initTeamNumber, Sprite initPlayerSprite, bool initAI)
    {
        //TODO add animator here? maybe?
        playerSprite = initPlayerSprite;
        pCard = initPCard;
        fieldStartPosition = initFieldStartPosition;
        teamNumber = initTeamNumber;
        AI = initAI;
    }
}
