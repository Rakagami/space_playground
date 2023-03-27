using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SensorRead
{
    public SensorRead(double[] raw)
    {
        temp   = raw[0];
        accel  = new Vector3((float)raw[1], (float)raw[2], (float)raw[3]);
        mag    = new Vector3((float)raw[4], (float)raw[5], (float)raw[6]);
        gyro   = new Vector3((float)raw[7], (float)raw[8], (float)raw[9]);
        gyro   = (gyro / 2.0f*Mathf.PI) * 360;
    }
    
    public double temp;
    public Vector3 accel;
    public Vector3 mag;
    public Vector3 gyro;

    private string Vec3ToString(Vector3 vec)
    {
        return $"{vec[0]}, {vec[1]}, {vec[2]}";
    }

    public override string ToString()
    {
        return $"Temp: {temp}; Accel: ({Vec3ToString(accel)}); Mag: ({Vec3ToString(mag)}); Gyro: ({Vec3ToString(gyro)});";
    }
}


public class Rotator : MonoBehaviour
{
    bool serverStarted = false;

    IEnumerator TcpServer()
    {
        newconnection:

        TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 6969);
        listener.Start();
        //Console.WriteLine("Waiting for a connection.");
        Debug.Log("Waiting for a connection.");
        TcpClient client = listener.AcceptTcpClient();
        //Console.WriteLine("Client accepted.");
        Debug.Log("Client accepted.");
        NetworkStream stream = client.GetStream();
        StreamReader sr = new StreamReader(client.GetStream());
        StreamWriter sw = new StreamWriter(client.GetStream());

        while(true)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int recv = 80;
                stream.Read(buffer, 0, buffer.Length);
                //Console.WriteLine("Data Received, length: " + recv + " bytes");
                Debug.Log("Data Received, length: " + recv + " bytes");
                double[] values = new double[recv / 8];
                Buffer.BlockCopy(buffer, 0, values, 0, recv);
                SensorRead read = new SensorRead(values);
                //Console.WriteLine("Received Package: " + read);
                Debug.Log("Received Package: " + read);
                gameObject.transform.Rotate(read.gyro, Space.Self);
                sw.WriteLine("Received Package!");
                sw.Flush();
            } catch(Exception e)
            {
                //Console.WriteLine("Connection Stopped...");
                Debug.Log("Connection Stopped...");
                sw.WriteLine(e.ToString());
                goto newconnection;
            }
            yield return null;
        }
    }
        

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(!serverStarted){
            StartCoroutine(TcpServer());
            serverStarted = true;
        }
    }
}
