using UnityEngine;

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
        Right
    }

    // 現在の監視状態（初期は中央）
    private E_WATCHSTATE watchState = E_WATCHSTATE.Center;

    // フィールド情報を管理するクラス
    private FieldDataManager fieldDataManager;

    // 向きの変更に合わせて見た目を回転させるオブジェクト
    [SerializeField] private GameObject rotatingObject;



    // カメラの位置（マス座標）
    private int x;
    private int y;

    // 左右の視点切替時のスライド量（-2～+2）
    private int slideOffset = 0;

    // カメラの正面方向（初期は上方向）
    private Vector2 forward = Vector2.up;

    private void Start()
    {
        // フィールドマネージャーを取得
        fieldDataManager = GameObject.Find("Field").GetComponentInChildren<FieldDataManager>();
        if (!fieldDataManager)
        {
            Debug.LogError(
                "Script:SurveillanceCamera.cs \n" +
                "fieldDataManagerがnullです"
            );
        }

        // ワールド座標をマス座標に変換して保存
        Vector3 worldPos = transform.position;
        x = Mathf.RoundToInt(worldPos.x);
        y = Mathf.RoundToInt(worldPos.z); // Z軸をマスのYとして使用

        Debug.Log($"カメラ位置（マス座標）: ({x}, {y})");
    }

    void Update()
    {
        // Kキー：監視状態を右に切り替える
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (watchState == E_WATCHSTATE.Left)
            {
                watchState = E_WATCHSTATE.Center;
            }
            else if (watchState == E_WATCHSTATE.Center)
            {
                watchState = E_WATCHSTATE.Right;
            }

            Debug.Log("→ 現在の監視状態：" + watchState);
        }
        // Lキー：監視状態を左に切り替える
        else if (Input.GetKeyDown(KeyCode.L))
        {
            if (watchState == E_WATCHSTATE.Right)
            {
                watchState = E_WATCHSTATE.Center;
            }
            else if (watchState == E_WATCHSTATE.Center)
            {
                watchState = E_WATCHSTATE.Left;
            }

            Debug.Log("← 現在の監視状態：" + watchState);
        }

        // 監視範囲の状態をリセット
        ResetCameraRange();

        // 現在の監視状態に応じて索敵処理を実行
        SearchRange();
    }

    /// <summary>
    /// 与えられた方向を90度回転させる
    /// </summary>
    private Vector2 RotateDirection(Vector2 dir, bool isLeft)
    {
        if (dir == Vector2.up)
        {
            if (isLeft)
            {
                return Vector2.left;
            }
            else
            {
                return Vector2.right;
            }
        }
        else if (dir == Vector2.right)
        {
            if (isLeft)
            {
                return Vector2.up;
            }
            else
            {
                return Vector2.down;
            }
        }
        else if (dir == Vector2.down)
        {
            if (isLeft)
            {
                return Vector2.right;
            }
            else
            {
                return Vector2.left;
            }
        }
        else if (dir == Vector2.left)
        {
            if (isLeft)
            {
                return Vector2.down;
            }
            else
            {
                return Vector2.up;
            }
        }

        return Vector2.up;
    }

    /// <summary>
    /// カメラオブジェクトの見た目をforwardの向きに合わせて回転させる
    /// </summary>
    private void RotateVisualObject()
    {
        if (rotatingObject == null)
        {
            Debug.LogWarning("回すオブジェクトがないよ");
            return;
        }

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

        rotatingObject.transform.rotation = Quaternion.Euler(0f, angleY, 0f);
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
        }
        else if (watchState == E_WATCHSTATE.Center)
        {
            offsetValue = 0;
        }
        else if (watchState == E_WATCHSTATE.Right)
        {
            offsetValue = -2;
        }

        // スライド方向を現在の向きに回転
        Vector2 slideDir = RotateOffset(new Vector2(offsetValue, 0), forward);

        // 索敵範囲の中心位置を計算（カメラの2マス先＋スライド方向）
        Vector2 center = new Vector2(x, y) + forward * 2 + slideDir;

        int maxX = fieldDataManager.fieldInfoArray.GetLength(0);
        int maxY = fieldDataManager.fieldInfoArray.GetLength(1);

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
                if (tx < 0 || tx >= maxX || ty < 0 || ty >= maxY)
                {
                    continue;
                }

                var info = fieldDataManager.GetInfo(targetPos);
                if (info.state == FieldDataManager.E_FIELDSTATE.none)
                {
                    info.state = FieldDataManager.E_FIELDSTATE.cameraRange;

                    // オブジェクトの色を赤に変更
                    if (info.obj != null)
                    {
                        var rend = info.obj.GetComponent<MeshRenderer>();
                        if (rend != null)
                        {
                            rend.material.color = Color.red;
                        }
                    }

                    fieldDataManager.SetInfo(targetPos, info);
                }
            }
        }
    }

    /// <summary>
    /// 前フレームに設定されたカメラの索敵範囲をリセットし、元の色に戻す
    /// </summary>
    private void ResetCameraRange()
    {
        int maxX = fieldDataManager.fieldInfoArray.GetLength(0);
        int maxY = fieldDataManager.fieldInfoArray.GetLength(1);

        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                var pos = new Vector2(x, y);
                var info = fieldDataManager.GetInfo(pos);

                if (info.state == FieldDataManager.E_FIELDSTATE.cameraRange)
                {
                    info.state = FieldDataManager.E_FIELDSTATE.none;

                    // タイルの色をチェッカーパターンで復元
                    if (info.obj != null)
                    {
                        var rend = info.obj.GetComponent<MeshRenderer>();
                        if (rend != null)
                        {
                            //TODO 仮で床の色を決めてあります今後変更予定
                            if ((x + y) % 2 == 0)
                            {
                                fieldDataManager.SetColor(new Vector2Int(x, y), Color.gray);
                            }
                            else
                            {
                                fieldDataManager.SetColor(new Vector2Int(x, y), Color.white);
                            }
                        }
                    }

                    fieldDataManager.SetInfo(pos, info);
                }
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
}