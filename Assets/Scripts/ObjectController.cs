using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public GameObject cube; // �L���[�u�̃Q�[���I�u�W�F�N�g
    public float moveSpeed; // �L���[�u�̑���

    public bool isTaskRunning = false;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = cube.transform.position;
    }

    private void Update()
    {
        // 位置のリセットと移動可能かを設定
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cube.transform.position = initialPosition;
            isTaskRunning = !isTaskRunning;
            UnityEngine.Debug.Log(isTaskRunning);
        }

        // 前進
        if (Input.GetKey(KeyCode.W) && isTaskRunning)
        {
            Vector3 direction = new Vector3(0, 0, 1);
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            cube.transform.position += movement;
        }

        // 後進
        else if (Input.GetKey(KeyCode.S)&& isTaskRunning)
        {
            Vector3 direction = new Vector3(0, 0, -1);
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            cube.transform.position += movement;
        }
    }
}