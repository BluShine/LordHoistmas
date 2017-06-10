using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollider : MonoBehaviour {

    public ThrowUI throwUI;

    private void OnTriggerEnter(Collider other)
    {
        if(other)
        throwUI.hit(other);
    }
}
