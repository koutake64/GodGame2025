using System.Runtime.CompilerServices;
using UnityEngine;

public class SurveillanceCamera : MonoBehaviour
{
    [SerializeField] FieldDataManager fieldDataManager;

    private Vector2 pos; // カメラの設置位置
    private Vector2 forward = Vector2.up; // 初期向き：上（前方向）

    private void Update()
    {
        /////////////////////////////////////////////////////////////////////
        ///アイテムが使用されたときに後で変える
        ///投げられた方向で向きが変わるようにする。
        // キーボード入力で向きを変更（K:左, L:右）
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("左回転");
            forward = RotateDirection(forward, true); // 左回転
            UpdateView();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("右回転");
            forward = RotateDirection(forward, false); // 右回転
            UpdateView();
        }
        ////////////////////////////////////////////////////////////////////////
    }

    /// <summary>
    /// 監視カメラの設置位置を保存
    /// </summary>
    public void SetPosition(Vector2 _pos)
    {
        pos = _pos;
        UpdateView(); // 初期状態の視野を描画
    }

    /// <summary>
    /// 視野を3x3で描画
    /// </summary>
    private void UpdateView()
    {
        // 3x3の範囲を前方方向にオフセットして確認
        for (int y = -1; y <= 1; y++)
        {
            for (int x = 1; x <= 3; x++)
            {
                Vector2 offset = new Vector2(x, y);
                Vector2 rotatedOffset = RotateOffset(offset, forward);
                Vector2 targetPos = pos + rotatedOffset;

                var info = new FieldDataManager.S_FIELDINFO();
                info.state = FieldDataManager.E_FIELDSTATE.cameraRange;
                fieldDataManager.SetInfo(targetPos, info);
            }
        }

        // カメラ自身のマスも設定
        var selfInfo = new FieldDataManager.S_FIELDINFO();
        selfInfo.state = FieldDataManager.E_FIELDSTATE.camera;
        fieldDataManager.SetInfo(pos, selfInfo);
    }

    /// <summary>
    /// 向きに応じてオフセットを回転
    /// </summary>
    private Vector2 RotateOffset(Vector2 offset, Vector2 forward)
    {
        // forwardに合わせてオフセットを回転（上基準）
        if (forward == Vector2.up) return offset;
        if (forward == Vector2.right) return new Vector2(-offset.y, offset.x);
        if (forward == Vector2.down) return new Vector2(-offset.x, -offset.y);
        if (forward == Vector2.left) return new Vector2(offset.y, -offset.x);
        return offset;
    }

    /// <summary>
    /// 向きを90度回転（左か右か指定）
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

        if (dir == Vector2.right)
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

        if (dir == Vector2.down)
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

        if (dir == Vector2.left)
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
        // 万が一どの条件にも当てはまらなかったら上を返す（デフォルト）
        return Vector2.up;
    }
}