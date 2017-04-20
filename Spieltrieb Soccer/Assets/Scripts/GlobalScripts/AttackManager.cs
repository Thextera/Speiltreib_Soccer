using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour {

    #region SINGLETON PATTERN
    //simple singleton pattern. This allows functions in this class to be called globally as THERE CAN BE ONLY ONE!!!
    public static AttackManager _instance;
    public static AttackManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AttackManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("AttackManager");
                    _instance = container.AddComponent<AttackManager>();
                }
            }

            return _instance;
        }
    }
    #endregion

    private Dictionary<string, AttackDef> abilityDict;
    private Dictionary<string, int> abilityTypeDict;
    BallAttack ba;

    private void Awake()
    {
        abilityDict = new Dictionary<string, AttackDef>();
        abilityTypeDict = new Dictionary<string, int>();
    }
    //establish a dictionary of all attacks possible in the game. This will be called by the attack function based upon the playercards attack strings.
    private void Start()
    {

        ba = FindObjectOfType<BallAttack>();

        abilityTypeDict.Add("Attack",0);
        abilityTypeDict.Add("Heal", 1);
        abilityTypeDict.Add("Revive", 2);
        abilityTypeDict.Add("Buff", 3);
        abilityTypeDict.Add("Debuff", 4);

        AEFlamestrike flameStrike = new AEFlamestrike();
        AEDragonkick dragonKick = new AEDragonkick();

        AttackDef fs = new AttackDef("FlameStrike", GameManager.Instance.AIActions["Attack"], abilityTypeDict["Attack"], 1.2f, 34, true, 3, 1.5f, flameStrike);
        AttackDef dk = new AttackDef("DragonKick", GameManager.Instance.AIActions["Attack"], abilityTypeDict["Attack"], 1.2f, 34, false, 0, 1.5f, dragonKick);

        print(fs + " " + dk);
        //this could potentially be exposed via a public array of these enteries. maybe?
        abilityDict.Add("FlameStrike", fs);
        abilityDict.Add("DragonKick", dk);
    }

    public void ExecuteAbility(string attackName, Player owner)
    {
        //tell ballatack to do something based upon the received attack name.
        AttackDef d = abilityDict[attackName];

        //trigger different ability set based on ability type.
        switch(d.abilityType)
        {
            case 0: //attack
                print(attackName + " " + owner);
                ba.TriggerAttack(d, owner);
            break;

            case 1: //heal
                ba.TriggerHeal(d, owner);
            break;

            case 2: //Revive
                ba.TriggerRevive(d, owner);
            break;

            case 3: //buff
                ba.TriggerBuff(d, owner);
            break;

            case 4: //debuff
                ba.TriggerDebuff(d, owner);
            break;
        }
    }

    /// <summary>
    /// returns the integer ID number of an attacks parent. Eg: return 1 means attack. return 2 means shoot. etc.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int GetAttackParentAction(string key)
    {
        int i;

        //we should error handle this bit because values could be entered wrong.
        try
        {
            i = abilityDict[key].GetParentAction();
        }
        catch
        {
            throw new UnityException("Attack Manager Excepton. Attack Dictonary Key didn't match any dictonary entery.");
        }
        return i;
    }

}
