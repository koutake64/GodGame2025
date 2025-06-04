using UnityEngine;
using System.Collections.Generic;


public class SecurityController : MonoBehaviour
{
    [Header("前方監視範囲")]
    [SerializeField] private int monitoringRange;

    [Header("足音が聞こえる範囲")]
    [SerializeField] private int footstepsRange;

    [SerializeField, SceneSelector] private string sceneName;

    private List<Vector2Int>        targetArray = new List<Vector2Int>();
    private CharacterMoveController moveController;         // CharacterMoveController
    private _FieldDataManager       fieldData;              // _FieldDataManager
    private bool                    isEndMovement;          // 目標座標までの移動終了したか
    private int                     currentIndex;           // 配列の何番目か
    private int                     addNum;                 // 加算する値
    private Vector2Int              fieldSize;              // フィールドサイズ
    private bool                    isFoundPrincess;        // お嬢様見つけたフラグ
    private Vector2Int              foundPos;               // お嬢様を見つけた座標
    private bool                    isStartMoveFoundPos;    // お嬢様を見つけた座標に移動を開始しているか
    private bool                    isFootstepsRange;       // 足音の聞こえる範囲にいるか

    private Vector2Int              initPos;        // 初期位置
    private TimeManager             timeManager;    // タイムマネージャー
    private UIManager               uiManager;      // UIマネージャー
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveController = GetComponent<CharacterMoveController>();
        if (!moveController)
        {
            Debug.LogError(
               "Script:SecurityController.cs \n" +
               "moveControllerがnullです"
            );
        }

        fieldData = GameObject.Find("Field").GetComponent<_FieldDataManager>();
        if (!fieldData)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "fieldDataがnullです"
            );
        }

        timeManager = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if (!timeManager)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "timeManagerがnullです"
            );
        }

        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        if(!uiManager)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "uiManagerがnullです"
            );
        }

        isEndMovement = true;
        currentIndex = 0;
        addNum = 1;
        fieldSize = fieldData.GetFieldSize();
        isFoundPrincess = false;
        isStartMoveFoundPos = false;
        isFootstepsRange = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 夜になったら初期位置に戻る
        if (timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.night && timeManager.IsChangeState() || 
            timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.noon && timeManager.IsChangeState())
        {
            transform.position = new Vector3(initPos.x, 0.0f, initPos.y);
        }

        // 一度足音を鳴らさなくする
        isFootstepsRange = false;

        // 対象の座標を取得
        List<Vector2Int> princess = fieldData.GetStatePos(_FieldDataManager.E_FIELDSTATE.princess);
       
        // 探索範囲
        int radius = monitoringRange / 2;

        // 自身の座標
        Vector2Int myPos = moveController.GetCurrentPos();

        foreach (Vector2Int pos in princess)
        {
            // 範囲内かどうかをチェック
            if (Mathf.Abs(pos.x - myPos.x) <= radius && Mathf.Abs(pos.y - myPos.y) <= radius)
            {
                // 足音フラグを立てる
                isFootstepsRange = true;

                // 1人見つけたら終了
                break;
            }
        }

        // 移動が終了していたら
        if (isEndMovement && targetArray.Count > 0)
        {
            // 移動先を指定
            moveController.StartAutoMove(targetArray[currentIndex]);

            // 移動終了フラグを下げる
            isEndMovement = false;
        }

        // 進行方向に対してチェックを行う
        ForwardMonitoring();
    }

    public void EndMovement()
    {
        if (isStartMoveFoundPos)
        {
            isStartMoveFoundPos = false;
        }

        if (targetArray.Count == 0)
        {
            Debug.Log("巡回ルートがありません");
            return;
        }

        // 巡回するように配列番号を更新
        currentIndex = (currentIndex + addNum) % targetArray.Count;

        if(currentIndex < 0)
        {
            currentIndex = targetArray.Count - 1;
        }

        isEndMovement = true;
    }

    public void InverseArray()
    {
        addNum *= -1;
    }
    private void ForwardMonitoring()
    {
        // 夜以外はスルー
        if (timeManager.GetCurState() != CommonSE_Proto.E_TIMEOFDAY.night)
        {
            return;
        }

        // 向いている方向
        Vector3 forward = transform.forward.normalized;
        Vector2Int direction;
        if(Mathf.Abs(forward.x) > Mathf.Abs(forward.z))
        {
            direction = forward.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            direction = forward.z > 0 ? Vector2Int.up : Vector2Int.down;
        }

        // 監視
        Vector2Int currentPos = moveController.GetCurrentPos();
        for(int i = 0; i < monitoringRange; ++i)
        {
            // マス目の情報取得
            Vector2Int pos = currentPos + direction * (i + 1);

            // 範囲外判定
            if(pos.x < 0 || pos.x >= fieldSize.x || pos.y < 0 || pos.y >= fieldSize.y)
            {
                continue;
            }

            var info = fieldData.GetInfoList(pos);
            for(int j = 0; j < info.Count; ++j)
            {
                // 影の場合次へ
                if (info[j].state == _FieldDataManager.E_FIELDSTATE.shadow)
                {
                    continue;
                }

                // お姫様を発見
                if (info[j].state == _FieldDataManager.E_FIELDSTATE.princess)
                {
                    // ゲームオーバーのUIを表示
                    uiManager.SetUIActive(UIManager.E_UI_KIND.gameOver, true);
                    SceneChanger.ChangeScene(sceneName);
                }

                // 貫通しないオブジェクトの場合
                if (info[j].state == _FieldDataManager.E_FIELDSTATE.wall || 
                    info[j].state == _FieldDataManager.E_FIELDSTATE.pillar ||
                    info[j].state == _FieldDataManager.E_FIELDSTATE.exhibitionStand)
                {
                    break;
                }
            }
        }
    }

    public void FoundPrincess(Vector2Int targetPos)
    {
        isFoundPrincess = true;
        foundPos = targetPos;
        currentIndex += addNum * -1;
        isEndMovement = false;
    }

    public bool GetIsFoundPrincess()
    {
        return isFoundPrincess;
    }

    public void SetIsFoundPrincess(bool flg)
    {
        isFoundPrincess = flg;
    }

    public void AddTargetPos(Vector2Int pos)
    {
        targetArray.Add(pos);
    }

    public void SetInitPos(Vector2Int pos)
    {
        initPos = pos;
    }

    public bool StartMoveFoundPos()
    {
        if(!isFoundPrincess)
        {
            return false;
        }
        if(isStartMoveFoundPos)
        {
            return false;
        }

        moveController.StartAutoMove(foundPos);
        isEndMovement = false;
        isStartMoveFoundPos = true;

        return true;
    }

    public bool GetIsFoodStepsFlg()
    {
        return isFootstepsRange;
    }
}