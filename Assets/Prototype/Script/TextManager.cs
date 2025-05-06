using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class TextManager : MonoBehaviour
{
    [Header("スペースキーで次の行へ、Aキーでオート切り替え")]
    [SerializeField, Header("名前表示用")] private Text nameText;
    [SerializeField, Header("セリフ表示用")] private Text mainText;
    [SerializeField, Header("Resourcesフォルダにあるテキストファイル名")] private string scenarioFileName = "scenario";
    [SerializeField, Header("1文字ごとの表示速度")] private float charInterval = 0.05f;
    [SerializeField, Header("オートモード時１行ごとに待つ時間(秒)")] private float autoDelay = 2.0f;
    [SerializeField, Header("背景パネル")] private GameObject backgroundPanel;

    // 読み込んだテキストの行ごとの配列
    private string[] lines;
    // 表示している行番号
    private int currentLine = 0;
    // 文字を表示中かどうか
    private bool isTyping = false;
    // 表示中に全文スキップして即表示するか
    private bool isSkip = false;
    // オートモードが有効か
    private bool isAuto = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Resourcesからテキストファイルを読み込む
        TextAsset textAsset = Resources.Load<TextAsset>(scenarioFileName);
        if (textAsset != null)
        {
            // 改行で分割して1行ずつにする
            lines = textAsset.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            // 最初の行を表示
            StartCoroutine(TypeNextLine());
        }
        else
        {
            Debug.LogError($"scenarioファイル'{scenarioFileName}'が見つかりません。\n"
                + "Resourceフォルダ内に配置してください。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // スペースキーを押したときの処理(オートモード中は無効)
        if (Input.GetKeyDown(KeyCode.Space) && !isAuto)
        {
            if (isTyping)
            {
                // 表示途中ならスキップ(全文即表示)
                isSkip = true;
            }
            else
            {
                // 次の行の表示開始
                StartCoroutine(TypeNextLine());
            }
        }

        // Aキーでオートモードの切り替え
        if (Input.GetKeyDown(KeyCode.A))
        {
            isAuto = !isAuto;
            Debug.Log("Text Auto Mode:" + (isAuto ? "ON" : "OFF"));
        }
    }

    /// <summary>
    /// 現在の行の文字を1文字ずつ表示するコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator TypeNextLine()
    {
        // 行が終わっていたら終了
        if (currentLine >= lines.Length) yield break;

        // 状態を更新
        isTyping = true;
        isSkip = false;

        // 表示欄を初期化
        mainText.text = "";
        nameText.text = "";

        // 現在の行を取得・前後の空白を除去
        string line = lines[currentLine].Trim();
        currentLine++;

        // 話者名とセリフに分割（「名前:セリフ」形式）
        string speaker = "";
        string message = line;

        int colonIndex = line.IndexOf(':');
        if (colonIndex != -1)
        {
            speaker = line.Substring(0, colonIndex).Trim();
            message = line.Substring(colonIndex + 1).Trim();
        }

        // 話者名をUIに表示
        nameText.text = speaker;

        // 表示済みの文字列
        string displayedText = "";

        // 現在開いているタグを記録するスタック
        Stack<string> tagStack = new Stack<string>();

        // セリフを一文字ずつ表示
        int i = 0;
        while (i < message.Length)
        {
            // スキップ指定時は全文表示して中断
            if (isSkip)
            {
                mainText.text = message;
                break;
            }

            // <タグ>（色指定など）がある場合はまとめて処理
            if (message[i] == '<')
            {
                int closeIndex = message.IndexOf('>', i);
                if (closeIndex != -1)
                {
                    string tag = message.Substring(i, closeIndex - i + 1);

                    // 終了タグ(</...>)ではない場合、スタックに積む
                    if (!tag.Contains("/"))
                    {
                        tagStack.Push(tag);
                    }
                    else if(tagStack.Count > 0)
                    {
                        // 終了タグならスタックから取り出す
                        tagStack.Pop();
                    }

                    i = closeIndex + 1;
                    continue;
                }
            }

            // 現在の1文字を取得
            string currentChar = message[i].ToString();

            // 開いているすべてのタグを再構築
            string openTags = string.Concat(tagStack.ToArray().Reverse());  // スタックの順序を正してたぐを開く
            string closeTags = string.Concat(tagStack.Select(t => "</" + t.Substring(1)));  // 対応する終了タグを生成

            // 表示：開いているタグ + 現在文字 + 閉じタグ
            mainText.text = openTags + displayedText + currentChar + closeTags;

            // 指定間隔待機
            yield return new WaitForSeconds(charInterval);

            // 表示済みテキストに現在の1文字を追加
            displayedText += currentChar;
            i++;
        }

        // 表示完了
        isTyping = false;

        // Autoモード中なら一定時間後に次の行を自動で表示
        if (isAuto)
        {
            yield return new WaitForSeconds(autoDelay);
            StartCoroutine(TypeNextLine());
        }
    }
}
