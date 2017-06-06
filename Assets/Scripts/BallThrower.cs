using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrower : MonoBehaviour {

    public GameObject ballPrefab;

    public float tossForce = 5;

    public Vector3 spin;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1")) { tossBall(transform.forward, tossForce); }
	}

    public GameObject tossBall(Vector3 direction, float force)
    {
        GameObject ball = Instantiate<GameObject>(ballPrefab);
        ball.transform.position = transform.position;
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 999999;
        rb.AddForce(direction.normalized * force, ForceMode.VelocityChange);
        rb.AddTorque(spin, ForceMode.VelocityChange);
        return ball;
    }
}
