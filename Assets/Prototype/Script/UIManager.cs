using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIを管理するクラス
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("各UIオブジェクト")]
    [SerializeField] private GameObject memoUIObj;

    /// <summary>
    /// UIの種類の列挙体
    /// </summary>
    public enum E_UI_KIND
    {
        memo,       // メモ
        timeIcon,   // 時間アイコン
    }


    private Animator memoAnimator;  // メモUIアニメーター
    private TimeManager timeMng;

    private bool useMemo;       // メモを開いているかどうか
    private bool currentFlag;   // 現在のフラグ状況
    private bool prevFlag;      // 1フレーム前のフラグ状況

    /// <summary>
    /// 初期化
    /// </summary>
    private void Start()
    {
        // 各オブジェクトからAnimatorを取得＆nullチェック
        memoAnimator = memoUIObj.GetComponent<Animator>();
        if(!memoAnimator)
        {
            Debug.LogError("UI_MenoにAnimatorコンポーネントを追加してください。");
        }

        // TimeManagerの取得
        timeMng = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if(!timeMng)
        {
            Debug.LogError("CanvasにTimeManagerがありません。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputUpdate();
        UpdateAnimator();
        UpdateTimeScale();

        // 過去フラグ状況の更新
        prevFlag = currentFlag;
    }


    /// <summary>
    /// アニメーションの更新
    /// </summary>
    void UpdateAnimator()
    {
        // TODO UIを追加したら随時ここに追加していく
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
        if (Input.GetKeyDown(KeyCode.Tab))
            useMemo = !useMemo;
    }

    /// <summary>
    /// タイムスケール関連更新
    /// </summary>
    void UpdateTimeScale()
    {
        // ※AnimatorControllerの設定でUIアニメーションのTimeScaleは影響を受けない


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
        // TODO オブジェクトを追加したら随時書き足す

        memoUIObj.SetActive(active);
    }

    /// <summary>
    /// 外部スクリプト用　個別のUIのアクティブを操作する関数(useはそのUIを開くかどうか)
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="active"></param>
    public void SetUIActive(E_UI_KIND kind,bool active,bool use = false)
    {
        switch(kind)
        {
            case E_UI_KIND.memo:
                memoUIObj.SetActive(active);
                useMemo = use;
                break;
            case E_UI_KIND.timeIcon:
                break;
            default:
                break;
        }
    }
}
