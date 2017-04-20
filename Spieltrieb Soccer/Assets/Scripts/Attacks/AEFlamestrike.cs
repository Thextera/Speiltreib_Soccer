using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// the attack visual effect for the Flamestrike ability.
/// </summary>
public class AEFlamestrike : IAttackEffect
{

    private ParticleSystem[] ps;
    private GameObject effectHolder;

    public float effectDuration = 1;

    public void start()
    { 
}

    public void AttackEffect(Ball b)
    {
        //check all the game objects on the ball of the flamestrike holder.
        foreach(Transform child in b.gameObject.transform)
        {
            //if the name is correct then make our effect holder the same object
            if (child.name == "FlameStrikeADDON")
            {
                effectHolder = child.gameObject;
            }
        }

        //just a check to see if the operation failed. I dont think there is a more simple way to do this unfortunately.
        if(effectHolder == null)
        {
            Debug.LogError("Effect object could not be found on ball. Please check names or this script.");
        }

        //grab all the particleSystems in the holder.
        ps = effectHolder.GetComponentsInChildren<ParticleSystem>();

        //TURN IT ON!
        foreach(ParticleSystem part in ps)
        {
            part.Play();
        }

        //create a couroutine to disable the effects after a period.
        new Task(EndEffects());

    }

    private IEnumerator EndEffects()
    {
        yield return new WaitForSeconds(effectDuration);

        //TURN IT OFF! 
        foreach (ParticleSystem part in ps)
        {
            part.Stop();
        }

        yield return null;
    }
}
