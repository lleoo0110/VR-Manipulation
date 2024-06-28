// 実験前の練習用

using UnityEngine;

public class Practice : MonoBehaviour
{
    public bool isTaskRunning = false;
    
    public float moveSpeed; // 速度
    private Vector3 initialPosition;
    public float objLowLim; // オブジェクトの可動範囲
    public float objHighLim;

    private void Start()
    {
        initialPosition = this.gameObject.transform.position;
    }

    private void Update()
    {
        if (objLowLim <= this.gameObject.transform.position.z && this.gameObject.transform.position.z <= objHighLim) {
            int input = UDPReceiver.receivedInt;
            // 位置のリセットと移動可能かを設定
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.gameObject.transform.position = initialPosition;
                isTaskRunning = !isTaskRunning;
                UnityEngine.Debug.Log(isTaskRunning);
            }
            // 前進
            if (input==2 && isTaskRunning)
            {
                Vector3 direction = new Vector3(0, 0, 1);
                Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
                this.gameObject.transform.position += movement;
            }

            // 後進
            else if (input==3 && isTaskRunning)
            {
                Vector3 direction = new Vector3(0, 0, -1);
                Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
                this.gameObject.transform.position += movement;
            }
        }
        
    }
}