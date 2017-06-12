using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrower : MonoBehaviour {

    public GameObject ballPrefab;
    public GameObject petardPrefab;

    public float tossForce = 5;

    public Vector3 spinForce;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//if(Input.GetButtonDown("Fire1")) { tossBall(transform.forward * tossForce, spinForce); }
	}

    public GameObject tossPetard(Vector3 direction, Vector3 spin)
    {
        GameObject ball = Instantiate<GameObject>(petardPrefab);
        ball.transform.position = transform.position;
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 999999;
        rb.AddForce(direction, ForceMode.VelocityChange);
        rb.AddTorque(spin, ForceMode.VelocityChange);
        return ball;
    }

    public GameObject tossBall(Vector3 direction, Vector3 spin)
    {
        GameObject ball = Instantiate<GameObject>(ballPrefab);
        ball.transform.position = transform.position;
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 999999;
        rb.AddForce(direction, ForceMode.VelocityChange);
        rb.AddTorque(spin, ForceMode.VelocityChange);
        return ball;
    }
}
