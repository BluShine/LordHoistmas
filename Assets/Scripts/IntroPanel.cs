using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPanel : MonoBehaviour {

    public float waitSeconds = 1;
    public float slideSpeed = 1;
    public float slideOffset = 200;
    float timeCounter = 0;

    Vector3 startPos;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        timeCounter += Time.deltaTime;
        if(timeCounter > waitSeconds)
        {
            transform.position = startPos + Vector3.right * Mathf.Min(1, (timeCounter - waitSeconds) / slideSpeed) * slideOffset;
        }
	}
}
