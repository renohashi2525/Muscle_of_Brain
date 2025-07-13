using System.Collections; // コレクション（リスト、配列など）の基本クラスを提供します。
using System.Collections.Generic; // ジェネリックコレクション（List<T>、Dictionary<TKey,TValue>など）を提供します。
using UnityEngine; // Unityの基本機能（ゲームオブジェクト、コンポーネント、シーン管理など）を提供します。
using UnityEngine.UI; // UnityのUIコンポーネント（ボタン、テキスト、画像など）を操作するための機能を提供します。
using TMPro; // TextMeshPro（高品質なテキスト表示用）に関するクラスを提供します。
using UnityEngine.SceneManagement; // Unityのシーン管理（シーンの読み込みや切り替えなど）を提供します。
using UnityEngine.Networking; // UnityでのHTTPリクエストやネットワーク通信を扱うためのクラスを提供します。
using System.Linq; // LINQ（言語統合クエリ）を使用してコレクションを操作するための機能を提供します。
using Newtonsoft.Json.Linq; // Newtonsoft.Jsonを使用するために必要 // JSONデータの操作と解析を行うためのクラスを提供します（Newtonsoft.Jsonライブラリ）。


public class SampleGameScript : MonoBehaviour
{
    public int listNum; // クイズ数をカウント
    // jsonファイル対応用unity内フィールド
    private QuizList quizList; // こちらのリストにjsonデータを挿入
    public TextMeshProUGUI quizNum;
    public TextMeshProUGUI quiz;
    public TextMeshProUGUI button1;
    public TextMeshProUGUI button2;
    public TextMeshProUGUI button3;
    public TextMeshProUGUI button4;
    public GameObject resultUi; // リザルト画面
    private string fullText; // 問題文代入用
    private string currentText = ""; // 現在の問題文を徐々に代入
    public float typingSpeed = 0.02f; // 文章出すスピード
    // クイズ番号をランダムに選定
    int randomNumber;
    // FirestoreのURL
    private string firestoreUrl = "https://firestore.googleapis.com/v1/projects/amity-4bad3/databases/(default)/documents/machoQuiz/quiz";
    // 表示済みクイズ追跡
    private List<int> displayedQuizIndices = new List<int>() { 0 };
    // 表示可能問題番号
    List<int> possibleNumbers  = new List<int>();
    // jsonファイル用のクラスと変数を定義
    [System.Serializable]
    public class QuizValue{
        public string quiztext;
        public string button1;
        public string button2;
        public string button3;
        public string button4;
    }

    // jsonファイル用のリストを定義
    [System.Serializable]
    public class QuizList {
        public List<QuizValue> quizs;
    }

    // jsonファイル読み込み＆初回表示
    void Start()
    {
        // 変数初期化
        listNum = 0;
        // 1からxまでの数字をリストに追加(x=クイズ数)
        for (int i = 1; i <= 30; i++)
        {
            possibleNumbers.Add(i);
        }
        // quizListの初期化
        quizList = new QuizList { quizs = new List<QuizValue>() };
        Debug.Log("Start");
    }

    // 問題の文章、選択肢を表示
    void Display(List<QuizValue> quizs, int listNum)
    {
        currentText = "";
        fullText = "";
        if (listNum >= quizList.quizs.Count) 
        {
            //不要?
            resultUi.SetActive(true);
        }else
        {
            quizNum.text = (listNum + 1).ToString();
            fullText = quizs[listNum].quiztext;

            StartCoroutine(TypeText());
            button1.text = quizs[listNum].button1;
            button2.text = quizs[listNum].button2;
            button3.text = quizs[listNum].button3;
            button4.text = quizs[listNum].button4;
        }
    }

    // 文章を決まったスピードで徐々に出力
    IEnumerator TypeText()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText += fullText[i];
            quiz.text = currentText;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void StartQuiz()
    {
        GetDataFromFirestore();
        Debug.Log("StartQuiz");
    }

    // Firebaseに関するコード
    public void GetDataFromFirestore()
    {
        Debug.Log("GetDataFromFirestore-Start");
        StartCoroutine(FetchDataFromFirestore());
        Debug.Log("GetDataFromFirestore-End");
    }
    // Firestoreからデータを取得するコルーチン
    IEnumerator FetchDataFromFirestore()
    {
        Debug.Log("FetchDataFromFirestore");
        // 含まれていない数字を取得
        possibleNumbers = possibleNumbers.Except(displayedQuizIndices).ToList();
        // 含まれていない数字が存在する場合&クイズの出題数が7問以下の時
        if (possibleNumbers.Count > 0)
        {
            randomNumber = possibleNumbers[Random.Range(0, possibleNumbers.Count)]; // ランダムに選択
            displayedQuizIndices.Add(randomNumber); // 選ばれた数字をリストに追加
        }
        else
        {
            Debug.Log("すべての数字が表示済みです。");
            quizNum.text = "";
            quiz.text = "";
            button1.text = "";
            button2.text = "";
            button3.text = "";
            button4.text = "";
            resultUi.SetActive(true);
            yield break;
        }
        // Firestore APIキー
        string apiKey = "AIzaSyAjRLVe_pXqiO8eZs2CHGqN08VSo5DqjAc"; 
        string urlWithApiKey = firestoreUrl + randomNumber + "?key=" + apiKey;

        // HTTPリクエストの作成
        UnityWebRequest request = new UnityWebRequest(urlWithApiKey, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // リクエスト送信
        yield return request.SendWebRequest();

        // レスポンスの確認
        if (request.result == UnityWebRequest.Result.Success)
        {
            ProcessFetchedData(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Firestoreからのデータ取得に失敗: " + request.error);
        }
    }

    // 取得したデータを処理する関数
    private void ProcessFetchedData(string jsonDataGet)
    {
        Debug.Log("ProcessFetchedData");
        // JSONをJObjectとして解析
        JObject json = JObject.Parse(jsonDataGet);

        // フィールドを持つFirestoreレスポンスを取得
        var fields = json["fields"];

        // fieldsがnullでないか確認
        if (fields != null)
        {
            // Firestoreのレスポンスの各フィールドにアクセス
            QuizValue quizValue = new QuizValue
            {
                quiztext = fields["quiztext"]?["stringValue"]?.ToString(),
                button1 = fields["button1"]?["stringValue"]?.ToString(),
                button2 = fields["button2"]?["stringValue"]?.ToString(),
                button3 = fields["button3"]?["stringValue"]?.ToString(),
                button4 = fields["button4"]?["stringValue"]?.ToString()
            };

            // 作成したQuizValueオブジェクトをリストに追加
            quizList.quizs.Add(quizValue);

            // 表示の更新
            Display(quizList.quizs, listNum);
        }
        else
        {
            Debug.LogError("fieldsが見つかりませんでした。Firestoreレスポンスを確認してください。");
        }
    }

    // Firestoreからのレスポンス構造に対応するクラス
    [System.Serializable]
    public class FirestoreResponse
    {
        public Dictionary<string, FirestoreStringValue> fields; // フィールドのディクショナリ
        public string name; // ドキュメントの名前
        public string createTime; // 作成時間
        public string updateTime; // 更新時間
    }

    [System.Serializable]
    public class FirestoreStringValue
    {
        public string stringValue; // 各フィールドの値
    }

    public void StageSelect()
    {
        SceneManager.LoadScene("StageSelect");
    }
}
