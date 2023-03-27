using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform rotationToCopy;

    void Start()
    {

    }

    void FixedUpdate()
    {
        gameObject.transform.rotation = rotationToCopy.rotation;
    }
}
