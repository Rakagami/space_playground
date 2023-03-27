using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateCopyRotation : MonoBehaviour
{
    [Header("Parameters")]
    public Transform tocopy;
    public float refreshRate_Hz = 1.0f;
    public bool refreshing;

    [Header("Measurement Noise")]
    public float bias = 0.0f;
    public float std  = 0.01f;

    public Transform debugSphere;

    private Quaternion lastRotation;
    private float timeSinceLastRefresh;

    //private Vector3 RVDelta(Quaternion qa, Quaternion qb, float dt)
    //{
    //    float anglea  = 0.0f;
    //    Vector3 axisa = Vector3.zero;
    //    qa.ToAngleAxis(out anglea, out axisa);
    //    anglea *= Mathf.Deg2Rad;

    //    float angleb  = 0.0f;
    //    Vector3 axisb = Vector3.zero;
    //    qb.ToAngleAxis(out angleb, out axisb);
    //    angleb *= Mathf.Deg2Rad;

    //    Vector3 vectora = (anglea * axisa);
    //    Vector3 vectorb = (angleb * axisb);

    //    //print("axisa " + axisa);

    //    //debugSphere.position = axisa * anglea;

    //    return (vectorb - vectora) / dt;
    //}

    //private Vector3 RVDelta2(Quaternion qa, Quaternion qb, float dt)
    //{
    //    Quaternion qdiff = qb * Quaternion.Inverse(qa);

    //    float angle;
    //    Vector3 axis;
    //    qdiff.ToAngleAxis(out angle, out axis);

    //    return (angle * axis) / dt;
    //}

    //private Vector3 RVDelta3(Quaternion qa, Quaternion qb, float dt)
    //{
    //    Quaternion qdiff = qb * Quaternion.Inverse(qa);

    //    Vector3 euler = qdiff.eulerAngles;

    //    return euler / dt;
    //}

    public static float NextGaussian()
    {
        float v1, v2, s;
        do {
            v1 = 2.0f * Random.Range(0f,1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f,1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);

        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);
    
        return v1 * s;
    }

    public static float NextGaussian(float mean, float standard_deviation)
    {
        return mean + NextGaussian() * standard_deviation;
    }

    private void construct_Xi(Quaternion q,
                                out Vector3 col1,
                                out Vector3 col2,
                                out Vector3 col3,
                                out Vector3 col4)
    {
        float q1 = q[0];
        float q2 = q[1];
        float q3 = q[2];
        float q4 = q[3];
        col1 = new Vector3( q4, -q3,  q2);
        col2 = new Vector3( q3,  q4, -q1);
        col3 = new Vector3(-q2,  q1,  q4);
        col4 = new Vector3(-q1, -q2, -q3);
    }

    private Vector3 AngularVelocity(Quaternion qa, Quaternion qb, float dt)
    {
        Vector4 qa_ = new Vector4(qa[0], qa[1], qa[2], qa[3]);
        Vector4 qb_ = new Vector4(qb[0], qb[1], qb[2], qb[3]);
        Vector4 qdiff = (qb_ - qa_) / dt;
        float q1 = qdiff[0];
        float q2 = qdiff[1];
        float q3 = qdiff[2];
        float q4 = qdiff[3];

        Vector3 col1, col2, col3, col4;
        construct_Xi(qa, out col1, out col2, out col3, out col4);

        Vector3 w = 2 * (q1 * col1 + q2 * col2 + q3 * col3 + q4 * col4);

        return w;
    }

    private Matrix4x4 ConstructDiffEqMatrix(Vector3 rvDelta, float dt)
    {
        Matrix4x4 A = Matrix4x4.identity;

        float w1, w2, w3;
        w1 = rvDelta[0];
        w2 = rvDelta[1];
        w3 = rvDelta[2];

        float a = dt/2;

        A.SetRow(0, new Vector4(   1.0f,   w3*a,  -w2*a,  w1*a));
        A.SetRow(1, new Vector4(  -w3*a,   1.0f,   w1*a,  w2*a));
        A.SetRow(2, new Vector4(   w2*a,  -w1*a,   1.0f,  w3*a));
        A.SetRow(3, new Vector4(  -w1*a,  -w2*a,  -w3*a, 1.0f));

        return A;
    }

    private Quaternion Eulerstep(Quaternion cur, Vector3 rvDelta, float dt)
    {
        Matrix4x4 A = ConstructDiffEqMatrix(rvDelta, dt);

        float q1, q2, q3, q4;
        q1 = cur[0];
        q2 = cur[1];
        q3 = cur[2];
        q4 = cur[3];

        Vector4 qnew =    q1 * A.GetColumn(0)
                        + q2 * A.GetColumn(1)
                        + q3 * A.GetColumn(2)
                        + q4 * A.GetColumn(3);
        
        qnew = qnew.normalized;

        return new Quaternion(qnew[0], qnew[1], qnew[2], qnew[3]);
    }

    private void printMatrix(Matrix4x4 m)
    {
        Debug.Log("Row 1:" + m.GetRow(0));
        Debug.Log("Row 2:" + m.GetRow(1));
        Debug.Log("Row 3:" + m.GetRow(2));
        Debug.Log("Row 4:" + m.GetRow(3));
    }

    //void Start()
    IEnumerator Start()
    {
        //Quaternion q = new Quaternion(1f, 2f, 3f, 4f);
        //print("Q: " + q);
        //printMatrix(Matrix4x4.Rotate(q));
        Debug.Log("Waiting 2 seconds before intialising");
        yield return new WaitForSeconds(2.0f);

        gameObject.transform.rotation = tocopy.transform.rotation;
        lastRotation = tocopy.transform.rotation;
        timeSinceLastRefresh = 0.0f;
        
    }

    void Update()
    {
        lastRotation = tocopy.transform.rotation;
    }

    void FixedUpdate()
    {
        timeSinceLastRefresh += Time.fixedDeltaTime;
        if(timeSinceLastRefresh > 1/refreshRate_Hz && refreshing) {
            timeSinceLastRefresh = 0.0f;
            gameObject.transform.rotation = tocopy.transform.rotation;
        }

        // rvDelta should behave like a Gyroscope measurement
        Vector3 angularv = AngularVelocity(
                            lastRotation,
                            tocopy.transform.rotation,
                            Time.fixedDeltaTime
                            );
        
        angularv += new Vector3(
            NextGaussian(bias, std),
            NextGaussian(bias, std),
            NextGaussian(bias, std)
        );
        
        //print("RVDelta " + rvDelta);

        gameObject.transform.rotation = Eulerstep(
                                            lastRotation,
                                            angularv,
                                            Time.fixedDeltaTime
                                            );

        lastRotation = gameObject.transform.rotation;
    }
}
