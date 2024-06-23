using UnityEngine;
using System.Diagnostics;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class ExperimentTask2 : MonoBehaviour
{
    public GameObject objectPrefab; // オブジェクトのプレハブ
    private Vector3 initialPosition;
    public GameObject window; // ウィンドウ
    public float winFrontPos; // ウィンドウの手前の座標
    public float winBackPos; // ウィンドウの奥の座標
    public Color initialColor; // 初期状態のウィンドウの色
    public Color activeColor; // アクティブ状態(ボールが通り過ぎた後)の色
    private Color noColor = new Color(0, 0, 0, 0);
    private int task = 1; //今どの状態かを示す　1: 休憩状態, 2: 手前から奥のタスク, 3: 奥から手前のタスク

    private float goal; // この位置を過ぎて静止できればタスククリア
    private Boolean goalOver = false; // ゴールを過ぎているか

    private int times = 1; // 今何回目のタスクか

    public Vector3[] objectPositions; // オブジェクトの生成位置の配列
    public Vector3[] windowPositions; // ウィンドウの生成位置の配列
    public float delayTime = 3f; // 静止時間
    public float stayDuration = 0.5f; // 静止判定の時間
    
    private GameObject currentObject; // 現在のオブジェクト
    private GameObject currentWindow; // 現在のウィンドウ
    private bool isTaskStarted = false; // タスク開始フラグ
    private bool isObjectMovable = false; // オブジェクト移動可能フラグ
    private Vector3 previousPosition; // 前回のオブジェクト位置
    private Stopwatch stayStopwatch = new Stopwatch(); // 静止判定用ストップウォッチ
    private Stopwatch delayStopwatch = new Stopwatch(); // 遅延時間用ストップウォッチ
    public Stopwatch taskStopwatch=new Stopwatch(); // 1タスクあたりのタイムスコアを計測するストップウォッチ

    // CSV書き出し用
    private List<List<string>> data = new List<List<string>>();

    private void Start()
    {
        data.Add(new List<string> { "Times", "Score(sec)"});
        initialPosition = this.gameObject.transform.position;
        window.GetComponent<Renderer>().material.color = noColor;
        StartDelayTimer();
        //SetupExperiment();
    }

    private void Update()
    {
        if (task == 2)
        {
            UpdateWindow2();
        }else if (task==3)
        {
            UpdateWindow3();
        }
        if (goalOver) {
            CheckStill();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && task == 1)//タスクないとき
        {
            task = UnityEngine.Random.Range(0, 2) + 2; // 2以上3未満の整数
            StartTask(task);
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) EndTask();
        //if (isTaskStarted)
        //{
        //    UpdateTaskState();
        //    MoveObjectWithMouseInput();
        //}
        //else
        //{
        //    CheckDelayTimerToStartTask();
        //}
    }

    private void SetupExperiment()
    {
        //SpawnObjectAndWindow();
        //SetupObjectRigidbody();
        //SetupWindowCollider();
        //SetInitialWindowColor();
    }

    //タスクの開始
    private void StartTask(int task)
    {
        float z = 0;
        //手前から奥のタスクの時
        if (task == 2)
        {
            z = winBackPos;
            goal = winFrontPos;
        }
        else if (task == 3)
        {
            z = winFrontPos;
            goal = winBackPos;
        }
        window.transform.position = new Vector3(window.transform.position.x, window.transform.position.y, z);
        window.GetComponent<Renderer>().material.color = initialColor; //ウィンドウの色
        this.gameObject.GetComponent<ObjectController>().isTaskRunning = true ;
        taskStopwatch.Restart();
        UnityEngine.Debug.Log("タスク開始");
    }

    //手前から奥の時のウィンドウの更新
    private void UpdateWindow2()
    {
        float z=this.gameObject.transform.position.z;
        if (z >= winBackPos&&!goalOver)//ゴールバーを超えていて、ゴール状態じゃないとき
        {
            window.GetComponent<Renderer>().material.color = activeColor;
            goalOver = true;
            UnityEngine.Debug.Log("ゴールゾーンに入りました");
        }
        else if(z<=winBackPos&&goalOver)
        {
            window.GetComponent<Renderer>().material.color = initialColor;
            goalOver = false;
            UnityEngine.Debug.Log("ゴールゾーンから外れました");
        }
    }

    // 奥から手前のときのウィンドウの更新
    private void UpdateWindow3()
    {
        float z = this.gameObject.transform.position.z;
        if (z <= winFrontPos&&!goalOver)//ゴールバーを超えていて、ゴール状態じゃないとき
        {
            window.GetComponent<Renderer>().material.color = activeColor;
            goalOver = true;
            UnityEngine.Debug.Log("ゴールゾーンに入りました");
        }
        else if(z>=winFrontPos&&goalOver)
        {
            window.GetComponent<Renderer>().material.color = initialColor;
            goalOver = false;
            UnityEngine.Debug.Log("ゴールゾーンから外れました");
        }
    }

    // 静止したかチェック
    private void CheckStill()
    {
        if(UDPReceiver.receivedInt == 1|Input.GetKeyDown(KeyCode.Return))
        {
            taskStopwatch.Stop();
            double score=taskStopwatch.Elapsed.TotalSeconds; // かかった時間
            // 今回のタスクのタイムスコアをdataに追加
            int newId = data.Count;
            data.Add(new List<string> { times.ToString(), score.ToString() });
            UnityEngine.Debug.Log("タスククリア(1を検出): "+times+"回目: "+score+"s");
            this.gameObject.transform.position = initialPosition;
            this.gameObject.GetComponent<ObjectController>().isTaskRunning = false;
            window.GetComponent<Renderer>().material.color = noColor;//無色にする
            goalOver = false;
            times++; // 次のタスクへ移行
            task = 1;
        }
    }

    // 全タスクの終了
    private void EndTask()
    {
        // 書き出し先のファイルパス（プロジェクトフォルダのルートに書き出す）
        string filePath = Path.Combine(Application.dataPath, "OutputData.csv");

        // CSVに書き出し
        WriteToCSV(filePath, data);
    }

    // データをCSVに書き出すメソッド
    public void WriteToCSV(string filePath, List<List<string>> data)
    {
        // StreamWriterを使ってファイルに書き込む
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (List<string> row in data)
            {
                // 各行のデータをカンマで区切って連結
                string line = string.Join(",", row);
                // 書き込む
                writer.WriteLine(line);
            }
        }

        UnityEngine.Debug.Log("CSVファイルに書き出しました: " + filePath);
    }

    //private void SpawnObjectAndWindow()
    //{
    //    int objectIndex = Random.Range(0, objectPositions.Length);
    //    int windowIndex = Random.Range(0, windowPositions.Length);
    //    currentObject = Instantiate(objectPrefab, objectPositions[objectIndex], Quaternion.identity);

    //    //currentWindow = Instantiate(windowPrefab, windowPositions[windowIndex], Quaternion.identity);
    //}

    private void SetupObjectRigidbody()
    {
        Rigidbody objectRigidbody = currentObject.AddComponent<Rigidbody>();
        objectRigidbody.useGravity = false;
    }

    private void SetupWindowCollider()
    {
        Collider windowCollider = currentWindow.AddComponent<BoxCollider>();
        windowCollider.isTrigger = true;
    }

    private void SetInitialWindowColor()
    {
        currentWindow.GetComponent<Renderer>().material.color = Color.red;
    }

    private void StartDelayTimer()
    {
        delayStopwatch.Start();
    }

    private void UpdateTaskState()
    {
        if (!isObjectMovable && IsDelayTimeElapsed())
        {
            isObjectMovable = true;
            SetWindowColor(Color.red);
        }
    }

    private bool IsDelayTimeElapsed()
    {
        return delayStopwatch.Elapsed.TotalSeconds >= delayTime;
    }

    private void SetWindowColor(Color color)
    {
        currentWindow.GetComponent<Renderer>().material.color = color;
    }

    private void MoveObjectWithMouseInput()
    {
        if (isObjectMovable && Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            currentObject.GetComponent<Rigidbody>().MovePosition(mousePosition);
        }
    }

    private void CheckDelayTimerToStartTask()
    {
        if (IsDelayTimeElapsed())
        {
            isTaskStarted = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsObjectEnteringWindow(other))
        {
            SetWindowColor(Color.green);
            previousPosition = currentObject.transform.position;
            stayStopwatch.Restart();
        }
    }

    private bool IsObjectEnteringWindow(Collider other)
    {
        return other.gameObject == currentObject;
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsObjectStayingInWindow(other))
        {
            CheckObjectStationaryState();
        }
    }

    private bool IsObjectStayingInWindow(Collider other)
    {
        return other.gameObject == currentObject;
    }

    private void CheckObjectStationaryState()
    {
        if (IsObjectStationary())
        {
            if (IsStayDurationElapsed())
            {
                UnityEngine.Debug.Log("Trial Clear!");
                // 次の試行に移行するための処理を追加
            }
        }
        else
        {
            UpdatePreviousPosition();
            stayStopwatch.Restart();
        }
    }

    private bool IsObjectStationary()
    {
        return currentObject.transform.position == previousPosition;
    }

    private bool IsStayDurationElapsed()
    {
        return stayStopwatch.Elapsed.TotalSeconds >= stayDuration;
    }

    private void UpdatePreviousPosition()
    {
        previousPosition = currentObject.transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsObjectExitingWindow(other))
        {
            SetWindowColor(Color.red);
            stayStopwatch.Stop();
        }
    }

    private bool IsObjectExitingWindow(Collider other)
    {
        return other.gameObject == currentObject;
    }
}