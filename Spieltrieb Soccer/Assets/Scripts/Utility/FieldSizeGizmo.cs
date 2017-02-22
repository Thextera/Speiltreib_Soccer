using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FieldSizeGizmo : MonoBehaviour {

    public Field f;

    float fieldLength;
    float fieldWidth;

    [ExecuteInEditMode]
    void OnDrawGizmos()
    {
        fieldWidth = f.fieldWidth;
        fieldLength = f.fieldLength;
        DrawRect();
    }

    private void DrawRect()
    {
        Vector2 tr = new Vector2(transform.position.x + (fieldWidth / 2), transform.position.y + (fieldLength / 2));
        Vector2 tl = new Vector2(transform.position.x - (fieldWidth / 2), transform.position.y + (fieldLength / 2));
        Vector2 br = new Vector2(transform.position.x + (fieldWidth / 2), transform.position.y - (fieldLength / 2));
        Vector2 bl = new Vector2(transform.position.x - (fieldWidth / 2), transform.position.y - (fieldLength / 2));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(tr, tl);
        Gizmos.DrawLine(br, bl);

        Gizmos.DrawLine(tr, br);
        Gizmos.DrawLine(tl, bl);
    }
}
