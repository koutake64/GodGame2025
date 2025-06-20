using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;


public class SecurityController : MonoBehaviour
{
    [Header("前方監視範囲")]
    [SerializeField] private int monitoringRange;

    [Header("周囲の監視範囲")]
    [SerializeField] private Vector2Int surroundingsRange;

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
    Vector2                         charaForward;           // 進行方向
    Vector2Int                      charaDir;               // マス目上で向いてる方向

    private Vector2Int              initPos;        // 初期位置
    private TimeManager             timeManager;    // タイムマネージャー
    private UIManager               uiManager;      // UIマネージャー
    private SecurityEffect          securityEffect; // セキュリティエフェクト
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

        securityEffect = GetComponentInChildren<SecurityEffect>();
        if(!securityEffect)
        {
            Debug.LogError(
               "Script:SecurityController.cs \n" +
               "securityEffectがnullです"
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

        // 足音を鳴らすか判定
        JudgeWalkSound();

        // 移動が終了していたら
        if (isEndMovement && targetArray.Count > 0)
        {
            // 移動先を指定
            moveController.StartAutoMove(targetArray[currentIndex]);

            // 移動終了フラグを下げる
            isEndMovement = false;
        }

        // 夜以外はスルー
        if (timeManager.GetCurState() != CommonSE_Proto.E_TIMEOFDAY.night)
        {
            // 進行方向に対してチェックを行う
            ForwardMonitoring();

            // 周囲に対して監視を行う
            SurroundingsMonitoring();
        }
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

    private void SurroundingsMonitoring()
    {
        // 向いている方向
        charaForward = new Vector2(transform.forward.x, transform.forward.z).normalized;
        if (Mathf.Abs(charaForward.x) > Mathf.Abs(charaForward.y))
        {
            charaDir = charaForward.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            charaDir = charaForward.y > 0 ? Vector2Int.up : Vector2Int.down;
        }



    }

    private void ForwardMonitoring()
    {
        // デバッグ用
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * monitoringRange, Color.green);

        // ヒット情報を格納する変数
        RaycastHit hit;

        // レイの方向と距離
        Vector3 forward = transform.forward.normalized;

        // レイキャストを実行
        if (Physics.Raycast(transform.position, forward, out hit, monitoringRange))
        {
            // ヒットしたオブジェクトのタグで判定
            if (hit.collider.CompareTag("Princess"))
            {
                // レイがヒットした相手が影のマスにいないか確認
                Vector2Int hitPos = new Vector2Int((int)hit.collider.transform.position.x, (int)hit.collider.transform.position.z);
                var objList = fieldData.GetInfoList(hitPos);
                foreach (var obj in objList)
                {
                    // 影があったら終了
                    if(obj.state == _FieldDataManager.E_FIELDSTATE.shadow)
                    {
                        return;
                    }
                }

                securityEffect.StartDoubleAlertEffect(); // エフェクトを再生

                // お姫様を発見
                uiManager.SetUIActive(UIManager.E_UI_KIND.gameOver, true);
                SceneChanger.ChangeScene(sceneName);
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

    private void JudgeWalkSound()
    {
        // 一度足音を鳴らさなくする
        isFootstepsRange = false;

        // 対象の座標を格納する用
        List<Vector2Int> targetPos = new List<Vector2Int>();

        // 対象の座標を取得
        if (timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.morning || timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.night)
        {
           targetPos = fieldData.GetStatePos(_FieldDataManager.E_FIELDSTATE.princess);
        }
        else if(timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.noon)
        {
            targetPos = fieldData.GetStatePos(_FieldDataManager.E_FIELDSTATE.butler);
        }

        // 探索範囲
        int radius = monitoringRange / 2;

        // 自身の座標
        Vector2Int myPos = moveController.GetCurrentPos();

        foreach (Vector2Int pos in targetPos)
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
    }
}