using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class TextManager : MonoBehaviour
{
    [Header("スペースキーで次の行へ、Aキーでオート切り替え")]
    [SerializeField, Header("名前表示用")] private Text nameText;
    [SerializeField, Header("セリフ表示用")] private Text mainText;
    [SerializeField, Header("Resources/Textsフォルダにあるテキストファイル名")] private string scenarioFile = "Texts/Scenario";
    [SerializeField, Header("1文字ごとの表示速度")] private float charInterval = 0.05f;
    //[SerializeField, Header("オートモード時１行ごとに待つ時間(秒)")] private float autoDelay = 2.0f;
    [SerializeField, Header("背景パネル")] private GameObject backgroundPanel;

    private Queue<char> _charQueue; // 文字列を格納するキュー
    private Queue<string> _pageQueue;

    private const string CHARACTER_IMAGE_PREFAB = "CharacterImage";

    private string _text = "";



    // 構造体定義
    
    /// <summary>
    /// 区切り文字
    /// </summary>
    private readonly struct S_Separate
    {
        public readonly static char MainStart = '「';
        public readonly static char MainEnd = '」';
        public readonly static char NextPage = '&';
    }

    /// <summary>
    /// コマンド
    /// </summary>
    private readonly struct S_Command
    {
        public readonly static string CharacterImage = "charaimg";
        public readonly static string Position = "_pos";
        public readonly static string Size = "_size";
        public readonly static string Rotation = "_rotate";
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            OnClick();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Init()
    {
        _text = LoadTextFile(scenarioFile);
        _pageQueue = SeparateString(_text, S_Separate.NextPage);
        ShowNextPage();
        
    }

    //============== 文字列関連処理 ===============

    /// <summary>
    /// 1行を読み出す関数
    /// </summary>
    /// <param name="text"></param>
    private void ReadLine(string text)
    {
        // '「'の位置で文字列を分割
        string[] ts = text.Split(S_Separate.MainStart);
        if(ts.Length < 2)
        {
            Debug.Log("想定される形式ではないのでScenarioファイルを書き直してください。");
        }
        // 分けて最初の値、名前を代入
        string name = ts[0];
        // 名前の次の値、「」で囲われた文字が代入される
        string main = ts[1].Remove(ts[1].LastIndexOf(S_Separate.MainEnd));
        nameText.text = name;
        mainText.text = "";
        _charQueue = SeparateString(main);
        StartCoroutine(ShowChars(charInterval));
    }

    /// <summary>
    /// 文を1文字ごとに区切って、キューに格納したものを返す
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private Queue<char> SeparateString(string str)
    {
        // 文字列をchar型の配列に格納＝1文字ごとに区切る
        char[] chars = str.ToCharArray();
        Queue<char> charQueue = new Queue<char>();
        // 配列に格納された文字をすべて取り出してキューに加える
        foreach (char c in chars) charQueue.Enqueue(c);
        return charQueue;
    }

    /// <summary>
    /// 文字列を区切り文字ごとに区切って、キューに格納したものを返す
    /// </summary>
    /// <param name="str"></param>
    /// <param name="sep"></param>
    /// <returns></returns>
    private Queue<string> SeparateString(string str,char sep)
    {
        string[] strs = str.Split(sep);
        Queue<string> queue = new Queue<string>();
        foreach (string l in strs) queue.Enqueue(l);
        return queue;
    }

    /// <summary>
    /// 1文字を出力
    /// </summary>
    private bool OutPutChar()
    {
        // キューに何も格納されていなければfalseを返す
        if (_charQueue.Count <= 0) return false;
        mainText.text += _charQueue.Dequeue();
        return true;
    }

    /// <summary>
    /// 文字送りコルーチン
    /// </summary>
    /// <param name="wait"></param>
    /// <returns></returns>
    private IEnumerator ShowChars(float wait)
    {
        // キューが空になるまでループ
        while (OutPutChar())
            // wait分待機
            yield return new WaitForSeconds(wait);
        // コルーチンを抜ける
        yield break;
    }

    /// <summary>
    /// 全文表示
    /// </summary>
    private void OutputAllChar()
    {
        // コルーチンストップ
        StopCoroutine(ShowChars(charInterval));
        // キューが空になるまで表示
        while (OutPutChar()) ;
    }

    /// <summary>
    /// クリックしたときの処理
    /// </summary>
    private void OnClick()
    {
        if(_charQueue.Count > 0)
            OutputAllChar();
        else
        {
            if (!ShowNextPage())
                // UIを非表示にする
                backgroundPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 次のページ表示
    /// </summary>
    /// <returns></returns>
    private bool ShowNextPage()
    {
        if (_pageQueue.Count <= 0) return false;
        ReadLine(_pageQueue.Dequeue());
        return true;
    }

    private string LoadTextFile(string fname)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fname);
        if(textAsset==null)
        {
            Debug.Log("テキストファイルが見つかりません");
        }
        return textAsset.text.Replace("\n", "").Replace("\r", "");
    }

    //============== コマンド・パラメータ設定 ===============
}
