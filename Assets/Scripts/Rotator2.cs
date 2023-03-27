using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator2 : MonoBehaviour
{
    [Header("Interpolation Points")]
    public Vector3 euler1;
    public Vector3 euler2;

    [Header("Other")]
    public float total = 100;
    public float speed = 2.0f;
    public float cur = 0;

    [System.Serializable]
    public enum InterpolMode
    {
        Euler,
        Lerp,
        Slerp,
        QLerp,
        QSlerp,
        QuaternionNorm,
    }
    public InterpolMode im = InterpolMode.Euler;


    private Quaternion interpolateEuler(Vector3 euler1, Vector3 euler2, float alpha)
    {
        Vector3 cur_euler = (1.0f-alpha) * euler1 + (alpha) * euler2;
        return Quaternion.Euler(cur_euler.x, cur_euler.y, cur_euler.z);
    }

    private Quaternion interpolateLerp(Vector3 euler1, Vector3 euler2, float alpha)
    {
        Vector3 cur_euler = Vector3.Lerp(euler1, euler2, alpha);
        return Quaternion.Euler(cur_euler.x, cur_euler.y, cur_euler.z);
    }

    private Quaternion interpolateSlerp(Vector3 euler1, Vector3 euler2, float alpha)
    {
        Vector3 cur_euler = Vector3.Slerp(euler1, euler2, alpha);
        return Quaternion.Euler(cur_euler.x, cur_euler.y, cur_euler.z);
    }

    private Quaternion interpolateQLerp(Vector3 euler1, Vector3 euler2, float alpha)
    {
        Quaternion cur_quat = Quaternion.Lerp(Quaternion.Euler(euler1), Quaternion.Euler(euler2), alpha);
        return cur_quat;
    }

    private Quaternion interpolateQSlerp(Vector3 euler1, Vector3 euler2, float alpha)
    {
        Quaternion cur_quat = Quaternion.Slerp(Quaternion.Euler(euler1), Quaternion.Euler(euler2), alpha);
        return cur_quat;
    }
    
    private Quaternion interpolateQuatNorm(Vector3 euler1, Vector3 euler2, float alpha)
    {
        Quaternion q1 = Quaternion.Euler(euler1);
        Quaternion q2 = Quaternion.Euler(euler2);
        Vector4 q1_ = new Vector4(q1.x, q1.y, q1.z, q1.w);
        Vector4 q2_ = new Vector4(q2.x, q2.y, q2.z, q2.w);
        Vector4 cur_q_ = (1.0f-alpha) * q1_ + (alpha) * q2_;
        return new Quaternion(cur_q_.x, cur_q_.y, cur_q_.z, cur_q_.w);
    }

    //private Quaternion interpolateEuler(euler1, euler2, alpha)
    //{

    //}

    //private Quaternion interpolateEuler(euler1, euler2, alpha)
    //{

    //}

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        cur = cur + moveHorizontal * speed;
        cur = (cur < 0) ? 0 : ((cur > total ? total:cur));
        float alpha = cur/total;

        Quaternion newQuat;
        switch(im) {
            case InterpolMode.Euler:
                newQuat = interpolateEuler(euler1, euler2, alpha);
                break;
            case InterpolMode.Lerp:
                newQuat = interpolateLerp(euler1, euler2, alpha);
                break;
            case InterpolMode.Slerp:
                newQuat = interpolateSlerp(euler1, euler2, alpha);
                break;
            case InterpolMode.QLerp:
                newQuat = interpolateQLerp(euler1, euler2, alpha);
                break;
            case InterpolMode.QSlerp:
                newQuat = interpolateQSlerp(euler1, euler2, alpha);
                break;
            case InterpolMode.QuaternionNorm:
                newQuat = interpolateQuatNorm(euler1, euler2, alpha);
                break;
            default:
                newQuat = Quaternion.Euler(0, 0, 0);
                break;
        }
        transform.rotation = newQuat;
    }
}
