using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPSender : MonoBehaviour {
    public static UDPSender instance; // �ʃI�u�W�F�N�g����Q�Ƃ���p
    private UdpClient udpClient;
    private string ipAddress = "127.0.0.1";
    private int port = 33333;

    void Start() {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        udpClient = new UdpClient();
    }

    void Update() {
        /*
        if(Keyboard.current.sKey.wasPressedThisFrame)
        {
            SendData("Start");
            Debug.Log("Start");
        }
        else if(Keyboard.current.eKey.wasPressedThisFrame)
        {
            SendData("End");
            Debug.Log("End");
        }
        */
    }

    public void SendData(string data) 
    {
        byte[] sendData = Encoding.UTF8.GetBytes(data);
        udpClient.Send(sendData, sendData.Length, ipAddress, port);
    }
}
