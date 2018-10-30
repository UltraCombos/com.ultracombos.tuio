using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OSC.NET;
using System.Threading;
using UnityEngine.Events;
public class UCOSCReceiver : MonoBehaviour {

    protected Thread thread;
    protected OSCReceiver oscreceiver;
    public int OSCReceivePort;
    protected bool isOSCReceiving = false;
    [System.Serializable]
    public class OSCPacketHandler : UnityEvent<OSCPacket> { };
    public OSCPacketHandler OSCPacketReceiveEvent = new OSCPacketHandler();
    public delegate void PacketReceivedEventHandler(OSCPacket packet);
    public event PacketReceivedEventHandler PacketReceivedEvent;


    private Queue<OSCPacket> OscPacketQueue = new Queue<OSCPacket>();

    protected void Awake()
    {
        oscreceiver = new OSCReceiver(OSCReceivePort);
        thread = new Thread(Receive);
        thread.Start();
    }

    protected void Update()
    {
        if(OscPacketQueue.Count !=0)
        {
            SendEventQueue();
        }
    }

    protected void SendEventQueue()
    {
        OSCPacket packet = null;
        while (true)
        {
            lock (OscPacketQueue)
            {
                if (OscPacketQueue.Count > 0)
                    packet = OscPacketQueue.Dequeue();
                else
                    return;
            }

            if (PacketReceivedEvent != null)
                PacketReceivedEvent.Invoke(packet);
            if (OSCPacketReceiveEvent != null)
                OSCPacketReceiveEvent.Invoke(packet);
        }
    }

    protected void Receive()
    {
        isOSCReceiving = true;
        while (isOSCReceiving)
        {
            OSCPacket packet = oscreceiver.Receive();
            if (packet != null)
            {
                lock(OscPacketQueue)
                {
                    OscPacketQueue.Enqueue(packet);
                }
            }
            Thread.Sleep(1);
        }
    }

    protected void OnDestroy()
    {
        oscreceiver.Close();
        isOSCReceiving = false;
        thread.Join();
    }
}
