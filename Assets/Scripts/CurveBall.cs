using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveBall : MonoBehaviour {

    public float curvePower = 1;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float velMag = rb.velocity.magnitude;
        Vector3 velDirection = rb.velocity.normalized;
        Vector3 curveDirection = (rb.GetPointVelocity(transform.position + velDirection) - rb.velocity);
        rb.AddForce((curveDirection - velDirection) * velMag * curvePower, ForceMode.Acceleration);

        if (transform.position.y < -1)
        {
            Destroy(gameObject);
        }
    }
}
