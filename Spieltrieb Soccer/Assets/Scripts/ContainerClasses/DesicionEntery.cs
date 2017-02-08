using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionEntery{

    public int weight;
    public int decisionPosition; //does a forward or a defender or both care about this desicion?
    public int name; //based off of the dictionary in game manager.
    public Player target;

    public DecisionEntery(int initWeight, int initName)
    {
        name = initName;
        weight = initWeight;
        decisionPosition = 0;
    }

    public DecisionEntery(int initName)
    {
        name = initName;
        decisionPosition = 0;
    }
}
