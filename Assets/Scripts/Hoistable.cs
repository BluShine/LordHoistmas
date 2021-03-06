﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoistable : MonoBehaviour {

    bool hoisting = false;
    [HideInInspector]
    public float maxHoist = -1;
    [HideInInspector]
    public bool finished = false;
    public int team = 0;

    LineRenderer line;
    Rigidbody body;
    [HideInInspector]
    public Vector3 highestPoint;
    List<Vector3> points;
    Vector3 lastPoint;
    float minTime = 1;

	// Use this for initialization
	void Start () {
        line = GetComponent<LineRenderer>();
        body = GetComponent<Rigidbody>();
        highestPoint = Vector3.zero;
        lastPoint = Vector3.zero;
        points = new List<Vector3>();
	}

    public void finish()
    {
        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
        body.constraints = RigidbodyConstraints.FreezeAll;
        transform.position = highestPoint;
    }

    public void startHoisting()
    {
        hoisting = true;
    }
	
	// Update is called once per frame
	void Update () {
		if(hoisting)
        {
            minTime -= Time.deltaTime;
            if(transform.position.y > maxHoist)
            {
                maxHoist = transform.position.y;
                highestPoint = transform.position;
            }
            if ((lastPoint - transform.position).sqrMagnitude > .02f)
            {
                points.Add(transform.position);
                lastPoint = transform.position;
            }
            if(transform.position.y < -.1 || (minTime <= 0 && body.velocity.sqrMagnitude < .1f))
            {
                hoisting = false;
                finished = true;
                if(transform.position.y < -.1)
                {
                    body.constraints = RigidbodyConstraints.FreezeAll;
                }
                FindObjectOfType<MainUI>().finishHoist(this);
            }
        }
	}
}
