using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Petard : MonoBehaviour {

    public float explosionForce = 5;
    public float hoistForce = 5;
    public float radius = 5;

    public GameObject explosionPrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void Hoist()
    {
        foreach(Hoistable h in FindObjectsOfType<Hoistable>())
        {
            h.startHoisting();
            Vector3 distance = h.transform.position - transform.position;
            if (distance.magnitude < radius)
            {
                float forceScale = 1 - distance.magnitude / radius;
                h.GetComponent<Rigidbody>().AddForce((distance.normalized * explosionForce + Vector3.up * hoistForce) * forceScale);
            }
        }
        GetComponent<MeshRenderer>().enabled = false;
        GameObject explosion = Instantiate<GameObject>(explosionPrefab, Vector3.Scale(transform.position, new Vector3(1, 0, 1)), Quaternion.identity);
        //Destroy(gameObject);
    }
}
