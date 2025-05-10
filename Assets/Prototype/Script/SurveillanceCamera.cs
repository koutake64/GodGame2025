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

    FieldDataManager fieldManager; // マスの状態取得用

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
        Debug.Log($"カメラとプレイヤーとの距離X" + (PlayerInRangeX));
        Debug.Log($"カメラとプレイヤーとの距離Y" + (PlayerInRangeY));
        if ((PlayerInRangeX < 1) &&
            (PlayerInRangeY < 1))
        {
            isPlayerInRange = true;
            Debug.Log("カメラの向きが変えられる");
        }
        else
        {
            isPlayerInRange = false;
            Debug.Log("カメラの向きが変えられません");
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
            Debug.Log("→ 現在の監視状態：" + watchState + SurveillanceCameraPos);
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

                // 確認リストに追加
                checkList.Add(pos);
            }
        }

        // お嬢様の座標
        Vector2Int princessGridPos = new Vector2Int((int)princessPos.x, (int)princessPos.z);



        // 対象タグの全オブジェクトを取得
        List<GameObject> securityObj = new List<GameObject>(GameObject.FindGameObjectsWithTag("Security"));

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

            // 対象の範囲内にいるか判定
            foreach (var obj in securityObj)
            {
                // CharacterMoveControllerが無い場合次へ
                var charaMove = obj.GetComponent<CharacterMoveController>();
                if (!charaMove) continue;

                // 対象オブジェクトの座標取得
                Vector2Int secyrityPos = charaMove.GetCurrentPos();
                Vector2Int cameraPos = new Vector2Int((int)this.transform.position.x, (int)this.transform.position.z);

                // 座標の差の絶対値を計算
                Vector2Int differencePos =
                    new Vector2Int(Mathf.Abs(secyrityPos.x - cameraPos.x), Mathf.Abs(secyrityPos.y - cameraPos.y));

                // 影響範囲内なら
                if (differencePos.x <= callRange && differencePos.y <= callRange)
                {
                    // SecurityConrtollerが無いなら次へ
                    var security = obj.GetComponent<SecurityController>();
                    if (!security) continue;

                    // すでに通知済みなら次へ
                    if (security.GetIsFoundPrincess())
                        continue;

                    Debug.Log("通知");

                    security.FoundPrincess(checkPos);
                }
            }
        }

        // 確認座標の全てにお嬢様がいない場合通知済みフラグを下げる
        if(rangeCount == 0)
        {
            foreach(var obj in securityObj)
            {
                // SecurityConrtollerが無いなら次へ
                var security = obj.GetComponent<SecurityController>();
                if (!security) continue;

                security.SetIsFoundPrincess(false);
            }
        }
    }

            /*//if (info[].state == _FieldDataManager.E_FIELDSTATE.sc_searchRange)
            //{
            //}*/
            /* Vector2Int pos = new Vector2Int(tx, ty);
             var info = fieldDataManager.GetInfoList(pos);

             for (int i = 0; i < info.Count; i++)
             {
                 if (info[i].state == _FieldDataManager.E_FIELDSTATE.sc_searchRange)
                 { 
                   //  ChangeColor(Color.red);
                 }
             }
            */
     
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

                /*var info = fieldDataManager.GetInfoList(pos);
                for (int i = 0; i < info.Count; i++)
                {
                    if (info[i].state != _FieldDataManager.E_FIELDSTATE.sc_searchRange)
                    {
                        // タイルの色をチェッカーパターンで復元
                        if (info != null)
                        {
                            int num = x + y;

                            if (num % 2 == 0)
                            {
                               // ChangeColor(Color.gray);
                            }
                            else
                            {
                               // ChangeColor(Color.white);
                            }
                        }
                    }
                }*/
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

   /* private void CheckPillar()//柱があるかチェック
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Vector2 offset = new Vector2(dx, dy);
                Vector2 rotatedOffset = RotateOffset(offset, forward * 2);
                Vector2 targetPos = SurveillanceCameraPos + rotatedOffset;

                int tx = (int)targetPos.x;
                int ty = (int)targetPos.y;

                Vector2Int pos = new Vector2Int(tx, ty);
                var info = fieldDataManager.GetInfoList(pos);


               // CommonSE_Proto.E_DIRECTION Dir
               

                if (watchState == E_WATCHSTATE.Left)
                {
                    
                    for (int i = 0; i < info.Count; i++)
                    {
                        if (info[i].state == _FieldDataManager.E_FIELDSTATE.pillar)
                        { }
                    }
                }
                else if (watchState == E_WATCHSTATE.Center)
                {

                    for (int i = 0; i < info.Count; i++)
                    {
                        if (info[i].state == _FieldDataManager.E_FIELDSTATE.pillar)
                        { }
                    }
                }
                else if (watchState == E_WATCHSTATE.Right)
                {

                    for (int i = 0; i < info.Count; i++)
                    {
                        if (info[i].state == _FieldDataManager.E_FIELDSTATE.pillar)
                        { }
                    }
                }
            }
        }
    }
   */

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