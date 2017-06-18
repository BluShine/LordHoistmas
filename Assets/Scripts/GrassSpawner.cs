using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSpawner : MonoBehaviour {

    public Vector2 area;
    public List<GameObject> prefabs;
    public float count = 100;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < count; i++)
        {
            GameObject g = Instantiate<GameObject>(prefabs[Random.Range(0, prefabs.Count)]);
            g.transform.position = transform.position + new Vector3(Random.Range(-1f, 1f) * area.x, 0, Random.Range(-1f, 1f) * area.y);
        }
	}

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position + new Vector3(area.x, 0, area.y), transform.position + new Vector3(area.x, 0, -area.y), Color.red);
        Debug.DrawLine(transform.position + new Vector3(area.x, 0, -area.y), transform.position + new Vector3(-area.x, 0, -area.y), Color.red);
        Debug.DrawLine(transform.position + new Vector3(-area.x, 0, -area.y), transform.position + new Vector3(-area.x, 0, area.y), Color.red);
        Debug.DrawLine(transform.position + new Vector3(-area.x, 0, area.y), transform.position + new Vector3(area.x, 0, area.y), Color.red);
    }
}
