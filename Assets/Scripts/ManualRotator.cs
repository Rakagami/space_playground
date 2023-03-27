using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualRotator : MonoBehaviour
{
    public Vector3 rotationAxis;

    public float duration;

    public bool reset;

    private Quaternion rotationStep;

    void Start()
    {
        gameObject.transform.rotation = Quaternion.identity;
        reset = false;
    }

    void FixedUpdate()
    {
        rotationStep = Quaternion.AngleAxis(360 / (duration / Time.fixedDeltaTime), rotationAxis);
        gameObject.transform.rotation = rotationStep * gameObject.transform.rotation;
        if(reset) {
            gameObject.transform.rotation = Quaternion.identity;
            reset = false;
        }
    }
}
