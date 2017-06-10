using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour {

    public float shrinkSpeed = 1;
    public Vector3 curveDirection;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale *= 1 - shrinkSpeed * Time.deltaTime;
        if(transform.localScale.x <= .1f)
        {
            Destroy(gameObject);
        }
	}
}
