using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using System.Text;

public class TrainingManager : MonoBehaviour
{
    public GameObject cube; // �L���[�u�̃Q�[���I�u�W�F�N�g
    public float restDuration; // 安静期間
    public float instructionDuration; // 指示期間
    public float crossDuration; // 合図期間
    public float moveDuration; // イメージ想起期間
    public float moveSpeed; // オブジェクトの移動速度

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
            // ここを繰り返す
            yield return StartCoroutine(Neutral());
            yield return StartCoroutine(PushCube());
            yield return StartCoroutine(PullCube());
        }
    }

    private IEnumerator Neutral()
    {
        UnityEngine.Debug.Log("Neutral started.");

        stopwatch = Stopwatch.StartNew();
        Vector3 startPosition = new Vector3(0, 1, 2.5f); // �X�^�[�g�ʒu���w��
        Vector3 direction = new Vector3(0, 0, 0); // �ړ��������w��
        cube.transform.position = startPosition;

        // 安静期間
        // ウィンドウ出す（白い画面）
        // Canvas ON
        // ここにプログラム
        yield return new WaitForSeconds(restDuration);

        // 指示期間
        // ウィンドウにイメージの指示を表示
        // ここにプログラム
        UnityEngine.Debug.Log("Stay still.");
        yield return new WaitForSeconds(instructionDuration);

        // ウィンドウに十字の合図を表示
        // ここにプログラム
        UnityEngine.Debug.Log("Cross sign.");
        yield return new WaitForSeconds(crossDuration);

        // イメージ想起期間
        // ここにプログラム Canvas OFF
        UnityEngine.Debug.Log("Staying still.");
        StartCoroutine(MoveCube(startPosition, direction, true));
        yield return new WaitForSeconds(moveDuration);

        // �\���̍��}
        // ここにプログラム
        UnityEngine.Debug.Log("Cross sign.");
        cube.transform.position = startPosition;
        yield return new WaitForSeconds(crossDuration);

        // 
        // ここにプログラム Canvas OFF
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
        Vector3 startPosition = new Vector3(0, 1, -5f); // �X�^�[�g�ʒu���w��
        Vector3 direction = new Vector3(0, 0, 1); // �ړ��������w��
        cube.transform.position = startPosition;

        // ���Ï��
        yield return new WaitForSeconds(restDuration);

        // �C���[�W�w��
        UnityEngine.Debug.Log("Imagine the cube backwards.");
        yield return new WaitForSeconds(instructionDuration);

        // �\���̍��}
        UnityEngine.Debug.Log("Cross sign.");
        yield return new WaitForSeconds(crossDuration);

        // �L���[�u�����ɓ�����(������)
        UnityEngine.Debug.Log("Moving the cube backwards with Sound.");
        StartCoroutine(MoveCube(startPosition, direction, false));
        yield return new WaitForSeconds(moveDuration);

        // �\���̍��}
        UnityEngine.Debug.Log("Cross sign.");
        cube.transform.position = startPosition;
        yield return new WaitForSeconds(crossDuration);

        // �L���[�u�����ɓ�����(���Ȃ�)
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
        Vector3 startPosition = new Vector3(0, 1, 10f); // �X�^�[�g�ʒu���w��
        Vector3 direction = new Vector3(0, 0, -1); // �ړ��������w��
        cube.transform.position = startPosition;

        // ���Ï��
        yield return new WaitForSeconds(restDuration);

        // �C���[�W�w��
        UnityEngine.Debug.Log("Imagine moving the cube forwards.");
        yield return new WaitForSeconds(instructionDuration);

        // �\���̍��}
        UnityEngine.Debug.Log("Cross sign.");
        yield return new WaitForSeconds(crossDuration);

        // �L���[�u�����ɓ�����(������)
        UnityEngine.Debug.Log("Moving the cube backwards with Sound.");
        StartCoroutine(MoveCube(startPosition, direction, false));
        yield return new WaitForSeconds(moveDuration);

        // �\���̍��}
        UnityEngine.Debug.Log("Cross sign.");
        cube.transform.position = startPosition;
        yield return new WaitForSeconds(crossDuration);

        // �L���[�u�����ɓ�����(���Ȃ�)
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

        // �I�u�W�F�N�g�̉��̃~���[�g��ݒ�
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
