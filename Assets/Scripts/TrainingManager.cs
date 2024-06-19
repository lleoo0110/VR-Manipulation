using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using System.Text;

public class TrainingManager : MonoBehaviour
{
    public GameObject cube; // キューブのゲームオブジェクト
    public float restDuration = 4f; // 安静状態の時間
    public float instructionDuration = 1f; // 指示の時間
    public float crossDuration = 1f; // 十字の合図の時間
    public float moveDuration = 4f; // キューブを動かす時間
    public float moveSpeed = 3f; // キューブの速さ

    private bool isTaskRunning = false;
    private Stopwatch stopwatch;

    private UdpClient udpClient;
    private string ipAddress = "127.0.0.1";
    private int port = 12354;

    private void Start()
    {
        udpClient = new UdpClient();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTaskRunning)
        {
            isTaskRunning = true;
            StartCoroutine(RunTraining());
            SendData("Start");
        }
    }

    private IEnumerator RunTraining()
    {
        // isTaskRunning = true;

        while (true)
        {
            yield return StartCoroutine(Neutral());
            yield return StartCoroutine(PushCube());
            yield return StartCoroutine(Neutral());
            yield return StartCoroutine(PullCube());
        }
    }

    private IEnumerator Neutral()
    {
        UnityEngine.Debug.Log("Neutral started.");

        stopwatch = Stopwatch.StartNew();
        Vector3 startPosition = new Vector3(0, 1, 1.5f); // スタート位置を指定
        Vector3 direction = new Vector3(0, 0, 0); // 移動方向を指定
        cube.transform.position = startPosition;

        // 安静状態
        yield return new WaitForSeconds(restDuration);

        // 静止の指示
        UnityEngine.Debug.Log("Stay still.");
        yield return new WaitForSeconds(instructionDuration);

        // 十字の合図
        UnityEngine.Debug.Log("Cross sign.");
        yield return new WaitForSeconds(crossDuration);

        // 静止
        UnityEngine.Debug.Log("Staying still.");
        StartCoroutine(MoveCube(startPosition, direction, true));
        yield return new WaitForSeconds(moveDuration);

        // 十字の合図
        UnityEngine.Debug.Log("Cross sign.");
        cube.transform.position = startPosition;
        yield return new WaitForSeconds(crossDuration);

        // 静止
        UnityEngine.Debug.Log("Staying still.");
        StartCoroutine(MoveCube(startPosition, direction, true));
        yield return new WaitForSeconds(moveDuration);

        stopwatch.Stop();
        UnityEngine.Debug.Log("Neutral completed. Elapsed time: " + stopwatch.Elapsed.TotalSeconds + " seconds");
    }

    private IEnumerator PushCube()
    {
        UnityEngine.Debug.Log(" startedPushCube.");

        stopwatch = Stopwatch.StartNew();
        Vector3 startPosition = new Vector3(0, 1, -4f); // スタート位置を指定
        Vector3 direction = new Vector3(0, 0, 1); // 移動方向を指定
        cube.transform.position = startPosition;

        // 安静状態
        yield return new WaitForSeconds(restDuration);

        // イメージ指示
        UnityEngine.Debug.Log("Imagine the cube backwards.");
        yield return new WaitForSeconds(instructionDuration);

        // 十字の合図
        UnityEngine.Debug.Log("Cross sign.");
        yield return new WaitForSeconds(crossDuration);

        // キューブを奥に動かす(音あり)
        UnityEngine.Debug.Log("Moving the cube backwards with Sound.");
        StartCoroutine(MoveCube(startPosition, direction, false));
        yield return new WaitForSeconds(moveDuration);

        // 十字の合図
        UnityEngine.Debug.Log("Cross sign.");
        cube.transform.position = startPosition;
        yield return new WaitForSeconds(crossDuration);

        // キューブを奥に動かす(音なし)
        UnityEngine.Debug.Log("Moving the cube backwards without Sound.");
        StartCoroutine(MoveCube(startPosition, direction, true));
        yield return new WaitForSeconds(moveDuration);

        stopwatch.Stop();
        UnityEngine.Debug.Log("PushCube completed. Elapsed time: " + stopwatch.Elapsed.TotalSeconds + " seconds");
    }

    private IEnumerator PullCube()
    {
        UnityEngine.Debug.Log("PullCube started.");

        stopwatch = Stopwatch.StartNew();
        Vector3 startPosition = new Vector3(0, 1, 7f); // スタート位置を指定
        Vector3 direction = new Vector3(0, 0, -1); // 移動方向を指定
        cube.transform.position = startPosition;

        // 安静状態
        yield return new WaitForSeconds(restDuration);

        // イメージ指示
        UnityEngine.Debug.Log("Imagine moving the cube forwards.");
        yield return new WaitForSeconds(instructionDuration);

        // キューブを奥に動かす(音あり)
        UnityEngine.Debug.Log("Moving the cube backwards with Sound.");
        StartCoroutine(MoveCube(startPosition, direction, false));
        yield return new WaitForSeconds(moveDuration);

        // 十字の合図
        UnityEngine.Debug.Log("Cross sign.");
        cube.transform.position = startPosition;
        yield return new WaitForSeconds(crossDuration);

        // キューブを奥に動かす(音なし)
        UnityEngine.Debug.Log("Moving the cube backwards without Sound.");
        StartCoroutine(MoveCube(startPosition, direction, true));
        yield return new WaitForSeconds(moveDuration);

        stopwatch.Stop();
        UnityEngine.Debug.Log("PullCube. Elapsed time: " + stopwatch.Elapsed.TotalSeconds + " seconds");
    }

    private IEnumerator MoveCube(Vector3 startPosition, Vector3 direction, bool isMuted)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // オブジェクトの音のミュートを設定
        AudioSource audioSource = cube.GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.mute = isMuted;
        }

        cube.transform.position = startPosition;

        while (stopwatch.Elapsed.TotalSeconds < moveDuration)
        {
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            cube.transform.position += movement;
            yield return null;
        }

        audioSource.mute = true;
        stopwatch.Stop();
    }

    public void SendData(string data)
    {
        byte[] sendData = Encoding.UTF8.GetBytes(data);
        udpClient.Send(sendData, sendData.Length, ipAddress, port);
    }


}
