using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour {

    public GameObject explosionSprite;
    public float explosionTime = .1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        explosionTime -= Time.deltaTime;
        if(explosionTime <= 0)
        {
            explosionSprite.SetActive(false);
        }
	}
}
