using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public GameObject cube; // キューブのゲームオブジェクト
    public float restDuration = 3f; // 安静状態の時間
    public float instructionDuration = 1f; // 指示の時間
    public float crossDuration = 1f; // 十字の合図の時間
    public float moveDuration = 4f; // キューブを動かす時間
    public float moveSpeed = 3f; // キューブの速さ

    private bool isTaskRunning = false;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = cube.transform.position;
    }

    private void Update()
    {
        // タスクオンオフ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cube.transform.position = initialPosition;
            isTaskRunning = !isTaskRunning;
        }

        // オブジェクト制御処理
        if ((Input.GetKeyDown(KeyCode.W)|| UDPReceiver.receivedInt == 2) && isTaskRunning)
        {
            Vector3 direction = new Vector3(0, 0, 1);
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            cube.transform.position += movement;
        }

        // オブジェクト制御処理
        else if ((Input.GetKeyDown(KeyCode.S) || UDPReceiver.receivedInt == 3) && isTaskRunning)
        {
            Vector3 direction = new Vector3(0, 0, -1);
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            cube.transform.position += movement;
        }

        
        else
        {
            Vector3 direction = new Vector3(0, 0, 0);
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            cube.transform.position += movement;
        }
    }
}