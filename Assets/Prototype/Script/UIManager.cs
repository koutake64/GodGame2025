using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

/// <summary>
/// UIを管理するクラス
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("各UIオブジェクトのリスト")]
    [SerializeField] private List<UIEntry> UIEntries = new List<UIEntry>();

    private Dictionary<E_UI_KIND, GameObject> UIDictionary = new Dictionary<E_UI_KIND, GameObject>();

    /// <summary>
    /// UIの種類の列挙体
    /// </summary>
    public enum E_UI_KIND
    {
        memo,       // メモ
        timeIcon,   // 時間アイコン
        gameOver,   // ゲームオーバー
        performance,// 画面演出
        backPanel,  // 背景パネル
    }

    /// <summary>
    /// UIオブジェクトと種類をセットで保存するクラス
    /// </summary>
    [System.Serializable]
    public class UIEntry
    {
        public E_UI_KIND kind;
        public GameObject uiObject;
    }


    private Animator memoAnimator;  // メモUIアニメーター
    private TimeManager timeMng;
    private _FieldDataManager fieldDataMng; // フィールドデータ

    private bool useMemo;       // メモを開いているかどうか
    private bool currentFlag;   // 現在のフラグ状況
    private bool prevFlag;      // 1フレーム前のフラグ状況



    // InputSystem
    private PlayerInput playerInput;
    private InputAction switchAction;
    public InputActionReference switchActionRef;    // 必要なデータだけ


	/// <summary>
	/// 初期化
	/// </summary>
	private void Awake()
    {
        // InputSystem
        playerInput = GetComponent<PlayerInput>();
        //switchAction = playerInput.actions["SwitchAction"];
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Start()
    {
        // UIEntriesからUIDictionaryに変換
        foreach(var entry in UIEntries)
        {
            if(entry != null && entry.uiObject != null && !UIDictionary.ContainsKey(entry.kind))
            {
                // 種類ごとにGameObjectを登録
                UIDictionary[entry.kind] = entry.uiObject;
            }
        }

        // 各オブジェクトからAnimatorを取得＆nullチェック
        if (UIDictionary.TryGetValue(E_UI_KIND.memo, out GameObject memoObj))
        {
            memoAnimator = memoObj.GetComponent<Animator>();
            if (!memoAnimator)
                Debug.LogError("UI_MenoにAnimatorコンポーネントを追加してください。");
        }
        else
            Debug.LogError("メモUIがDictiopnaryに登録されていません");

        // TimeManagerの取得
        timeMng = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if(!timeMng)
        {
            Debug.LogError("CanvasにTimeManagerがありません。");
        }

        // _FieldDataManagerの取得
        fieldDataMng = GameObject.Find("Field").GetComponent<_FieldDataManager>();
        if(!fieldDataMng)
        {
            Debug.LogError("_FieldDataManagerが見つかりません。");
        }

        UIDictionary.GetValueOrDefault(E_UI_KIND.performance).SetActive(false);
        UIDictionary.GetValueOrDefault(E_UI_KIND.backPanel).SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        InputUpdate();
        UpdateAnimator();
        UpdateTimeScale();
        ObjectFlagUpdate();

        // 過去フラグ状況の更新
        prevFlag = currentFlag;
    }


    /// <summary>
    /// アニメーションの更新
    /// </summary>
    void UpdateAnimator()
    {
        // TODO UIを追加したら随時ここに追加していく

        if (memoAnimator != null)
            memoAnimator.SetBool("isUseMemo", useMemo);
    }

    /// <summary>
    /// 入力ごとの更新
    /// </summary>
    void InputUpdate()
    {
        // 現在のフラグ状況を更新
        currentFlag = useMemo;


        // Tabキーでメモをポップアップする
        //if (Input.GetKeyDown(KeyCode.Tab))
        //    useMemo = !useMemo;
    }

    /// <summary>
    /// タイムスケール関連更新
    /// </summary>
    void UpdateTimeScale()
    {
        // ※AnimatorControllerの設定でUIアニメーションのTimeScaleは影響を受けない
        if (timeMng == null) return;

        // ここの条件はUIを開いている間、裏のゲーム自体を止めたい場合
        if(useMemo)
        {
            timeMng.SetTimeScale(0.0f);
        }
        else
        {
            // それ以外は裏で動いていても大丈夫
            timeMng.SetTimeScale(1.0f);
        }
    }

    /// <summary>
    /// 外部スクリプト用　すべてのUIのアクティブを操作する関数
    /// </summary>
    /// <param name="active"></param>
    public void SetAllUIActive(bool active)
    {
        // Dictionaryに登録されているUIすべてをactiveにする
        foreach(var kvp in UIDictionary)
        {
            if (kvp.Value != null)
                kvp.Value.SetActive(active);
        }
    }

    /// <summary>
    /// 外部スクリプト用　個別のUIのアクティブを操作する関数(useはそのUIを開くかどうか)
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="active"></param>
    public void SetUIActive(E_UI_KIND kind,bool active,bool use = false)
    {
        if(UIDictionary.TryGetValue(kind,out GameObject obj) && obj != null)
        {
            obj.SetActive(active);
        }
        switch(kind)
        {
            case E_UI_KIND.memo:
                useMemo = use;
                break;
            case E_UI_KIND.timeIcon:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// オブジェクトのフラグを更新する
    /// </summary>
    void ObjectFlagUpdate()
    {
        if(timeMng.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.morning ||
           timeMng.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.night)
        {
            UIDictionary.GetValueOrDefault(E_UI_KIND.memo).SetActive(false);
        }
        else
            UIDictionary.GetValueOrDefault(E_UI_KIND.memo).SetActive(true);
    }

    public void OnEnable()
    {
		if (switchActionRef != null && switchActionRef.action != null)
		{
			switchAction = switchActionRef.action;
			switchAction.performed += OnSwitchPerformed;
			switchAction.Enable();
		}
		else
		{
			Debug.LogError("switchActionRefが未設定、またはactionがnullです。");
		}
	}


    private void OnSwitchPerformed(InputAction.CallbackContext context)
    {
        if(timeMng.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.noon || 
           timeMng.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.afternoon)
        {
            // メモをポップアップする
            useMemo = !useMemo;
        }
    }
}
