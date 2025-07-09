using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// 監視カメラの索敵範囲を管理するスクリプト
/// カメラの向き・索敵状態を切り替え、視野内のマスの状態や色を変更する
/// </summary>
public class SurveillanceCamera : MonoBehaviour
{
    public enum CameraDirection
    {
        Left = -1,  // 左にずらした際に左に回転させたいから-1スタート
        Center,
        Right,
    }

    [Header("監視する範囲")]
    [SerializeField] private Vector2Int searchRange;

    [Header("お嬢様を見つけた際に警備員に通知する範囲")]
    [SerializeField] private Vector2Int notificationRange;

    [Header("方向をずらした際の回転角度")]
    [SerializeField, Range(0, 90)] private float rotateAngle;


    private _FieldDataManager   fieldData;      // _FieldDataManager
    private CameraDirection     direction;      // 現在の方向
    private Vector2Int          myPos;          // 自身のマス
    private Vector2Int          fieldSize;      // フィールドサイズ
    private List<Vector2Int>    searchList;     // 監視座標リスト
    private Vector2             cameraForward;  // カメラの進行方向
    private Vector2Int          cameraDir;      // カメラの向き
    private TimeManager         timeManager;    // TimeManager
    private VisualEffect        vfx;            // VisualEffectコンポーネント



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeManager = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if (!timeManager)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "timeManagerがnullです"
            );
        }

        fieldData = GameObject.Find("Field").GetComponentInChildren<_FieldDataManager>();
        if (!fieldData)
        {
            Debug.LogError(
                "Script:LightObject.cs \n" +
                "fieldがnullです"
            );
        }

        vfx = GetComponentInChildren<VisualEffect>();
        if (!vfx)
        {
            Debug.LogError(
                "Script:SurveillanceCamera.cs \n" +
                "VisualEffectコンポーネントが見つかりません"
            );
        }
        else 
        {
            vfx.Stop(); // 初期状態ではVFXを停止
        }

        direction = CameraDirection.Center;
        myPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        fieldSize = fieldData.GetFieldSize();
        searchList = new List<Vector2Int>();
        cameraForward = new Vector2(transform.forward.x, transform.forward.z).normalized;

        // 向いている方向
        if (Mathf.Abs(cameraForward.x) > Mathf.Abs(cameraForward.y))
        {
            cameraDir = cameraForward.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            cameraDir = cameraForward.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

        // 初期位置の監視範囲を計算
        CalcSearchRange();
    }

    // Update is called once per frame
    void Update()
    {
        // 夜以外は終了
        if (timeManager.GetCurState() != CommonSE_Proto.E_TIMEOFDAY.night)
        {
            return;
        }

        searchCamera();
    }

    public void Action(Transform playerTransform)
    {
        // プレイヤーがライトに対してどの位置にいるか計算
        Vector2 toPlayer = new Vector2(playerTransform.position.x - this.transform.position.x, playerTransform.position.z - this.transform.position.z).normalized;

        // 内積の計算により、ライトの向きに対しての位置関係を計算
        float dot = Vector2.Dot(cameraForward, toPlayer);

        // しきい値で横にいても0.0fにならない場合に対応
        if (Mathf.Abs(dot) < 0.01)
        {
            dot = 0.0f;
        }

        // 外積の計算を用いてライトに対して左右どちらにいるか判定
        float cross = cameraForward.x * toPlayer.y - cameraForward.y * toPlayer.x;

        // 横にいる場合にのみ処理を行う
        if (dot == 0)
        {
            if (cross > 0) // ライトの左側
            {
                ChangeDirection(CameraDirection.Right);
            }
            else if (cross < 0) // ライトの右側
            {
                ChangeDirection(CameraDirection.Left);
            }
        }
    }
    public void ChangeDirection(CameraDirection changeDirection)
    {
        // 前の方向とずらしたい方向を足して変更可能な値か確認
        CameraDirection newDirection = (int)direction + changeDirection;
        if (Mathf.Abs((int)newDirection) > 1)
        {
            return;
        }

        // 方向を変更する場合はVFXを再生
        vfx.Play();

        // 角度を更新
        Vector3 currentAngle = transform.eulerAngles;

        // 向きを更新
        direction = newDirection;

        // 新しい方向を適用
        currentAngle.y += (int)changeDirection * rotateAngle;
        transform.eulerAngles = currentAngle;

        // 影の位置を再計算
        CalcSearchRange();
    }

    private void CalcSearchRange()
    {
        // リストに要素があれば削除処理を実行
        if (searchList.Count > 0)
        {
            searchList.Clear();
        }

        // 方向に応じて加算する値を変更する
        Vector2Int lateralDir = new Vector2Int();
        switch (direction)
        {
            case CameraDirection.Left:
                lateralDir = new Vector2Int(-cameraDir.y, cameraDir.x);
                break;
            case CameraDirection.Right:
                lateralDir = new Vector2Int(cameraDir.y, -cameraDir.x);
                break;
            case CameraDirection.Center:
                lateralDir = new Vector2Int(-cameraDir.y, cameraDir.x);
                break;
        }

        // 向いている方向に応じた影の生成処理
        if (direction == CameraDirection.Center)
        {
            for (int i = -1; i < searchRange.x - 1; ++i)
            {
                for (int j = 0; j < searchRange.y; ++j)
                {
                    // 対象マスの座標を計算
                    Vector2Int targetPos = myPos + cameraDir * (j + 1) + lateralDir * i;

                    // 範囲外チェック
                    if (targetPos.x < 0 || targetPos.x >= fieldSize.x || targetPos.y < 0 || targetPos.y >= fieldSize.y)
                    {
                        continue;
                    }

                    // ターゲット座標をVector3に変換
                    Vector3 target = new Vector3(targetPos.x, 0.0f, targetPos.y);

                    // 自身から目標座標へ向かう方向を計算
                    Vector3 dirToTarget = (target - this.transform.position).normalized;

                    // 自身から目標座標への距離を計算
                    float distance = Vector3.Distance(this.transform.position, target);

                    // レイに対する全ての衝突を検知
                    RaycastHit[] hits = Physics.RaycastAll(this.transform.position, dirToTarget, distance);

                    // このマスをスキップするか
                    bool isSkipPos = false;

                    // 目標座標に向かってレイを飛ばし、障害物がないか確認する
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.CompareTag("Obstacles"))
                        {
                            isSkipPos = true;
                            break;
                        }
                    }

                    if (isSkipPos)
                    {
                        continue;
                    }

                    searchList.Add(targetPos);
                }
            }
        }
        else
        {
            for (int i = 0; i < searchRange.x; ++i)
            {
                for (int j = 0; j < searchRange.y; ++j)
                {
                    // 対象マスの座標を計算
                    Vector2Int targetPos = myPos + cameraDir * (j + 1) + lateralDir * i;

                    // 範囲外チェック
                    if (targetPos.x < 0 || targetPos.x >= fieldSize.x || targetPos.y < 0 || targetPos.y >= fieldSize.y)
                    {
                        continue;
                    }

                    // ターゲット座標をVector3に変換
                    Vector3 target = new Vector3(targetPos.x, 0.0f, targetPos.y);

                    // 自身から目標座標へ向かう方向を計算
                    Vector3 dirToTarget = (target - this.transform.position).normalized;

                    // 自身から目標座標への距離を計算
                    float distance = Vector3.Distance(this.transform.position, target);

                    // レイに対する全ての衝突を検知
                    RaycastHit[] hits = Physics.RaycastAll(this.transform.position, dirToTarget, distance);

                    // このマスをスキップするか
                    bool isSkipPos = false;

                    // 目標座標に向かってレイを飛ばし、障害物がないか確認する
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.CompareTag("Obstacles"))
                        {
                            isSkipPos = true;
                            break;
                        }
                    }

                    if (isSkipPos)
                    {
                        continue;
                    }

                    searchList.Add(targetPos);
                }
            }
        }

        if (searchList.Count > 0)
        {
            // 監視の範囲を監視範囲表示スクリプトに送信
            this.GetComponent<MonitoringRangeGanerate>().SetArrayShadow(searchList);
        }
    }

    private void searchCamera()
    {
        var asas = fieldData.GetGameObjectList(_FieldDataManager.E_FIELDSTATE.princess);


        foreach (var pos in searchList)
        {
            // 監視座標にお嬢様がいるか確認
            var objList = fieldData.GetInfoList(pos);

            foreach(var obj in objList)
            {
                // オブジェクトがプリンセスか確認
                if(obj.state == _FieldDataManager.E_FIELDSTATE.princess)
                {
                    WarningVolumeController.Instance.NotifyCameraDetection();
                    
                    // 警備員リスト
                    var securities = fieldData.GetGameObjectList(_FieldDataManager.E_FIELDSTATE.securityGuard_N);

                    // 各々通知範囲にいるか確認
                    foreach(var security in securities)
                    {
                        // フィールド上の警備員の座標
                        Vector2Int securityPos = security.GetComponent<CharacterMoveController>().GetCurrentPos();
    
                        // 警備員が通知範囲にいるか確認
                        if( securityPos.x > myPos.x - notificationRange.x / 2 && 
                            securityPos.x < myPos.x + notificationRange.x / 2 &&
                            securityPos.y > myPos.y - notificationRange.y / 2 &&
                            securityPos.y < myPos.y + notificationRange.y / 2 )
                        {
                            SecurityController controller = security.GetComponent<SecurityController>();

                            if (!controller.GetIsFoundPrincess())
                            {
                                controller.FoundPrincess(pos);
                            }
                        }
                    }
                }
            }
        }
    }
} 