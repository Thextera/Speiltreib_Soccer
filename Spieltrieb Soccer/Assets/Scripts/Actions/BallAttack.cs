using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallAttack : MonoBehaviour {

    public SpriteRenderer screenDarken;
    public Image abilityPlaque;
    public Text abilityTitle;
    private Ball b;
    private Rigidbody2D ballBody;
    private AttackDef currentAttackDef;
    private Player currentAttacker;
    public CircleCollider2D AOECol;

    private bool damaging;
    private bool AOEDamaging;

    public void Start()
    {
        b = GetComponent<Ball>();
        ballBody = GetComponent<Rigidbody2D>();
        screenDarken.color = new Color(0,0,0,0);
        abilityPlaque.color = new Color(1, 1, 1, 0);
        abilityTitle.color = new Color(0, 0, 0, 0);
    }



    //tells the ball to be damaging for a period. sets the area of effects and other valuable effects.
    public void CalibrateAttack(AttackDef def)
    {
        damaging = true;

        if(def.GetAOE())
        {
            AOEDamaging = true;
            AOECol.radius = def.GetAOERadius();
        }

        Invoke("ResetAll", def.GetDuration());
    }

    //heavily toned down calibrate attack. sets teh ball to be a default attack with nothing special about it.
    public void DefaultAttack(float duration, float damage, Player pl)
    {
        damaging = true;
        currentAttacker = pl;
        Invoke("ResetAll", duration);
    }

    //trigger various attack types based upon the ability definition.
    public void TriggerAttack(AttackDef def, Player owner)
    {
        print("Attack TRIGGERED");
        //set current user
        currentAttackDef = def;
        currentAttacker = owner;

        //calibrate the attacks values.
        CalibrateAttack(def);

        //use attack stats received to set off attack and trigger animation.
        def.TriggerEffect(b);
        TriggerDefaultAnim(1.0f, def);
    }

    public void TriggerHeal(AttackDef def, Player owner)
    {
        //set current user
        currentAttackDef = def;
        currentAttacker = owner;

        //calibrate the attacks values.
        CalibrateAttack(def);

        def.TriggerEffect(b);
        TriggerDefaultAnim(1.0f, def);
    }

    public void TriggerRevive(AttackDef def, Player owner)
    {
        //set current user
        currentAttackDef = def;
        currentAttacker = owner;

        //calibrate the attacks values.
        CalibrateAttack(def);

        def.TriggerEffect(b);
        TriggerDefaultAnim(1.0f, def);
    }

    public void TriggerBuff(AttackDef def, Player owner)
    {
        //set current user
        currentAttackDef = def;
        currentAttacker = owner;

        //calibrate the attacks values.
        CalibrateAttack(def);

        def.TriggerEffect(b);
        TriggerDefaultAnim(1.0f, def);
    } 

    public void TriggerDebuff(AttackDef def, Player owner)
    {
        //set current user
        currentAttackDef = def;
        currentAttacker = owner;

        //calibrate the attacks values.
        CalibrateAttack(def);

        def.TriggerEffect(b);
        TriggerDefaultAnim(1.0f, def);
    }

    /// <summary>
    /// the default animation for an attack being a slow, zoom and background dim. 
    /// </summary>
    /// <param name="duration">how long should DefaultAnim last?</param>
    public void TriggerDefaultAnim(float duration, AttackDef def)
    {
        print("AnimTriggered");

        //darken the screen to focus the player on the attack.
        StartCoroutine(FadeTo(0.5f,GameManager.Instance.universalAnimationDuration));

        //slow the time down to add to the effect of the attack
        Time.timeScale = 0.2f;

        //manage the ability plate. this will pass vluable info to the player.
        abilityPlaque.color = new Color(1,1,1,1);
        abilityTitle.color = new Color(0,0,0,1);
        abilityTitle.text = def.name;

        //pause the ball.
        ballBody.Pause();
        b.PauseBall();
        Invoke("Unpause", GameManager.Instance.universalAnimationDuration);

        Invoke("ResetAllEffects",def.GetDuration());

    }

    private void Unpause()
    {
        ballBody.UnPause();
        b.UnpauseBall();
    }

    /// <summary>
    /// resets all default attack visuals.
    /// </summary>
    private void ResetAllEffects()
    {
        StartCoroutine(FadeTo(0, 0.33f));
        abilityPlaque.color = new Color(1, 1, 1, 0);
        abilityTitle.color = new Color(0, 0, 0, 0);
    }

    private void ResetAll()
    {
        //after the attack has been executed rest all related variables to return play to normal.
        Time.timeScale = 1;
        currentAttackDef = null;
        damaging = false;
        AOEDamaging = false;
        AOECol.radius = 0.1f;
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {

        float alpha = screenDarken.color.a;
        for (float t = 0.0f; t < 1f; t += Time.deltaTime / Time.timeScale / aTime)
        {
            Color newColor = new Color(0, 0, 0, Mathf.Lerp(alpha, aValue, t));
            screenDarken.color = newColor;
            yield return null;
        }
    }

    //when something is caught in the AOE of the attack.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //check if im allowed to damage, then check if the target i bumped into is a player, finally check if that player is on my team.
        //perhaps this could be done more efficiently.
        if (AOEDamaging && collision.gameObject.GetComponent<Player>()!= null && collision.gameObject.GetComponent<Player>().team != currentAttacker.team)
        {
            collision.gameObject.SendMessage("TakeDamage", currentAttackDef.GetAOEDamage());
            print("AAAAAH IT BURNS! DD:");
        }
    }

    //when something collides with the ball.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //check if im allowed to damage, then check if the target i bumped into is a player, finally check if that player is on my team.
        //perhaps this could be done more efficiently.
        if (damaging && collision.gameObject.GetComponent<Player>() != null && collision.gameObject.GetComponent<Player>().team != currentAttacker.team)
        {
            collision.gameObject.SendMessage("TakeDamage", currentAttackDef.GetDamage());
            print("IM MELTING! D:");
        }
    }


}
