using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallThrower : MonoBehaviour {

    public GameObject player1BallPrefab;
    public GameObject player2BallPrefab;
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

    GameObject tossPrefab(GameObject prefab, Vector3 direction, Vector3 spin)
    {
        GameObject ball = Instantiate<GameObject>(prefab);
        ball.transform.position = transform.position;
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 999999;
        rb.AddForce(direction, ForceMode.VelocityChange);
        rb.AddTorque(spin, ForceMode.VelocityChange);
        return ball;
    }

    public GameObject tossPetard(Vector3 direction, Vector3 spin)
    {
        return tossPrefab(petardPrefab, direction, spin);
    }

    public GameObject tossP1Ball(Vector3 direction, Vector3 spin)
    {
        return tossPrefab(player1BallPrefab, direction, spin);
    }

    public GameObject tossP2Ball(Vector3 direction, Vector3 spin)
    {
        return tossPrefab(player2BallPrefab, direction, spin);
    }
}
