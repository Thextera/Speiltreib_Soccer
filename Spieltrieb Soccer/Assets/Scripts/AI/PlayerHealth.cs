using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    private Player p;
    public Image healthForeground;
    public Image healthDamageTransition;
    public Image healthHealTransition;
    public Text healthName;

    public float fillSpeed = 0.015f;
    public float healthPanelOffsetX = 0.35f;
    public float healthPanelOffsetY = 0.35f;

    public float lastKnownHealth;

    private bool heal;
    private bool damage;

    private void Start()
    {
        //move the hpbar into the right branch of the UI
        transform.SetParent(PlayerManager.Instance.playerHealthHolder.transform);
        //initialize frame late health var
        lastKnownHealth = p.currentHealth;
        //properlyu set the players name.
        healthName.text = p.playerName;
    }

    void Update()
    {
        //dont bother showing HP if the players health is full.
        if(healthForeground.fillAmount > 0.991f)
        {
            DisableChildren();
        }
        else
        {
            EnableChildren();
        }

        #region HealthbarTransitions
        //always set heal value to healthbar.
        healthHealTransition.fillAmount = p.currentHealth / p.maxHealth;

        //move the transition bar to match the regular bar.
        if(p.currentHealth < lastKnownHealth)
        {
            print("Damage " + p.currentHealth / p.maxHealth);
            //damage was taken. show damage transition
            //dont transition foreground, only transition damage
            healthForeground.fillAmount = p.currentHealth / p.maxHealth;
            damage = true;
        }
        else if(p.currentHealth > lastKnownHealth)
        {
            print("Heal " + p.currentHealth / p.maxHealth);
            //healing was done. show damage transition
            //dont transition damage, only transition foreground.
            healthDamageTransition.fillAmount = p.currentHealth / p.maxHealth;
            heal = true;
        }



        //if we know healing was done then...
        if (heal)
        {
            //transition a fixed amount towards the current HP
            healthForeground.fillAmount = healthForeground.fillAmount + fillSpeed;

            //if the transiton is close enough to the actuall ammount set it to the ammount and end the loop.
            if (healthForeground.fillAmount >= (p.currentHealth / p.maxHealth) - fillSpeed)
            {
                healthForeground.fillAmount = p.currentHealth / p.maxHealth;
                heal = false;
            }
        }

        //if we know damage was done then...
        if (damage)
        {
            //transition a fixed amount towards the current HP
            healthDamageTransition.fillAmount = healthDamageTransition.fillAmount - fillSpeed;

            //if the transiton is close enough to the actuall ammount set it to the ammount and end the loop.
            if (healthDamageTransition.fillAmount <= (p.currentHealth / p.maxHealth) + fillSpeed)
            {
                healthDamageTransition.fillAmount = p.currentHealth / p.maxHealth;
                damage = false;
            }
        }

        #endregion


        //manage moving the healthbar over this unit.
        Vector3 worldPos = new Vector3(p.transform.position.x + healthPanelOffsetX, p.transform.position.y + healthPanelOffsetY, p.transform.position.z);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        this.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
        

        lastKnownHealth = p.currentHealth;

    }

    /// <summary>
    /// iterates through transform and sets every child object to inactive.
    /// </summary>
    public void DisableChildren()
    {
        foreach(Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// iterates through transform and sets every child object to active.
    /// </summary>
    public void EnableChildren()
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
    }

    public void SetOwner(Player initP)
    {
        p = initP;
    }
}
