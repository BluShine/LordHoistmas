using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunSpinner : MonoBehaviour {

    public Vector3 rotatingSpeed;
    Vector3 rotate;

    void Start()
    {
        rotate = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update () {
        rotate += rotatingSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(rotate);
	}
}
