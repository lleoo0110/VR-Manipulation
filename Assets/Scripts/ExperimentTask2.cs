using UnityEngine;
using System.Diagnostics;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;
//using UnityEngine.Windows;

public class ExperimentTask2 : MonoBehaviour
{
    private Vector3 initialPosition;//ボールの初期位置
    public float objFrontPos; // ボールの初期位置(手前のとき)
    public float objBackPos; // ボールの初期位置(奥のとき)
    private float moveSpeed; // ボールの移動速度(ObjectControllerから取得)

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
    public int taskNum; // 全体のタスク数
    private Boolean taskComplete = false;


    public float intervalTime = 1f; // タスクの開始時のインターバル
    public float stayDuration = 0.5f; // 静止判定の時間
    
    private Vector3 previousPosition; // 前回のオブジェクト位置
    private Stopwatch stayStopwatch = new Stopwatch(); // 静止判定用ストップウォッチ
    private Stopwatch delayStopwatch = new Stopwatch(); // 遅延時間用ストップウォッチ
    public Stopwatch taskStopwatch=new Stopwatch(); // 1タスクあたりのタイムスコアを計測するストップウォッチ
    private DateTime TodayNow;//現在時間(CSVファイル名に使う)
    public string user_name;//ファイル名に表示するユーザー名
    private string filename;

    // タイムスコアCSV書き出し用
    private List<List<string>> scoreData= new List<List<string>>();
    // 脳波による入力書き出し用
    private List<List<string>> inputData= new List<List<string>>();
    private List<string> inputRow = new List<string>();

    private void Start()
    {
        //時間を取得(ファイル名に使用)
        TodayNow = DateTime.Now;
        filename = TodayNow.Year.ToString() + "_" + TodayNow.Month.ToString() + "_" + TodayNow.Day.ToString() + "_" + DateTime.Now.ToLongTimeString() + ".csv";
        filename = filename.Replace(":", "_");//":"はファイル名に使えない
        scoreData.Add(new List<string> { "Times", "Score(sec)"});
        inputData.Add(new List<string> { "EEG Input" });
        // ObjectControllerからmoveSpeedを取得
        moveSpeed = this.gameObject.GetComponent<ObjectController>().moveSpeed;
        initialPosition = this.gameObject.transform.position;
        window.GetComponent<Renderer>().material.color = noColor;
        StartDelayTimer();
    }

    private void Update()
    {
        if (task == 2 || task == 3) UpdateBall();
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
        //タスクないときエンターを押したら次のタスクをスタート
        else if (UnityEngine.Input.GetKeyDown(KeyCode.Return) && task == 1)
        {
            task = UnityEngine.Random.Range(0, 2) + 2; // 2以上3未満の整数
            // コルーチンを開始
            StartCoroutine(DelayedStartTask(intervalTime,task));
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.Space) && (task == 2 || task == 3)) // スペースキーを押すとタイマーを再スタート
        {
            UnityEngine.Debug.Log("位置リセット+タイマーリスタート");
            taskStopwatch.Restart();
        }

        if (times > taskNum && !taskComplete)
        {
            taskComplete = true;
        }else if (taskComplete)
        {
            UnityEngine.Debug.Log("End");
        }
    }

    // 1秒遅延させてタスク開始するコルーチン
    IEnumerator DelayedStartTask(float delayTime,int task)
    {
        // タスク開始前の表示
        UnityEngine.Debug.Log(delayTime + "秒後にタスクを開始します");
        float winZ = 0;
        float objZ = 0;
        //手前から奥のタスクの時
        if (task == 2)
        {
            winZ = winBackPos;
            objZ = objFrontPos;
            goal = winFrontPos;
        }
        else if (task == 3)
        {
            winZ = winFrontPos;
            objZ = objBackPos;
            goal = winBackPos;
        }
        window.transform.position = new Vector3(window.transform.position.x, window.transform.position.y, winZ);
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, objZ);
        window.GetComponent<Renderer>().material.color = initialColor; //ウィンドウの色

        // 1秒待機
        yield return new WaitForSeconds(delayTime);

        this.gameObject.GetComponent<ObjectController>().isTaskRunning = true;
        taskStopwatch.Restart(); // ストップウォッチスタート
        UnityEngine.Debug.Log(times+"回目のタスク開始");
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

    //ボール位置の更新
    private void UpdateBall()
    {
        int input = UDPReceiver.receivedInt;
        if (UnityEngine.Input.GetKey(KeyCode.W))
        {
            input = 2;
            UnityEngine.Debug.Log("2desuu");
        }else if (UnityEngine.Input.GetKey(KeyCode.S))
        {
            input = 3;
        }
        UnityEngine.Debug.Log(input+input.ToString());
        inputRow.Add(input.ToString());//脳波入力をリストに追加
        // 前進
        if (input == 2 && task==2)
        {
            Vector3 direction = new Vector3(0, 0, 1);
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            this.gameObject.transform.position += movement;
        }

        // 後進
        else if (input == 3 && task == 3)
        {
            Vector3 direction = new Vector3(0, 0, -1);
            Vector3 movement = direction.normalized * moveSpeed * Time.deltaTime;
            this.gameObject.transform.position += movement;
        }
    }

    // 静止したかチェック
    private void CheckStill()
    {
        if(UDPReceiver.receivedInt == 1|UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            taskStopwatch.Stop();
            double score=taskStopwatch.Elapsed.TotalSeconds; // かかった時間
            // 今回のタスクのタイムスコアをdataに追加
            scoreData.Add(new List<string> { times.ToString(), score.ToString() });
            // タイムスコアの書き出し
            UnityEngine.Debug.Log(filename);
            // 書き出し先のファイルパス（プロジェクトフォルダのルートに書き出す）
            string filePath = Path.Combine(Application.dataPath, user_name + "_TaskScore_"+filename);
            WriteToCSV(filePath, scoreData);
            // 脳波データの書き出し
            inputData.Add(inputRow);
            inputRow = new List<string>();// 脳波入力の行リストの初期化
            filePath= Path.Combine(Application.dataPath, user_name + "_EEG_Input_" + filename);
            WriteToCSV(filePath, inputData);
            UnityEngine.Debug.Log("タスククリア(1を検出): " + times + "回目: " + score + "s");

            //次の準備
            this.gameObject.transform.position = initialPosition;
            this.gameObject.GetComponent<ObjectController>().isTaskRunning = false;
            window.GetComponent<Renderer>().material.color = noColor;//無色にする
            goalOver = false;
            times++; // 次のタスクへ移行
            task = 1;
        }
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

    private void StartDelayTimer()
    {
        delayStopwatch.Start();
    }
}