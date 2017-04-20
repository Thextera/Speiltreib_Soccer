
using UnityEngine;
using System.Collections;

public static class RigidbodyExtension
{

    static Vector2 velocity;
    static float angularVelocity;

    public static void Pause(this Rigidbody2D rb)
    {
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;
        rb.isKinematic = true;
    }

    public static void UnPause(this Rigidbody2D rb)
    {
        rb.isKinematic = false;
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
    }
}