using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIを管理するクラス
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("各UIオブジェクト")]
    [SerializeField] private GameObject memoUIObj;


    private Animator memoAnimator;  // メモUIアニメーター

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
    }

    // Update is called once per frame
    void Update()
    {
        InputUpdate();
        UpdateAnimator();

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

        }
        else
        {
            // それ以外は裏で動いていても大丈夫
        }
    }
}
