using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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
    FieldDataManager fieldManager;




    //レイ用
    public float rayLength = 6.0f;   // Rayの長さ（6.0f）
    public int rayCount = 8;         // Rayの本数（例：6本で扇状）
    private void Start()
    {
        // フィールドマネージャーを取得
        fieldDataManager = GameObject.Find("Field").GetComponentInChildren<_FieldDataManager>();
        if (!fieldDataManager)
        {
            Debug.LogError(
                "Script:SurveillanceCamera.cs \n" +
                "fieldDataManagerがnullです"
            );
        }

        SurveillanceCameraPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        Debug.Log($"カメラ位置（マス座標）: ({SurveillanceCameraPos}");

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
            Debug.LogError("プレイヤーが見つかりませんでした");
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
            Debug.LogError("プリンセスが見つかりませんでした");
        }


        Vector2Int playerGridPos = new Vector2Int((int)playerPos.x, (int)playerPos.z);

        int PlayerInRangeY = Mathf.Abs(playerGridPos.y - SurveillanceCameraPos.y) + 1;
        int PlayerInRangeX = Mathf.Abs(playerGridPos.x - SurveillanceCameraPos.x) + 1;
        //Debug.Log($"カメラとプレイヤーとの距離X" + (PlayerInRangeX));
        //Debug.Log($"カメラとプレイヤーとの距離Y" + (PlayerInRangeY));
        if ((PlayerInRangeX < 1) &&
            (PlayerInRangeY < 1))
        {
            isPlayerInRange = true;
            //Debug.Log("カメラの向きが変えられる");
        }
        else
        {
            isPlayerInRange = false;
            //Debug.Log("カメラの向きが変えられません");
        }

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Return))
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

        float angleY = 0f;

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

        // 状態に応じてスライド方向を決定
        if (watchState == E_WATCHSTATE.Left)
        {
            offsetValue = 2;
            this.transform.rotation = Quaternion.Euler(0f, this.transform.rotation.y - 45, 0f);
        }
        else if (watchState == E_WATCHSTATE.Center)
        {
            offsetValue = 0;
        }
        else if (watchState == E_WATCHSTATE.Right)
        {
            offsetValue = -2;
            this.transform.rotation = Quaternion.Euler(0f, this.transform.rotation.y + 45, 0f);
        }

        // スライド方向を現在の向きに回転
        Vector2 slideDir = RotateOffset(new Vector2(offsetValue, 0), forward);


        // 索敵範囲の中心位置を計算（カメラの2マス先＋スライド方向）
        Vector2 center = SurveillanceCameraPos + forward * 2 + slideDir;

        Vector2Int max = fieldDataManager.GetFieldSize();

        // お嬢様がいるか確認する座標を配列に格納
        List<Vector2Int> checkList = new List<Vector2Int>();

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

            ////Rayが何かに当たったかチェック（距離制限付き）
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == this.gameObject)
                    continue; // 自分自身は無視

                // 3×3の範囲を走査
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        Vector2 offset = new Vector2(dx, dy);
                        Vector2 rotatedOffset = RotateOffset(offset, forward);
                        Vector2 targetPos = center + rotatedOffset;

                        int tx = (int)targetPos.x;
                        int ty = (int)targetPos.y;

                        // 範囲外は無視
                        if (tx < 0 || tx >= max.x || ty < 0 || ty >= max.y)
                        {
                            continue;
                        }


                        Vector2Int pos = new Vector2Int(tx, ty);
                        // プリンセスなら表示
                        if (hit.collider.CompareTag("Princess"))
                        {
                            Debug.Log("プリンセス発見！" + hit.collider.gameObject.name);
                        }
                        else if (hit.collider.CompareTag("Player"))
                        {
                            Debug.Log("執事発見！" + hit.collider.gameObject.name);
                        }
                        else
                        {
                            Debug.Log("何かにヒット → " + hit.collider.gameObject.name);
                        }


                        // 確認リストに追加
                        checkList.Add(pos);
                    }
                }
            }
        }






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
            }
        }

        // お嬢様が完全に範囲外か確認
        int rangeCount = checkList.Count;   // 座標確認数

        foreach (var checkPos in checkList)
        {
            // 座標がお嬢様と座標が違うなら
            if (checkPos != princessGridPos)
            {
                // 確認済み
                rangeCount--;
                continue;
            }

            if (securityObj.Count != 0)
            {
                foreach (var obj in securityObj)
                {
                    // すでに通知済みなら次へ
                    if (obj.GetIsFoundPrincess())
                        continue;

                    obj.FoundPrincess(checkPos);
                }
            }
        }

        // 確認座標の全てにお嬢様がいない場合通知済みフラグを下げる
        if (rangeCount == 0 && securityObj.Count != 0)
        {
            foreach (var obj in securityObj)
            {
                // フラグが立っていないならfalseにする必要なので次へ
                if (!obj.GetIsFoundPrincess()) continue;

                obj.SetIsFoundPrincess(false);
            }
        }
    }


    /// <summary>
    /// 前フレームに設定されたカメラの索敵範囲をリセットし、元の色に戻す
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


}