セットアップ編
1. MATLABセットアップをする
    ・EMOTIV PROの設定により，LSLを有効にする

      ・MATLABに脳波データをリアルタイムで送信する

    ・MATLABで，MATLAB Code => LSL => vis_stream.mを実行

      ・チャンネル数を(1 19)，サンプリングレートを(256)に設定しスタートすると，脳波データがリアルタイムで可視化される

    ・この実行により，LSLをMATLABにロードしているため最初にこの手順を踏む
   
2. Unityのセットアップをする
    ・PCとQuest3をC　to　Cケーブルで繋ぐ（送信速度が遅いケーブルだと処理落ちにつながる）

    ・UnityはQuest Linkを有効にした状態で，Unityを実行しVR環境内に入る


Trainingシーン（トレーニングのためのScene）
1. MATLAB操作
    ・MATLABのMATLAB Code => Spatial Audio => Caribrarion.mファイルを実行する

    ・Startボタンを押して脳はデータの記録を開始する

      ・Stopボタンを押すまでの脳波データが記録される

    ・LabelとStopボタンにはトレーニング終了まで触れない

      ・LabelはUnityでの操作と同時に自動で記録されるようになっている

2. Unity操作
    ・MATLABで脳波データの記録を開始したら，Unityを実行する

    ・Spaceキーでトレーニングを開始する

      ・Unityのコンソールにトレーニング開始の出力，MATLABのコンソールにラベルの記録がされていることを確認する

3. 必要なデータ分繰り返す
    ・５分間計測し，5分経ったら実行を止めてもう一度実行し繰り返す

    ・今回の実験では，4回繰り返し，計20分間分のデータを取得する

      ・トレーニング中，MATLABの実行を止めないことに注意

4. 必要なデータを取得したらトレーニングを終了する
    ・Unityは実行を止める

    ・MATLABはStopボタンを押して終了

    ・データ取得後，MATLABでは解析が自動で進むため，実行が終了されるまで，数分間待つ

      ・データをDataフォルダにコピーして終了


VR Manipulationシーン(オブジェクト操作確認のためのシーン)
1. MATLAB操作
    ・トレーニングデータをロードした状態で，MATLABで ReceiveData.mファイルを実行する

    ・実行すると，10秒後から設定した間隔で，現在の脳波状態を分類器が分類しリアルタイムで出力する

      ・MATLABはこの状態にしておけば大丈夫

    ・止める時は脳波データ記録終了ボタンを押して実行を終了する

3. Unity操作
    ・MATLABでUnityに出力結果を送信されるようになったら，Unityを実行する

    ・Spaceキーを押すと入力を受け付けるモードに変わる

      ・押す度にON/OFFの切り替えが可能で，Unityのコンソール画面で確認できる
    ・オブジェクト操作はキー操作とMATLABからの出力結果の2通りを受け付ける

      ・キー操作はWキーでオブジェクトを前方移動，Sキーでオブジェクトを後方移動

      ・脳波制御はMATLABからの出力で決定する

    ・UnityとMATLABのコンソールを確認し，ズレがないか観察
