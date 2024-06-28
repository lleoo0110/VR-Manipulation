using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public static ObjectController instance {  get; private set; }
    public float moveSpeed; // �L���[�u�̑���

    public bool isTaskRunning = false;
    private Vector3 initialPosition;

    private void Start()
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
        initialPosition = this.gameObject.transform.position;
    }

    private void Update()
    {
        // 位置のリセットと移動可能かを設定
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.gameObject.transform.position = initialPosition;
            isTaskRunning = !isTaskRunning;
            UnityEngine.Debug.Log(isTaskRunning);
        }

        // 前進
        if (Input.GetKey(KeyCode.W) && isTaskRunning)
        {
            Vector3 direction = new Vector3(0, 0, 1);
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            this.gameObject.transform.position += movement;
        }

        // 後進
        else if (Input.GetKey(KeyCode.S)&& isTaskRunning)
        {
            Vector3 direction = new Vector3(0, 0, -1);
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            this.gameObject.transform.position += movement;
        }
    }
}