using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour {

    public float shrinkSpeed = 1;
    public Vector3 curveDirection;

	// Use this for initialization
	void Start () {
        
	}

    public void setSize(float size)
    {
        Transform wall = transform.GetChild(0);
        wall.localScale = new Vector3(1 + 2 * size, wall.localScale.y, wall.localScale.z);
        wall.localPosition = new Vector3((1 - size) * Random.Range(-1f, 1f), wall.localPosition.y, wall.localPosition.z);
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
