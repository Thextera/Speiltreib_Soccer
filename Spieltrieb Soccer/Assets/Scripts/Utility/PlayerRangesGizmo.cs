using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangesGizmo : MonoBehaviour {

    public Player p;

    void OnDrawGizmosSelected()
    {
        //UnityEditor.Handles.DrawWireDisc(p.transform.position,Vector3.back,p.targetShortRange);

        //UnityEditor.Handles.DrawWireDisc(p.transform.position, Vector3.back, p.targetMidRange);

        //UnityEditor.Handles.DrawWireDisc(p.transform.position, Vector3.back, p.targetLongRange);
    }
}
