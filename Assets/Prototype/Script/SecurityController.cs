using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

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
        if (timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.night)
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

        // 加算する方向を計算
        Vector2Int lateralDir = new Vector2Int();
        if (charaDir.x != 0)
        {
            lateralDir = new Vector2Int(0, 1);
        }
        else
        {
            lateralDir = new Vector2Int(1, 0);
        }

        // 現在の座標
        Vector2Int pos = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.z);

        // 遮るオブジェクトがあったら終了させるフラグ
        bool isEndCheck = false;

        for (int j = 0; j < surroundingsRange.y; ++j)
        {
            // 確認終了フラグを下げる
            isEndCheck = false;

            // 警備員に対して横方向に確認
            for (int i = 1; i <= (surroundingsRange.x / 2); ++i)
            {
                // 確認終了フラグがあったら終了
                if (isEndCheck)
                {
                    break;
                }

                // 対象マスの座標を計算
                Vector2Int targetPos = pos + charaDir * (j + 1) + lateralDir * i;

                // 範囲外チェック
                if (targetPos.x < 0 || targetPos.x >= fieldSize.x || targetPos.y < 0 || targetPos.y >= fieldSize.y)
                {
                    continue;
                }

                // マスの情報を取得
                var infoArray = fieldData.GetInfoList(targetPos);
                foreach (var info in infoArray)
                {
                    // 障害物があったら以降を確認しない
                    if (info.state == _FieldDataManager.E_FIELDSTATE.pillar ||
                        info.state == _FieldDataManager.E_FIELDSTATE.wall)
                    {
                        // 確認終了フラグを立てる
                        isEndCheck = true;
                        break;
                    }

                    if (info.state == _FieldDataManager.E_FIELDSTATE.princess)
                    {
                        // ヒット情報を格納する変数
                        RaycastHit hit;

                        // 相手の座標
                        Vector3 otherPos = new Vector3(info.pos.x, 0.0f, info.pos.y);

                        // レイ生成する方向を計算
                        Vector3 rayDirection = (otherPos - transform.position).normalized;

                        // レイの長さを計算
                        float rayDistance = Vector3.Distance(transform.position, otherPos);

                        // 警備員とお嬢様の間にレイを生成し障害物がないか確認
                        Ray ray = new Ray(transform.position, rayDirection);
                        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);

                        // レイキャストを実行
                        if (Physics.Raycast(transform.position, rayDirection, out hit, rayDistance))
                        {
                            // ヒットしたオブジェクトのタグで判定
                            if (hit.collider.CompareTag("Princess"))
                            {
                                // お姫様を発見
                                securityEffect.StartDoubleAlertEffect(); // エフェクトを再生
                                uiManager.SetUIActive(UIManager.E_UI_KIND.gameOver, true);
                                SceneChanger.ChangeScene(sceneName);
                            }
                        }
                    }
                }
            }

            // 確認終了フラグを下げる
            isEndCheck = false;

            // 警備員に対して横方向に確認
            for (int i = -1; i >= -(surroundingsRange.x / 2); --i)
            {
                // 確認終了フラグがあったら終了
                if (isEndCheck)
                {
                    break;
                }

                // 対象マスの座標を計算
                Vector2Int targetPos = pos + charaDir * (j + 1) + lateralDir * i;

                // 範囲外チェック
                if (targetPos.x < 0 || targetPos.x >= fieldSize.x || targetPos.y < 0 || targetPos.y >= fieldSize.y)
                {
                    continue;
                }

                // マスの情報を取得
                var infoArray = fieldData.GetInfoList(targetPos);
                foreach (var info in infoArray)
                {
                    // 障害物があったら以降を確認しない
                    if (info.state == _FieldDataManager.E_FIELDSTATE.pillar ||
                        info.state == _FieldDataManager.E_FIELDSTATE.wall)
                    {
                        // 確認終了フラグを立てる
                        isEndCheck = true;
                        break;
                    }

                    if (info.state == _FieldDataManager.E_FIELDSTATE.princess)
                    {
                        // ヒット情報を格納する変数
                        RaycastHit hit;

                        // 相手の座標
                        Vector3 otherPos = new Vector3(info.pos.x, 0.0f, info.pos.y);

                        // レイ生成する方向を計算
                        Vector3 rayDirection = (otherPos - transform.position).normalized;

                        // レイの長さを計算
                        float rayDistance = Vector3.Distance(transform.position, otherPos);

                        // 警備員とお嬢様の間にレイを生成し障害物がないか確認
                        Ray ray = new Ray(transform.position, rayDirection);
                        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);

                        // レイキャストを実行
                        if (Physics.Raycast(transform.position, rayDirection, out hit, rayDistance))
                        {
                            // ヒットしたオブジェクトのタグで判定
                            if (hit.collider.CompareTag("Princess"))
                            {
                                // お姫様を発見
                                securityEffect.StartDoubleAlertEffect(); // エフェクトを再生
                                uiManager.SetUIActive(UIManager.E_UI_KIND.gameOver, true);
                                SceneChanger.ChangeScene(sceneName);
                            }
                        }
                    }
                }
            }
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

                // お姫様を発見
                securityEffect.StartDoubleAlertEffect(); // エフェクトを再生
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