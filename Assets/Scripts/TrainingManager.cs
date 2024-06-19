using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using System.Text;

public class TrainingManager : MonoBehaviour
{
    public GameObject cube; // �L���[�u�̃Q�[���I�u�W�F�N�g
    public float restDuration; // ���Ï�Ԃ̎���
    public float instructionDuration; // �w���̎���
    public float crossDuration; // �\���̍��}�̎���
    public float moveDuration; // �L���[�u�𓮂�������
    public float moveSpeed; // �L���[�u�̑���

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

        // ���Ï��
        yield return new WaitForSeconds(restDuration);

        // �Î~�̎w��
        UnityEngine.Debug.Log("Stay still.");
        yield return new WaitForSeconds(instructionDuration);

        // �\���̍��}
        UnityEngine.Debug.Log("Cross sign.");
        yield return new WaitForSeconds(crossDuration);

        // �Î~
        UnityEngine.Debug.Log("Staying still.");
        StartCoroutine(MoveCube(startPosition, direction, true));
        yield return new WaitForSeconds(moveDuration);

        // �\���̍��}
        UnityEngine.Debug.Log("Cross sign.");
        cube.transform.position = startPosition;
        yield return new WaitForSeconds(crossDuration);

        // �Î~
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
