using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {

    public List<Transform> views;
    public Transform deadView;

    public float lerpSpeed = 1;
    float lerpAmount = 0;

    int viewIndex = 0;

    [HideInInspector]
    public bool spinMode = false;
    [HideInInspector]
    public Transform spinTarget;
    public float spinHeight = 5;
    public float spinDistance = 5;
    public float spinSpeed = 1;
    float spinAngle = 0;
    bool dead = false;

	// Use this for initialization
	void Start () {
		
	}

    public void HoistedByOwnPetard()
    {
        dead = true;
        lerpAmount = 1;
    }
	
	// Update is called once per frame
	void Update () {
        if (spinMode)
        {
            Vector3 lookDir = Vector3.up * spinHeight +
                Quaternion.Euler(0, spinAngle, 0) * Vector3.forward * spinDistance;
            transform.position = spinTarget.position + lookDir;
            transform.rotation = Quaternion.LookRotation(-lookDir.normalized);
            spinAngle = (spinAngle + spinSpeed * Time.deltaTime * 360) % 360;
        } else if (dead)
        {
            lerpAmount = Mathf.Max(0, lerpAmount - lerpSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(deadView.position, views[viewIndex].position, lerpAmount);
            transform.rotation = Quaternion.Lerp(deadView.rotation, views[viewIndex].rotation, lerpAmount);
        }
        else
        {
            if (Input.GetButtonDown("SwapCamera"))
            {
                viewIndex = (viewIndex + 1) % views.Count;
                lerpAmount = 1;
            }

            lerpAmount = Mathf.Max(0, lerpAmount - lerpSpeed * Time.deltaTime);
            int prevIndex = (viewIndex + views.Count - 1) % views.Count;
            transform.position = Vector3.Lerp(views[viewIndex].position, views[prevIndex].position, lerpAmount);
            transform.rotation = Quaternion.Lerp(views[viewIndex].rotation, views[prevIndex].rotation, lerpAmount);
        }
	}
}
