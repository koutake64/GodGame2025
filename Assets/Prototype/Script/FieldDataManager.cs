using UnityEngine;

public class FieldDataManager : MonoBehaviour
{
    public enum E_FIELDSTATE
    {
        outOfRange = -1,// 範囲外
        none,           // 何もない
        start,          // お姫様のスタート地点
        goal,           // お宝の場所
        obstacle,       // 障害物(仮)
        camera,         // 監視カメラ
        cameraRange,    // 監視カメラの見える範囲
        player,         // プレイヤー
        princess,       // お姫様
        security,       // 警備員

        _count
    }

    [System.Serializable]
    public struct S_FIELDINFO
    {
        public GameObject obj;
        public Vector2 pos;
        public E_FIELDSTATE state;
        public CommonSE_Proto.E_DIRECTION dir;
    }

    public S_FIELDINFO[,] fieldInfoArray;

    /// <summary>
    /// 配列の初期化
    /// </summary>
    /// <param name="range">範囲</param>
    public void InitArray(Vector2 range)
    {
        fieldInfoArray = new S_FIELDINFO[(int)range.x, (int)range.y];
    }

    /// <summary>
    /// 指定したマスに情報を追加する関数
    /// </summary>
    /// <param name="pos">マス指定</param>
    /// <param name="info">格納する情報</param>
    /// <returns>成功:true, 失敗:false</returns>
    public bool AddFieldInfo(Vector2 pos, S_FIELDINFO info)
    {
        // 範囲外チェック
        if (fieldInfoArray == null || pos.x < 0 || pos.y < 0 || pos.x >= fieldInfoArray.GetLength(0) || pos.y >= fieldInfoArray.GetLength(1))
        {
            Debug.LogError(
                "Script:FieldDataManager.cs \n" +
                "AddFieldInfo:指定のマスは範囲外です" +
                $"pos=({pos.x},{pos.y})"
                );
            return false;
        }

        // すでに何か入っている場合は追加しない
        if (fieldInfoArray[(int)pos.x, (int)pos.y].obj != null)
        {
            Debug.LogWarning(
                "Script:FieldDataManager.cs \n" +
                "AddFieldInfo:指定のマスには既に情報があります" +
                $"pos=({pos.x},{pos.y})"
                );
            return false;
        }

        fieldInfoArray[(int)pos.x, (int)pos.y] = info;
        fieldInfoArray[(int)pos.x, (int)pos.y].pos = pos;

        return true;
    }

    /// <summary>
    /// 情報のGetter
    /// </summary>
    /// <param name="pos">指定マス</param>
    /// <returns>指定したマスの情報</returns>
    public S_FIELDINFO GetInfo(Vector2 pos)
    {
        // 範囲外チェック
        if (fieldInfoArray == null || pos.x < 0 || pos.y < 0 || pos.x >= fieldInfoArray.GetLength(0) || pos.y >= fieldInfoArray.GetLength(1))
        {
            S_FIELDINFO info = new S_FIELDINFO();
            info.state = E_FIELDSTATE.outOfRange;
            info.obj = null;
            return info;
        }

        return fieldInfoArray[(int)pos.x, (int)pos.y];
    }

    /// <summary>
    /// 情報のSetter
    /// </summary>
    /// <param name="pos">指定マス</param>
    /// <param name="info">情報</param>
    /// <returns>指定したマスの情報を設定</returns>
    public bool SetInfo(Vector2 pos, S_FIELDINFO info)
    {
        // 範囲外チェック
        if (fieldInfoArray == null || pos.x < 0 || pos.y < 0 ||
            pos.x >= fieldInfoArray.GetLength(0) || pos.y >= fieldInfoArray.GetLength(1))
        {
            Debug.LogError(
                "Script:FieldDataManager.cs \n" +
                "SetInfo: 指定のマスは範囲外です" +
                $"pos=({pos.x},{pos.y})"
            );
            return false;
        }

        // 情報の上書き
        fieldInfoArray[(int)pos.x, (int)pos.y] = info;
        fieldInfoArray[(int)pos.x, (int)pos.y].pos = pos;

        return true;
    }

    // TODO プロト終わったら消す
    public void SetColor(Vector2Int pos, Color color)
    {
        fieldInfoArray[pos.x, pos.y].obj.GetComponent<MeshRenderer>().material.color = color;
    }
}
