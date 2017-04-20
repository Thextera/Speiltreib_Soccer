using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionEntery{

    public int weight;
    public int name; //based off of the dictionary in game manager.
    public Player target;
    public Vector2 shotTarget;
    public string special;

    public DecisionEntery(int initWeight, int initName)
    {
        name = initName;
        weight = initWeight;
    }

    public DecisionEntery(int initName)
    {
        name = initName;
    }
}
