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

    public void Start()
    {
        b = GetComponent<Ball>();
        ballBody = GetComponent<Rigidbody2D>();
        screenDarken.color = new Color(0,0,0,0);
        abilityPlaque.color = new Color(1, 1, 1, 0);
        abilityTitle.color = new Color(0, 0, 0, 0);
    }

    //trigger various attack types based upon the ability definition.
    public void TriggerAttack(AttackDef def, Player owner)
    {
        print("Attack TRIGGERED");
        //use attack stats received to set off attack and trigger animation.
        def.TriggerEffect(b);
        currentAttackDef = def;
        TriggerDefaultAnim(1.0f, def);
    }

    public void TriggerHeal(AttackDef def, Player owner)
    {
        def.TriggerEffect(b);
        currentAttackDef = def;
        TriggerDefaultAnim(1.0f, def);
    }

    public void TriggerRevive(AttackDef def, Player owner)
    {
        def.TriggerEffect(b);
        currentAttackDef = def;
        TriggerDefaultAnim(1.0f, def);
    }

    public void TriggerBuff(AttackDef def, Player owner)
    {
        def.TriggerEffect(b);
        currentAttackDef = def;
        TriggerDefaultAnim(1.0f, def);
    } 

    public void TriggerDebuff(AttackDef def, Player owner)
    {
        def.TriggerEffect(b);
        currentAttackDef = def;
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

        Invoke("ResetAll", 1f);

    }

    private void Unpause()
    {
        ballBody.UnPause();
        b.UnpauseBall();
    }

    private void ResetAll()
    {
        //after the attack has been executed rest all related variables to return play to normal.
        StartCoroutine(FadeTo(0, 0.33f));
        abilityPlaque.color = new Color(1, 1, 1, 0);
        abilityTitle.color = new Color(0, 0, 0, 0);
        Time.timeScale = 1;
        currentAttackDef = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentAttackDef != null)
        {
            collision.gameObject.SendMessage("TakeDamage", currentAttackDef.GetDamage());
        }
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
}
