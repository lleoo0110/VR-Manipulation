using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UDPReceiver : MonoBehaviour
{
    public static UDPReceiver instance {  get; private set; }
    UdpClient udpClient;
    public static int receivedInt;
    int listenPort = 12354;
    Boolean isTaskRunning = false;
    List<string> inputRow = new List<string>();

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        udpClient = new UdpClient(listenPort);
        BeginReceive();
    }

    // 別スクリプトを参照できないため、ExperimentTask2からこっちの値を更新する
    public void taskChange(Boolean isTaskRunning)
    {
        this.isTaskRunning= isTaskRunning;
    }

    // タスクごとの入力データを返してリストを初期化
    public List<string> getInputs()
    {
        List<string> inputs = inputRow;
        inputRow = new List<string>();
        return inputs;
    }

    void BeginReceive()
    {
        udpClient.BeginReceive(new AsyncCallback(OnReceived), null);
    }

    void OnReceived(IAsyncResult result)
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);
        byte[] receivedBytes = udpClient.EndReceive(result, ref remoteEP);
        // バイト配列を整数に変換
        if (receivedBytes.Length == 4)
        {
            receivedInt = BitConverter.ToInt32(receivedBytes, 0);
            Debug.Log("Received int: " + receivedInt);

            // 直接Unity内のオブジェクトに接続するのは厳しい
            if (isTaskRunning)
            {
                UnityEngine.Debug.Log("記録中...");
                inputRow.Add(receivedInt.ToString());
            }
        }
        

        // 再度受信を開始
        BeginReceive();
    }

    void OnApplicationQuit()
    {
        this.udpClient.Close();
    }

}