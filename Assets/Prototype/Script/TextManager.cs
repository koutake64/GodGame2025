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
    [SerializeField, Header("１行ごとに待つ時間(秒)")] private float autoDelay = 2.0f;
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
        // 最後の行を超えたら終了
        if (currentLine >= lines.Length)
        {
            // パネル非表示
            if (backgroundPanel != null) backgroundPanel.SetActive(false);
            yield break;
        }

        // 状態初期化
        isTyping = true;
        isSkip = false;
        mainText.text = "";
        nameText.text = "";

        // 現在の行を取得、前後の空白を除去
        string line = lines[currentLine].Trim();
        currentLine++;

        // 名前とセリフに分割
        string speaker = "";
        string message = line;

        int colonIndex = line.IndexOf(':');
        if (colonIndex != -1)
        {
            speaker = line.Substring(0, colonIndex).Trim();
            message = line.Substring(colonIndex + 1).Trim();
        }

        // 名前をUIに表示
        nameText.text = speaker;

        // セリフを1文字ずつ表示（タグは非表示で装飾のみに使う）
        int i = 0;
        string displayText = ""; // ユーザーに見せるテキスト（タグ含むが<>は非表示）
        Stack<string> tagStack = new Stack<string>();
        while (i < message.Length)
        {
            // スキップ時は全文表示して中断
            if (isSkip)
            {
                mainText.text = message; // スキップ時はタグごと全文表示
                break;
            }

            // ▼ タグ検出（例: <color=red>）: タグは1文字ずつ表示せず一括で追加する
            if (message[i] == '<')
            {
                int closeIndex = message.IndexOf('>', i);
                if (closeIndex != -1)
                {
                    string tag = message.Substring(i, closeIndex - i + 1); // <～> を抽出

                    if(tag.StartsWith("</"))
                    {
                        tagStack.Pop();
                        displayText += tag;
                    }
                    else
                    {
                        tagStack.Push(tag);
                        displayText += tag;
                    }
                    i = closeIndex + 1;
                    continue;
                }
            }

            // 現在のタグをすべて適応した状態で1文字追加
            string combined = "";
            foreach (string tag in tagStack)
                combined += tag;

            combined += message[i];

            foreach(string tag in tagStack.Reverse())
            {
                if(tag.StartsWith("<") && !tag.StartsWith("</"))
                {
                    string tagName = tag.Substring(1, tag.IndexOf('=') > 0 ? tag.IndexOf('=') - 1 : tag.Length - 2);
                    combined += $"</{tagName}>";
                }
            }

            displayText += combined;
            mainText.text = displayText;
            i++;

            // 指定間隔待機
            yield return new WaitForSeconds(charInterval);
        }

        // 表示完了
        isTyping = false;

        // オート中なら一定時間後に次の行を自動で表示
        if (isAuto)
        {
            yield return new WaitForSeconds(autoDelay);
            StartCoroutine(TypeNextLine());
        }
    }
}
