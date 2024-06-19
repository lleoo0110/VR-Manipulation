using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public GameObject cube; // �L���[�u�̃Q�[���I�u�W�F�N�g
    public float restDuration = 3f; // ���Ï�Ԃ̎���
    public float instructionDuration = 1f; // �w���̎���
    public float crossDuration = 1f; // �\���̍��}�̎���
    public float moveDuration = 4f; // �L���[�u�𓮂�������
    public float moveSpeed = 3f; // �L���[�u�̑���

    private bool isTaskRunning = false;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = cube.transform.position;
    }

    private void Update()
    {
        // �^�X�N�I���I�t
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cube.transform.position = initialPosition;
            isTaskRunning = !isTaskRunning;
        }

        // �I�u�W�F�N�g���䏈��
        if ((Input.GetKeyDown(KeyCode.W)|| UDPReceiver.receivedInt == 2) && isTaskRunning)
        {
            Vector3 direction = new Vector3(0, 0, 1);
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            cube.transform.position += movement;
        }

        // �I�u�W�F�N�g���䏈��
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