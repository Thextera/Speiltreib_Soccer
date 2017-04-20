using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The definition of attack used for the attack manager'd dictionary. To be called when the attack is performed.
/// </summary>
public class AttackDef
{

    private int parentAction; //does this attack count as an attack, a pass, a shoot, etc. Values will be compaired against the GameManager dict.

    private float attackFoundation; //damage the attack does
    private bool AOE;//does the attack have an area effect?
    private float AOERadius; // how wide the effect can be.
    private float duration;//how long the ball is considered "in attack"
    public int abilityType;//what the ability is designed to do
    private float ballKickAmplifier; //how much harder the player will kick the ball using this ability.
    public string name;

    private IAttackEffect effect;

    /// <summary>
    /// constructor, use this when creating an attack. 
    /// </summary>
    /// <param name="initParentAction"></param>
    /// <param name="initAttackType"></param>
    /// <param name="initBallKickAmplifier"></param>
    /// <param name="initAttackFoundation"></param>
    /// <param name="initAOE"></param>
    /// <param name="initAOERadius"></param>
    /// <param name="initDuration"></param>
    /// <param name="initEffect"></param>
    public AttackDef(string initName, int initParentAction, int initAbilityType, float initBallKickAmplifier, float initAttackFoundation, bool initAOE, float initAOERadius, float initDuration, IAttackEffect initEffect)
    {
        parentAction = initParentAction;
        attackFoundation = initAttackFoundation;
        AOE = initAOE;
        AOERadius = initAOERadius;
        duration = initDuration;
        abilityType = initAbilityType;
        ballKickAmplifier = initBallKickAmplifier;
        name = initName;

        effect = initEffect;
    }

    public int GetParentAction()
    {
        return parentAction;
    }

    public void TriggerEffect(Ball b)
    {
        effect.AttackEffect(b);
    }

    public float GetDamage()
    {
        return attackFoundation;
    }
}
