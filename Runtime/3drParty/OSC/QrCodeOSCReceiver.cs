using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OSC.NET;
using UnityEngine.Events;

public class QrCodeOSCReceiver : UCOSCReceiver {

    private const string OSCqrcodeAddress = "/qrcode";
    [SerializeField]
    private string qrcodeaddress;
    private int TimeStep;
    private int tempTimeStep;
    [System.Serializable]
    public class QrcodeAddressHandler : UnityEvent<string> { };
    public QrcodeAddressHandler QrcodeAddressEvent = new QrcodeAddressHandler();
    private List<string> qrcodes = new List<string>();

    new void Awake()
    {
        base.Awake();
        PacketReceivedEvent += PacketToQrcode;
    }

    private new void Update()
    {
        base.Update();
        if(TimeStep != tempTimeStep)
        {
            TimeStep = tempTimeStep;
            if(qrcodes.Count != 0)
            {
                lock(qrcodes)
                {
                    qrcodeaddress = qrcodes[0];
                    print(qrcodeaddress + ";");
                    qrcodes.RemoveAt(0);
                    QrcodeAddressEvent.Invoke(qrcodeaddress);
                }
            }
        }
    }

    new void OnDestroy()
    {
        base.OnDestroy();
    }

    public void PacketToQrcode(OSCPacket packet)
    {
        ArrayList arraylist;
        if (!packet.IsBundle())
        {
            OSCMessage msg = (OSCMessage)packet;
            arraylist = msg.Values;
        }
        else
        {
            OSCBundle bundle = (OSCBundle)packet;
            arraylist = bundle.Values;
        }
        Debug.Log(packet.Address);
        if (packet.Address == OSCqrcodeAddress)
        {
            if (tempTimeStep != (int)arraylist[0])
            {
                tempTimeStep = (int)arraylist[0];
                lock (qrcodes)
                {
                    qrcodes.Add((string)arraylist[1]);
                }
            }
        }
    }
}
