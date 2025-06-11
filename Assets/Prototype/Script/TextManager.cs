using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class TextManager : MonoBehaviour
{
    [Header("スペースキー,マウスクリックで次のセリフへ進む")]
    [SerializeField, Header("名前表示用")] private Text nameText;
    [SerializeField, Header("セリフ表示用")] private Text mainText;
    [SerializeField, Header("名前表示用(TextMeshPro版)")] TMP_Text nameText_Pro = null;
    [SerializeField, Header("セリフ表示用(TextMeshPro版)")] TMP_Text mainText_Pro = null;
    [SerializeField, Header("Resources/Textsフォルダにあるテキストファイルリスト")] private List<string> scenarioFile;
    [SerializeField, Header("1文字ごとの表示速度")] private float charInterval = 0.05f;
    [SerializeField, Header("TMPro版を使用する場合はチェックを入れる")] private bool isUseTMPro = false;
    [SerializeField, Header("オートモード時１行ごとに待つ時間(秒)")] private float autoDelay = 2.0f;
    [SerializeField, Header("オートモードに切り替え(Tキーで変えられる)")] private bool isAuto;
    [Header("各オブジェクト")]
    [SerializeField, Header("背景パネル")] private GameObject backgroundPanel;
    [SerializeField, Header("メインテキスト")] private GameObject mainTextObj;
    [SerializeField, Header("名前テキスト")] private GameObject nameTextObj;
    [SerializeField, Header("メインテキスト(TMPro)")] private GameObject mainiTextProObj;
    [SerializeField, Header("名前テキスト(TMPro)")] private GameObject nameTextProObj;

    [Header("※ここからはさわらない※")]
    public string spriteDirectory = "Sprites/";
    [SerializeField, Header("立ち絵のオブジェクト")] private GameObject characterImages;
    private string prefabsDirectory = "Prefabs/";
    private List<Image> _charaImageList = new List<Image>();

    private Queue<char> _charQueue; // 文字列を格納するキュー
    private Queue<string> _pageQueue;
    private Queue<RichChar> _richCharQueue;

    private const string CHARACTER_IMAGE_PREFAB = "CharacterImage";

    private string _text = "";

    public bool talkFlg = false;

    private TimeManager timeMng;

    // 構造体定義

    /// <summary>
    /// 区切り文字
    /// </summary>
    private readonly struct S_Separate
    {
        public readonly static char MainStart = '「';
        public readonly static char MainEnd = '」';
        public readonly static char NextPage = '&';
        public readonly static char Command = '!';
        public readonly static char Param = '=';
    }

    /// <summary>
    /// コマンド
    /// </summary>
    private readonly struct S_Command
    {
        public const string CharacterImage = "charaimg";
        public const string Position = "_pos";
        public const string Size = "_size";
        public const string Rotation = "_rotate";
        public const string Sprite = "_sprite";
        public const string Color = "_color";
    }

    private readonly struct S_Prefab
    {
        public const string CharaImage = "CharaImage";
    }

    /// <summary>
    /// 1文字とその前後のタグを保持する構造体
    /// </summary>
    private struct RichChar
    {
        public string visibleChar;  // 実際に表示される文字
        public string prefixTag;    // 前につけるタグ
        public string suffixTag;    // 閉じタグ

        public RichChar(string visible, string prefix = "", string suffix = "")
        {
            visibleChar = visible;
            prefixTag = prefix;
            suffixTag = suffix;
        }

        public override string ToString()
        {
            return $"{prefixTag}{visibleChar}{suffixTag}";
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeMng = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if(!timeMng)
        {
            Debug.LogError("Script:TextManager.cs \n" +
              "TimeManagerがnullです");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!isAuto && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)))
            OnClick(false);

        // Tキーでオートモード切り替え
        if(Input.GetKeyDown(KeyCode.T))
        {
            isAuto = !isAuto;
            Debug.Log("モードを切り替えました。オートモード:" + isAuto);
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Init()
    {
        _text = LoadTextFile(scenarioFile[0].ToString());
        _pageQueue = SeparateString(_text, S_Separate.NextPage);
        ShowNextPage();

    }

    //============== 外部スクリプトから呼び出す用 ================
    /// <summary>
    /// 外部用　会話のスタート
    /// </summary>
    /// <param name="talkNum"></param>
    public void StartTalk(int talkNum, bool Auto = true)
    {

        isAuto = Auto;
        talkFlg = true;
        //backgroundPanel.SetActive(true);
        if (scenarioFile[talkNum] == null)
            Debug.LogError($"テキストファイルリスト番号{talkNum}番のテキストファイルがリストに登録されていません。");
        else
            _text = LoadTextFile(scenarioFile[talkNum].ToString());

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
        // 最初が「!」だったら
        if (text[0].Equals(S_Separate.Command))
        {
            ReadCommand(text);
            ShowNextPage();
            return;
        }

        // '「'の位置で文字列を分割
        string[] ts = text.Split(S_Separate.MainStart);
        if (ts.Length < 2)
        {
            Debug.Log("想定される形式ではないのでScenarioファイルを書き直してください。");
        }
        // 分けて最初の値、名前を代入
        string name = ts[0];
        // 名前の次の値、「」で囲われた文字が代入される
        string main = ts[1].Remove(ts[1].LastIndexOf(S_Separate.MainEnd));


        if (!isUseTMPro)
        {
            nameTextProObj.SetActive(false);
            mainiTextProObj.SetActive(false);
            nameText.text = name;
            mainText.text = "";
            _charQueue = SeparateString(main);
        }
        else
        {
            nameTextObj.SetActive(false);
            mainTextObj.SetActive(false);
            nameText_Pro.text = name;
            mainText_Pro.text = "";
            _richCharQueue = SeparateRichString(main);
        }

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
    private Queue<string> SeparateString(string str, char sep)
    {
        string[] strs = str.Split(sep);
        Queue<string> queue = new Queue<string>();
        foreach (string l in strs) queue.Enqueue(l);
        return queue;
    }

    private Queue<RichChar> SeparateRichString(string str)
    {
        Queue<RichChar> richQueue = new Queue<RichChar>();
        int i = 0;
        //string currentTag = "";
        Stack<string> tagStack = new Stack<string>();

        while (i < str.Length)
        {
            if (str[i] == '<')
            {
                int closeIndex = str.IndexOf('>', i);
                if (closeIndex == -1) break;

                string tag = str.Substring(i, closeIndex - i + 1);
                if (!tag.Contains("</"))
                {
                    if (tagStack.Count > 0) tagStack.Pop();
                }
                else
                {
                    tagStack.Push(tag);
                }

                i = closeIndex + 1;
            }
            else
            {
                string currentChar = str[i].ToString();
                string combiedPrefix = string.Concat(tagStack.ToArray());

                string closingTags = "";
                foreach (var t in tagStack)
                {
                    if (t.StartsWith("<") && !t.StartsWith("</"))
                    {
                        int spaceIndex = t.IndexOf(' ');
                        int equalIndex = t.IndexOf('=');
                        int tagNameEnd = spaceIndex != -1 ? spaceIndex : (equalIndex != -1 ? equalIndex : t.IndexOf('>'));
                        if (tagNameEnd > 0)
                        {
                            string tagName = t.Substring(1, tagNameEnd - 1);
                            closingTags = $"</{tagName}>" + closingTags;
                        }
                    }
                }

                richQueue.Enqueue(new RichChar
                {
                    visibleChar = currentChar,
                    prefixTag = combiedPrefix,
                    suffixTag = closingTags
                });
                i++;
            }


        }
        return richQueue;
    }

    /// <summary>
    /// 1文字を出力
    /// </summary>
    private bool OutPutChar()
    {
        if (!isUseTMPro)
        {
            // キューに何も格納されていなければfalseを返す
            if (_charQueue.Count <= 0) return false;
            mainText.text += _charQueue.Dequeue();
        }
        else
        {
            if (_richCharQueue.Count <= 0) return false;
            mainText_Pro.text += _richCharQueue.Dequeue().ToString();
        }
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
            yield return new WaitForSecondsRealtime(wait);
        if(isAuto)
        {
            yield return new WaitForSecondsRealtime(autoDelay);
            OnClick(false);
        }

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
    private void OnClick(bool auto)
    {

        if (!isUseTMPro)
        {
            if (_charQueue.Count > 0)
                OutputAllChar();
            else
            {
                if (!ShowNextPage())
                {
                    // UIを非表示にする
                    talkFlg = false;
                    backgroundPanel.SetActive(false);
                }
            }
        }
        else
        {
            if (_richCharQueue.Count > 0)
                OutputAllChar();
            else
            {
                if (!ShowNextPage())
                {
                    // UIを非表示にする
                    talkFlg = false;
                    backgroundPanel.SetActive(false);
                }
            }
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

    /// <summary>
    /// テキストファイルを読みこむ
    /// </summary>
    /// <param name="fname"></param>
    /// <returns></returns>
    private string LoadTextFile(string fname)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fname);
        if (textAsset == null)
        {
            Debug.Log("テキストファイルが見つかりません");
        }
        return textAsset.text.Replace("\\n", "\n").Replace("\r", "");
    }

    //============== コマンド・パラメータ設定 ===============

    private string ConvertColorTags(string input)
    {
        Dictionary<string, string> tagDict = new Dictionary<string, string>()
        {
            {"!red","<color=red>" },
            {"!green","<color=green>" },
            {"!blue","<color=blue>" },
            {"!yellow","<color=yellow>" },
            {"!black","</color>" },
            {"!end","</color>" },
        };

        foreach (var pair in tagDict)
        {
            input = input.Replace(pair.Key, pair.Value);
        }
        return input;
    }

    private void ReadCommand(string cmdLine)
    {
        // 最初の「!」を消す
        cmdLine = cmdLine.Remove(0, 1);
        Queue<string> cmdQueue = SeparateString(cmdLine, S_Separate.Command);
        foreach(string cmd in cmdQueue)
        {
            string[] cmds = cmd.Split(S_Separate.Param);
            if (cmds[0].Contains(S_Command.CharacterImage))
                SetCharacterImage(cmds[1], cmds[0], cmds[2]);
        }
    }

    /// <summary>
    /// 立ち絵をファイルから読み出し、生成する
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private Sprite LoadSprite(string name)
    {
        return Instantiate(Resources.Load<Sprite>(spriteDirectory + name));
    }

    private void SetImage(string cmd,string parameter,Image image)
    {
        cmd = cmd.Replace(" ", "");
        parameter = parameter.Substring(parameter.IndexOf('"') + 1, parameter.LastIndexOf('"') - parameter.IndexOf('"') - 1);
        switch(cmd)
        {
            case S_Command.Sprite:
                image.sprite = LoadSprite(parameter);
                break;
            case S_Command.Size:
                image.GetComponent<RectTransform>().sizeDelta = ParameterToVector3(parameter);
                break;
            case S_Command.Position:
                image.GetComponent<RectTransform>().anchoredPosition = ParameterToVector3(parameter);
                break;
            case S_Command.Rotation:
                image.GetComponent<RectTransform>().eulerAngles = ParameterToVector3(parameter);
                break;
        }
    }

    /// <summary>
    /// 立ち絵の設定
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cmd"></param>
    /// <param name="parameter"></param>
    private void SetCharacterImage(string name,string cmd,string parameter)
    {
        cmd = cmd.Replace(S_Command.CharacterImage, "");
        name = name.Substring(name.IndexOf('"') + 1, name.LastIndexOf('"') - name.IndexOf('"') - 1);
        Image image = _charaImageList.Find(n => n.name == name);
        if(image == null)
        {
            image = Instantiate(Resources.Load<Image>(prefabsDirectory +  S_Prefab.CharaImage), characterImages.transform);
            image.name = name;
            _charaImageList.Add(image);
        }
        SetImage(cmd, parameter, image);
    }

    private Vector3 ParameterToVector3(string parameter)
    {
        string[] ps = parameter.Replace(" ", "").Split(',');
        return new Vector3(float.Parse(ps[0]), float.Parse(ps[1]), float.Parse(ps[2]));
    }
}
