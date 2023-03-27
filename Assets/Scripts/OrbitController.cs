using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
    [Header("Orbit Parameters")]
    public float inclination;
    public float raan;
    public float altitude;
    public float startAnomaly;
    public float anomaly;

    [Header("Oscillation Parameters")]
    public float frequency = 0.5f;
    public float magnitude = 10f;


    [Header("Simulation Parameters")]
    public float duration = 10;
    public Transform sat;
    public Transform orbitFixedAxis;
    public Transform orbitalClone;
    public Transform inertialClone;
    public bool pause;

    [Header("Debug Parameters")]
    public float A;
    public float B;

    private Quaternion body_to_orbitfixed;
    private Quaternion anomalyRotation;
    private float stepDegree;
    private Vector3 orbitNormal;
    private Quaternion body_to_inertial;
    private Quaternion body_to_orbital;
    private float acc_time;

    // Start is called before the first frame update
    void Start()
    {
        stepDegree = (duration / Time.fixedDeltaTime) / 360;
        anomaly = startAnomaly;
        acc_time = 0;

        //Vector3 voriginal = new Vector3(0, 1, 0);
        //print("Voriginal: " + voriginal);
        //Quaternion qa = new Quaternion(0, Mathf.Sqrt(2)/2, 0, Mathf.Sqrt(2)/2);
        //print("qa: " + qa);
        //Quaternion qb = new Quaternion(Mathf.Sqrt(2)/2, 0, 0, Mathf.Sqrt(2)/2);
        //print("qb: " + qb);

        //voriginal = qa * voriginal;
        //print("After Applying qa:" + voriginal);

        //voriginal = qb * voriginal;
        //print("After Applying qb:" + voriginal);

        //print("Reset");
        //voriginal = new Vector3(0, 1, 0);
        //Quaternion qab = qa * qb;
        //print("After Applying (qa * qb):" + ( qab * voriginal ));

        //print("Reset");
        //voriginal = new Vector3(0, 1, 0);
        //Quaternion qba = qb * qa;
        //print("After Applying (qb * qa):" + ( qba * voriginal ));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Using Orbital Parameters
        body_to_orbitfixed = Quaternion.AngleAxis(raan, new Vector3(0, -1, 0)) * Quaternion.AngleAxis(inclination, new Vector3(-1, 0, 0));
        orbitNormal = new Vector3(0, 1, 0);
        orbitNormal = body_to_orbitfixed * orbitNormal;
        if(!pause) anomaly += stepDegree;
        if(anomaly > 360) anomaly = 0;

        // Initialize
        body_to_inertial = new Quaternion(0, 0, 0, 1);
        body_to_orbital  = new Quaternion(0, 0, 0, 1);

        // Define body to orbital
        acc_time += Time.fixedDeltaTime;
        body_to_orbital = Quaternion.AngleAxis(
                magnitude * Mathf.Sin(2*Mathf.PI*frequency*acc_time),
                new Vector3(1, 0, 0)
                );

        // Define body to inertial
        body_to_inertial =  body_to_orbitfixed
                          * Quaternion.AngleAxis(anomaly, -(new Vector3(0, 1, 0)))
                          * Quaternion.AngleAxis( 90, -(new Vector3(0, 1, 0))) 
                          * Quaternion.AngleAxis(-90, -(new Vector3(1, 0, 0)))
                          * body_to_orbital;

        orbitFixedAxis.rotation = body_to_orbitfixed;

        // Apply to satellite object
        sat.position  = body_to_orbitfixed
                        * Quaternion.AngleAxis(anomaly, -new Vector3(0, 1, 0))
                        * (new Vector3(altitude, 0, 0));
        sat.rotation  = body_to_inertial;

        // Apply to inertial/orbital clones
        orbitalClone.rotation  = body_to_orbital;
        inertialClone.rotation = body_to_inertial;
    }
}
