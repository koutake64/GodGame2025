using UnityEngine;

public class SurveillanceCamera : MonoBehaviour
{
    [SerializeField] private FieldDataManager fieldDataManager;
    [SerializeField] private GameObject rotatingObject;

    private int x;
    private int y;

    private Vector2 forward = Vector2.up;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            forward = RotateDirection(forward, true);
            Debug.Log("カメラを左に回したよ！向き：" + forward);
            RotateVisualObject();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            forward = RotateDirection(forward, false);
            Debug.Log("カメラを右に回したよ！向き：" + forward);
            RotateVisualObject();
        }

        ResetCameraRange();
        SearchRange();
    }

    public void PositionSave(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    private Vector2 RotateDirection(Vector2 dir, bool isLeft)
    {
        if (dir == Vector2.up) return isLeft ? Vector2.left : Vector2.right;
        if (dir == Vector2.right) return isLeft ? Vector2.up : Vector2.down;
        if (dir == Vector2.down) return isLeft ? Vector2.right : Vector2.left;
        if (dir == Vector2.left) return isLeft ? Vector2.down : Vector2.up;
        return Vector2.up;
    }

    private void RotateVisualObject()
    {
        if (rotatingObject == null)
        {
            Debug.LogWarning("回すオブジェクトがないよ！");
            return;
        }

        float angleY = 0f;

        if (forward == Vector2.up) angleY = 0f;
        else if (forward == Vector2.right) angleY = 90f;
        else if (forward == Vector2.down) angleY = 180f;
        else if (forward == Vector2.left) angleY = 270f;

        rotatingObject.transform.rotation = Quaternion.Euler(0f, angleY, 0f);
    }

    private void SearchRange()
    {
        Vector2 center = new Vector2(x, y);
        int maxX = fieldDataManager.fieldInfoArray.GetLength(0);
        int maxY = fieldDataManager.fieldInfoArray.GetLength(1);

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = 1; dy <= 3; dy++)
            {
                Vector2 offset = new Vector2(dx, dy);
                Vector2 rotatedOffset = RotateOffset(offset, forward);
                Vector2 targetPos = center + rotatedOffset;

                int tx = (int)targetPos.x;
                int ty = (int)targetPos.y;

                // 範囲チェックをここで直接行う
                if (tx < 0 || tx >= maxX || ty < 0 || ty >= maxY)
                {
                    continue;
                }

                var info = fieldDataManager.GetInfo(targetPos);

                if (info.state == FieldDataManager.E_FIELDSTATE.none)
                {
                    info.state = FieldDataManager.E_FIELDSTATE.cameraRange;

                    if (info.obj != null)
                    {
                        MeshRenderer rend = info.obj.GetComponent<MeshRenderer>();
                        if (rend != null)
                        {
                            rend.material.color = Color.red; // 赤に変える
                        }
                    }

                    fieldDataManager.SetInfo(targetPos, info);
                }
            }
        }
    }

    private void ResetCameraRange()
    {
        int maxX = fieldDataManager.fieldInfoArray.GetLength(0);
        int maxY = fieldDataManager.fieldInfoArray.GetLength(1);
        
        for (int i = 0; i < maxX; i++)
        {
            for (int j = 0; j < maxY; j++)
            {
                Vector2 pos = new Vector2(i, j);
                FieldDataManager.S_FIELDINFO info = fieldDataManager.GetInfo(pos);

                if (info.state == FieldDataManager.E_FIELDSTATE.cameraRange)
                {
                    info.state = FieldDataManager.E_FIELDSTATE.none;

                    if (info.obj != null)
                    {
                        // TODO 元の色を変える処理を書く後で！！
                        MeshRenderer rend = info.obj.GetComponent<MeshRenderer>();
                        if (rend != null)
                        {
                            rend.material.color = Color.green; // 緑に変える
                        }
                    }

                    fieldDataManager.SetInfo(pos, info);
                }
            }
        }
    }

    private Vector2 RotateOffset(Vector2 offset, Vector2 forward)
    {
        if (forward == Vector2.up) return offset;
        if (forward == Vector2.right) return new Vector2(-offset.y, offset.x);
        if (forward == Vector2.down) return new Vector2(-offset.x, -offset.y);
        if (forward == Vector2.left) return new Vector2(offset.y, -offset.x);
        return offset;
    }
}