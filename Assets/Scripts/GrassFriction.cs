using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassFriction : MonoBehaviour {

    public float frictionForce = 1f;
    public float angularFrictionForce = 1f;

    void OnTriggerStay(Collider other)
    {
        Rigidbody r = other.attachedRigidbody;
        if (r != null)
        {
            if(r.velocity.magnitude <= frictionForce * Time.fixedDeltaTime)
            {
                r.velocity = Vector3.zero;
            } else
            {
                r.velocity -= r.velocity.normalized * frictionForce * Time.fixedDeltaTime;
            }

            if (r.angularVelocity.magnitude <= angularFrictionForce * Time.fixedDeltaTime)
            {
                r.angularVelocity = Vector3.zero;
            }
            else
            {
                r.angularVelocity -= r.angularVelocity.normalized * angularFrictionForce * Time.fixedDeltaTime;
            }
        }
    }
}
