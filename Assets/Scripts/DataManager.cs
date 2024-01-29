using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;

using System;
using System.Text;

[Serializable]
public class Accelerator
{
    public Accelerator() 
    { 
        roll1 = 0;
        roll2 = 0;
    }

    public float roll1 { get; set; }
    public float roll2 { get; set; }
}

public class DataManager : MonoBehaviour
{
    public Accelerator accelerator = new Accelerator();

    private Socket sock;
    //
    public const int BufferSize = 64;// Size of receive buffer
    private byte[] buffer = new byte[BufferSize];// Receive buffer

    private void Start()
    {
        sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        sock.Connect(IPAddress.Parse("192.168.137.218"), 4210);

        Debug.Log("Started socket");
        Debug.Log("Send pin-code");
        sock.Send(Encoding.UTF8.GetBytes("150302"));

        // Begin receiving the data from the remote device.  
        sock.BeginReceive(this.buffer, 0, DataManager.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    }

    private void OnDestroy()
    {
        sock.Close();
    }

    float fps = 0;
    long startTick;
    private void ReceiveCallback(IAsyncResult ar)
    {
        int bytesRead = sock.EndReceive(ar);
        string receivedMessage = System.Text.Encoding.UTF8.GetString(this.buffer, 0, bytesRead);

        string[] rolls = receivedMessage.Split(',');
        accelerator.roll1 = float.Parse(rolls[0]);
        accelerator.roll2 = float.Parse(rolls[1]);

        //Debug.Log(receivedMessage + " - " + accelerator.roll1 + " - " + accelerator.roll2);
        Debug.Log("Data received: " + receivedMessage.ToString());

        if (fps < 0.00006)
        {
            startTick = DateTime.Now.Ticks;
        }
        //
        fps += 1.0f;
        TimeSpan elapse = new TimeSpan(DateTime.Now.Ticks - startTick);
        if (elapse.TotalSeconds > 10.0)
        {

            fps = fps / (float)elapse.TotalSeconds;
            //Debug.Log("Data received FPS: " + fps);
            fps = 0;
            startTick = DateTime.Now.Ticks;
        }

        // Continue receiving the data from the remote device.  
        sock.BeginReceive(this.buffer, 0, DataManager.BufferSize, 0, new AsyncCallback(ReceiveCallback), null);
    }
}
