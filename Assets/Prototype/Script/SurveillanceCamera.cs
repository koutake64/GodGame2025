using NUnit.Framework.Internal.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static CommonSE_Proto;

/// <summary>
/// 監視カメラの索敵範囲を管理するスクリプト
/// カメラの向き・索敵状態を切り替え、視野内のマスの状態や色を変更する
/// </summary>
public class SurveillanceCamera : MonoBehaviour
{
    // 監視状態の列挙型（左・中央・右）
    private enum E_WATCHSTATE
    {
        Left,
        Center,
        Right,
    }

    [Header("警備員の呼び出し範囲")]
    [SerializeField] int callRange = 0;

    // 現在の監視状態（初期は中央）
    private E_WATCHSTATE watchState = E_WATCHSTATE.Center;

    // フィールド情報を管理するクラス
    private _FieldDataManager fieldDataManager;

    // カメラの位置（マス座標）
    private Vector2Int SurveillanceCameraPos = new Vector2Int();


    // カメラの正面方向（初期は上方向）
    private Vector2 forward = Vector2.up;

    //プレイヤーの座標
    Vector3 playerPos = new Vector3();

    private Vector3 princessPos = new Vector3();

    //カメラの向きの取得
    CommonSE_Proto.E_DIRECTION CameraDir;

    //カメラの周りにプレイヤーがいるか
    bool isPlayerInRange = false;

    // マスの状態取得用
    //FieldDataManager fieldManager;

    // お嬢様呼び出し処理用
    bool isFoundTarget = false;
    int frameCount = 0;
    List<SecurityController> callSecurityList = new List<SecurityController>();

    Vector2Int targetPos = new Vector2Int();

    // タイムマネージャー
    private TimeManager timeManager;
    
    //監視カメラの向き
    private float angleY = 0f;

    //レイ用
    public float rayLength = 6.0f;   // Rayの長さ（6.0f）
    public int rayCount = 8;         // Rayの本数（例：6本で扇状）
    private void Start()
    {
        AudioManager.Instance.PlaySE(3);

        // フィールドマネージャーを取得
        fieldDataManager = GameObject.Find("Field").GetComponentInChildren<_FieldDataManager>();
        if (!fieldDataManager)
        {
            Debug.LogError(
                "Script:SurveillanceCamera.cs \n" +
                "fieldDataManagerがnullです"
            );
        }

        // タイムマネージャー取得
        timeManager = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if (!timeManager)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "timeManagerがnullです"
            );
        }


        SurveillanceCameraPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        //Debug.Log($"カメラ位置（マス座標）: ({SurveillanceCameraPos}");

        float yRotation = transform.eulerAngles.y;
        if (Mathf.Approximately(yRotation, 0f))
            forward = Vector2.up;
        else if (Mathf.Approximately(yRotation, 90f))
            forward = Vector2.right;
        else if (Mathf.Approximately(yRotation, 180f))
            forward = Vector2.down;
        else if (Mathf.Approximately(yRotation, 270f))
            forward = Vector2.left;
        else
            Debug.LogWarning($"想定外の角度です: {yRotation}");

        // 回転と索敵範囲の初期描画を実行
        RotateVisualObject();
        SearchRange();

        // 初期向きを記録しておく
       
    }
    void Update()
    {
        // プレイヤーの GameObject を使って座標を取得
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerPos = player.transform.position;
            //Debug.Log($"プレイヤーの位置 : ({playerPos}");
        }
        else
        {
            //Debug.LogError("プレイヤーが見つかりませんでした");
        }

        // プリンセスの GameObject を使って座標を取得
        GameObject princess = GameObject.FindWithTag("Princess");
        if (princess != null)
        {
            princessPos = princess.transform.position;
            //Debug.Log($"プレイヤーの位置 : ({princessPos}");
        }
        else
        {
            //Debug.LogError("プリンセスが見つかりませんでした");
        }


        Vector2Int playerGridPos = new Vector2Int((int)playerPos.x, (int)playerPos.z);

        int PlayerInRangeY = (playerGridPos.y - SurveillanceCameraPos.y);
        int PlayerInRangeX = (playerGridPos.x - SurveillanceCameraPos.x);
        isPlayerInRange = false; // 一度リセット

        if (forward == Vector2.up || forward == Vector2.down)
        {
            // 左右（X軸方向）1マス離れているならOK
            if ((PlayerInRangeX == 1 || PlayerInRangeX == -1) && PlayerInRangeY == 0)
            {
                isPlayerInRange = true;
                Debug.Log("カメラの向きが変えられる（左右）");
            }
        }
        else if (forward == Vector2.right || forward == Vector2.left)
        {
            // 上下（Y軸方向）1マス離れているならOK
            if ((PlayerInRangeY == 1 || PlayerInRangeY == -1) && PlayerInRangeX == 0)
            {
                isPlayerInRange = true;
                Debug.Log("カメラの向きが変えられる（上下）");
            }
        }
        //Debug.Log($"カメラとプレイヤーとの距離X" + (PlayerInRangeX));
        //Debug.Log($"カメラとプレイヤーとの距離Y" + (PlayerInRangeY));


        if (isPlayerInRange == true && Input.GetKeyDown(KeyCode.Return))
        {

            if (forward == Vector2.up)
                if (playerPos.x > this.transform.position.x) // プレイヤーがカメラの左側
                {
                    if (watchState == E_WATCHSTATE.Center)
                        watchState = E_WATCHSTATE.Right;
                    else if (watchState == E_WATCHSTATE.Left)
                        watchState = E_WATCHSTATE.Center;
                }
                else if (playerPos.x < this.transform.position.x) // プレイヤーがカメラの右側
                {
                    if (watchState == E_WATCHSTATE.Center)
                        watchState = E_WATCHSTATE.Left;
                    else if (watchState == E_WATCHSTATE.Right)
                        watchState = E_WATCHSTATE.Center;
                }

            if (forward == Vector2.down)
                if (playerPos.x < this.transform.position.x) // プレイヤーがカメラの左側
                {
                    if (watchState == E_WATCHSTATE.Center)
                        watchState = E_WATCHSTATE.Right;
                    else if (watchState == E_WATCHSTATE.Left)
                        watchState = E_WATCHSTATE.Center;
                }
                else if (playerPos.x > this.transform.position.x) // プレイヤーがカメラの右側
                {
                    if (watchState == E_WATCHSTATE.Center)
                        watchState = E_WATCHSTATE.Left;
                    else if (watchState == E_WATCHSTATE.Right)
                        watchState = E_WATCHSTATE.Center;
                }

            if (forward == Vector2.left)
                if (playerPos.z < this.transform.position.z) // プレイヤーがカメラの左側
                {
                    if (watchState == E_WATCHSTATE.Center)
                        watchState = E_WATCHSTATE.Right;
                    else if (watchState == E_WATCHSTATE.Left)
                        watchState = E_WATCHSTATE.Center;
                }
                else if (playerPos.z > this.transform.position.z) // プレイヤーがカメラの右側
                {
                    if (watchState == E_WATCHSTATE.Center)
                        watchState = E_WATCHSTATE.Left;
                    else if (watchState == E_WATCHSTATE.Right)
                        watchState = E_WATCHSTATE.Center;
                }

            if (forward == Vector2.right)
                if (playerPos.z > this.transform.position.z) // プレイヤーがカメラの左側
                {
                    if (watchState == E_WATCHSTATE.Center)
                        watchState = E_WATCHSTATE.Right;
                    else if (watchState == E_WATCHSTATE.Left)
                        watchState = E_WATCHSTATE.Center;
                }
                else if (playerPos.z < this.transform.position.z) // プレイヤーがカメラの右側
                {
                    if (watchState == E_WATCHSTATE.Center)
                        watchState = E_WATCHSTATE.Left;
                    else if (watchState == E_WATCHSTATE.Right)
                        watchState = E_WATCHSTATE.Center;
                }

           

            RotateVisualObject();
            //Debug.Log("→ 現在の監視状態：" + watchState + SurveillanceCameraPos);

            fieldDataManager.ChangeColor();

        }

        // 監視範囲の状態をリセット
        ResetCameraRange();

        // 現在の監視状態に応じて索敵処理を実行
        SearchRange();
    }

    /// <summary>
    /// カメラオブジェクトの見た目をforwardの向きに合わせて回転させる
    /// </summary>
    private void RotateVisualObject()
    {

        

        if (forward == Vector2.up)
        {
            angleY = 0f;
        }
        else if (forward == Vector2.right)
        {
            angleY = 90f;
        }
        else if (forward == Vector2.down)
        {
            angleY = 180f;
        }
        else if (forward == Vector2.left)
        {
            angleY = 270f;
        }

        this.transform.rotation = Quaternion.Euler(0f, angleY, 0f);
    }

    /// <summary>
    /// 現在の監視状態に応じて3×3の索敵範囲を設定し、色を変更する
    /// </summary>
    private void SearchRange()
    {
        int offsetValue = 0;
        isFoundTarget = false;
        frameCount++;

        // 状態に応じてスライド方向を決定
        if (forward == Vector2.right || forward == Vector2.left)
        {

            if (watchState == E_WATCHSTATE.Left)
            {
                offsetValue = 2;
                transform.rotation = Quaternion.Euler(0f, angleY - 45f, 0f);
            }
            else if (watchState == E_WATCHSTATE.Center)
            {
                offsetValue = 0;
                transform.rotation = Quaternion.Euler(0f, angleY, 0f);
            }
            else if (watchState == E_WATCHSTATE.Right)
            {
                offsetValue = -2;
                transform.rotation = Quaternion.Euler(0f, angleY + 45f, 0f);
            }
        }
        if (forward == Vector2.up || forward == Vector2.down)
        {

            if (watchState == E_WATCHSTATE.Left)
            {
                offsetValue = 2;
                transform.rotation = Quaternion.Euler(0f, angleY + 45f, 0f);
            }
            else if (watchState == E_WATCHSTATE.Center)
            {
                offsetValue = 0;
                transform.rotation = Quaternion.Euler(0f, angleY, 0f);
            }
            else if (watchState == E_WATCHSTATE.Right)
            {
                offsetValue = -2;
                transform.rotation = Quaternion.Euler(0f, angleY - 45f, 0f);
            }
        }
        // スライド方向を現在の向きに回転
        Vector2 slideDir = RotateOffset(new Vector2(offsetValue, 0), forward);

        // 索敵範囲の中心位置を計算（カメラの2マス先＋スライド方向）
        Vector2 center = SurveillanceCameraPos + forward * 2 + slideDir;

        Vector2Int max = fieldDataManager.GetFieldSize();

        // お嬢様の座標
        Vector2Int princessGridPos = new Vector2Int((int)princessPos.x, (int)princessPos.z);


        float halfAngle = 90f; // 左右90°ずつ（合計180°）
        float startAngle = -halfAngle; // 左端の角度（-90°）

        float angleStep = (halfAngle * 2) / (rayCount - 1); // Ray間の角度差

        for (int i = 0; i < rayCount; i++)
        {
            // 各Rayの発射角度
            float angle = startAngle + i * angleStep;

            // 角度分だけ前方をY軸で回転 → 飛ばす方向
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            // Ray作成
            Ray ray = new Ray(transform.position, direction);

            // シーンビュー上にRayを描画
            Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

            // RaycastAll を使用してすべてのヒットを取得
            RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, rayLength);

            // ヒット順にソート（近い順）
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            RaycastHit hit;

            // Rayが何かに当たったかチェック（距離制限付き）
            // Rayが最初に当たった物体を調べる
            if (Physics.Raycast(ray.origin, ray.direction * rayLength, out hit))
            {
                if (hit.collider.gameObject == this.gameObject)
                    continue; // 自分自身は無視

                // ヒットポイントをグリッド座標に変換
                Vector2Int hitPos = new Vector2Int((int)hit.point.x, (int)hit.point.z);

                bool inSightRange = false;

                // 3×3の範囲を走査して、ヒットポイントが含まれているかチェック
                for (int dx = -1; dx <= 1 && !inSightRange; dx++)
                {
                    for (int dy = -1; dy <= 1 && !inSightRange; dy++)
                    {
                        Vector2 offset = new Vector2(dx, dy);
                        Vector2 rotatedOffset = RotateOffset(offset, forward);
                        Vector2Int checkPos = Vector2Int.RoundToInt(center + rotatedOffset);

                        int tx = checkPos.x;
                        int ty = checkPos.y;

                        // 範囲外は無視
                        if (tx < 0 || tx >= max.x || ty < 0 || ty >= max.y)
                            continue;

                        // ヒットポイントが範囲内にあればフラグを立ててループ終了
                        if (hitPos == checkPos)
                        {
                            inSightRange = true;
                            break;
                        }
                    }
                }

                if (!inSightRange)
                    continue; // 索敵範囲外なら無視
                if (timeManager.GetCurState() == E_TIMEOFDAY.noon && timeManager.GetCurState() == E_TIMEOFDAY.afternoon)
                {
                    // 索敵範囲内だった場合の処理
                    if (hit.collider.CompareTag("Player"))
                    {
                        /// ※ここに執事がアクションした場合に警備員が向かう処理を書く
                        Debug.Log($"執事発見！: ({hitPos}) - {hit.collider.gameObject.name}");
                    }
                }
                if (timeManager.GetCurState() == E_TIMEOFDAY.night)
                {
                    // 索敵範囲内だった場合の処理
                    if (hit.collider.CompareTag("Princess"))
                    {
                        AudioManager.Instance.PlaySE(5);
                        Debug.Log($"プリンセス発見！: ({hitPos}) - {hit.collider.gameObject.name}");
                        isFoundTarget = true;
                        frameCount = 0;
                        targetPos = hit.collider.GetComponent<CharacterMoveController>().GetCurrentPos();
                    }
                    else if (hit.collider.CompareTag("Player"))
                    {
                        Debug.Log($"執事発見！: ({hitPos}) - {hit.collider.gameObject.name}");
                    }
                }
            }
        }

        if (isFoundTarget)
        {
            // 呼び出し通知オブジェクトリスト
            List<SecurityController> securityObj = new List<SecurityController>();

            // 自身の座標
            Vector2Int cameraPos = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.z);

            // 呼び始めの座標用
            Vector2Int callStart = new Vector2Int(cameraPos.x - callRange / 2, cameraPos.y - callRange / 2);

            // 通知範囲内にいる警備員を取得
            for (int y = 0; y < callRange; ++y)
            {
                for (int x = 0; x < callRange; ++x)
                {
                    // 範囲内のリスト取得
                    Vector2Int callPos = new Vector2Int(callStart.x + x, callStart.y + y);

                    // 範囲外確認
                    if (callPos.x < 0 || callPos.x >= max.x || callPos.y < 0 || callPos.y >= max.y)
                        continue;

                    // 対象座標のリスト取得
                    var list = fieldDataManager.GetInfoList(callPos);

                    if (list.Count != 0)
                    {
                        // リストの中に警備員がいたら取得
                        foreach (var obj in list)
                        {
                            if (!obj.obj) continue;
                            var security = obj.obj.GetComponent<SecurityController>();
                            if (!security) continue;

                            // リストに追加
                            securityObj.Add(security);
                        }
                    }
                    if (securityObj.Count != 0)
                    {
                        foreach (var obj in securityObj)
                        {
                            // すでに通知済みなら次へ
                            if (obj.GetIsFoundPrincess())
                                continue;

                            // 呼び出しオブジェクトリストに追加
                            callSecurityList.Add(obj);

                            // ターゲット座標を通知
                            obj.FoundPrincess(targetPos);

                            frameCount = 0;
                        }
                    }
                }
            }
        }

        if(!isFoundTarget && frameCount > 120 && callSecurityList.Count != 0)
        {
            foreach(var security in callSecurityList)
            {
                if(security.GetIsFoundPrincess())
                {
                    security.SetIsFoundPrincess(false);
                }
            }

            // 配列を初期化
            callSecurityList.Clear();
        }
    }

    /// <summary>
    /// 前フレームに設定されたカメラの索敵範囲をリセット
    /// </summary>
    private void ResetCameraRange()
    {
        Vector2Int max = fieldDataManager.GetFieldSize();

        for (int x = 0; x < max.x; x++)
        {
            for (int y = 0; y < max.y; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

            }
        }
    }

    /// <summary>
    /// オフセットベクトルをforward方向に応じて回転させる
    /// </summary>
    private Vector2 RotateOffset(Vector2 offset, Vector2 forward)
    {
        if (forward == Vector2.up)
        {
            return offset;
        }
        else if (forward == Vector2.right)
        {
            return new Vector2(-offset.y, offset.x);
        }
        else if (forward == Vector2.down)
        {
            return new Vector2(-offset.x, -offset.y);
        }
        else if (forward == Vector2.left)
        {
            return new Vector2(offset.y, -offset.x);
        }

        return offset;
    }


    public void SetCameraDir(CommonSE_Proto.E_DIRECTION dir)
    {
        CameraDir = dir;
    }

    public void PerformRayBasedSearch()
    {
        List<Vector2Int> searchArea = fieldDataManager.GetStatePos(_FieldDataManager.E_FIELDSTATE.sc_searchRange);
        Vector3 rayOrigin = GetRayStartPoint(SurveillanceCameraPos, CameraDir); // 近い頂点2つからでも良い

        foreach (var targetPos in searchArea)
        {
            if (targetPos == SurveillanceCameraPos) continue;

            Vector3 targetWorld = GridToWorld(targetPos);

            Vector3 dirToTarget = (targetWorld - rayOrigin).normalized;
            float dist = Vector3.Distance(rayOrigin, targetWorld);

            if (Physics.Raycast(rayOrigin, dirToTarget, out RaycastHit hit, dist))
            {
                if (IsWall(hit.collider.gameObject))
                {
                    // 壁に遮られている
                    Debug.Log("壁が範囲内にあります。");
                    continue;
                }
            }
            // 壁に遮られていない
        }
    }

    Vector3 GetRayStartPoint(Vector2Int gridPos, E_DIRECTION dir)
    {
        Vector3 world = GridToWorld(gridPos);

        switch (dir)
        {
            case CommonSE_Proto.E_DIRECTION.right:
                return world + new Vector3(0.5f, 0, 0.3f);
            case CommonSE_Proto.E_DIRECTION.left:
                return world + new Vector3(-0.5f, 0, -0.3f);
            case CommonSE_Proto.E_DIRECTION.up:
                return world + new Vector3(0.3f, 0, 0.5f);
            case CommonSE_Proto.E_DIRECTION.down:
                return world + new Vector3(-0.3f, 0, -0.5f);
            default:
                return world;
        }
    }

    Vector3 GridToWorld(Vector2Int grid)
    {
        return new Vector3(grid.x, 0, grid.y); // Yが高さ、X-ZがマスのXY
    }

    bool IsWall(GameObject obj)
    {
        return obj.CompareTag("Wall"); // 壁には"Wall"タグをつけておく
    }


    // 索敵範囲のマス座標を保持するリスト
    private List<Vector2Int> searchedTileList = new List<Vector2Int>();

    /// <summary>
    /// 現在の監視状態に応じて索敵しているマスのリストを返す
    /// </summary>
    /// <summary>
    /// 現在の監視状態に応じて索敵しているマスのリストを返す
    /// </summary>
    public List<Vector2Int> GetSearchedTileList()
    {
        List<Vector2Int> searchedTileList = new List<Vector2Int>();

        Vector2Int center = SurveillanceCameraPos;
        Vector2Int[] offsets = GetOffsetsBasedOnWatchState();

        // レイの発射点（監視カメラの世界座標）
        Vector3 rayOrigin = GetRayStartPoint(center, CameraDir);

        foreach (var offset in offsets)
        {
            Vector2Int targetGrid = center + offset;
            Vector3 targetWorld = GridToWorld(targetGrid);

            Vector3 dirToTarget = (targetWorld - rayOrigin).normalized;
            float dist = Vector3.Distance(rayOrigin, targetWorld);

            // 壁に遮られているかどうか判定
            if (Physics.Raycast(rayOrigin, dirToTarget, out RaycastHit hit, dist))
            {
                if (hit.collider.gameObject)
                {
                    // 壁があるため、このマスは視認不可
                    continue;
                }
            }

            // 遮蔽物なし＝視認可能なマスなのでリストに追加
            searchedTileList.Add(targetGrid);
        }

        // デバッグ表示
        foreach (var pos in searchedTileList)
        {
            Debug.Log($"視認可能な索敵マス: {pos}");
        }

        return searchedTileList;
    }

    /// <summary>
    /// 監視状態とforwardに応じて、索敵範囲の相対オフセットを返す
    /// </summary>
    private Vector2Int[] GetOffsetsBasedOnWatchState()
    {
        List<Vector2Int> offsetList = new List<Vector2Int>();

        Vector2Int forwardDir = Vector2Int.RoundToInt(forward);
        Vector2Int rightDir = new Vector2Int(-forwardDir.y, -forwardDir.x);

        int sideOffset = 0;
        if (watchState == E_WATCHSTATE.Left) sideOffset = -1;
        else if (watchState == E_WATCHSTATE.Right) sideOffset = 1;

        for (int i = 1; i <= 3; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector2Int offset = forwardDir * i + rightDir * (j + sideOffset);
                offsetList.Add(offset);
            }
        }

        return offsetList.ToArray();
    }
}