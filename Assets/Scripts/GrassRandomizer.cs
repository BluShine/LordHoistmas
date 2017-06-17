using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassRandomizer : MonoBehaviour {

    public Gradient colors;
    public float scaleRandomness = 2;

	// Use this for initialization
	void Start () {
        foreach (SpriteRenderer s in GetComponentsInChildren<SpriteRenderer>())
        {
            s.color = colors.Evaluate(Random.value);
        }
        transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        transform.localScale = transform.localScale * Random.Range(1f, scaleRandomness);
	}
}
