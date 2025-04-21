using UnityEngine;

public class FieldDataManager : MonoBehaviour
{
    public enum E_FIELDSTATE
    {
        none,
        start,
        goal,

        _count
    }

    [System.Serializable]
    public struct S_FIELDINFO
    {
        public GameObject obj;
        public E_FIELDSTATE state;
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
        int x = (int)pos.x;
        int y = (int)pos.y;

        // 範囲外チェック
        if (fieldInfoArray == null || x < 0 || y < 0 || x >= fieldInfoArray.GetLength(0) || y >= fieldInfoArray.GetLength(1))
        {
            Debug.LogError(
                "Script:FieldDataManager.cs \n" +
                "AddFieldInfo:指定のマスは範囲外です" +
                $"pos=({x},{y})"
                );
            return false;
        }

        // すでに何か入っている場合は追加しない
        if (fieldInfoArray[x, y].obj != null)
        {
            Debug.LogWarning(
                "Script:FieldDataManager.cs \n" +
                "AddFieldInfo:指定のマスには既に情報があります" +
                $"pos=({x},{y})"
                );
            return false;
        }

        fieldInfoArray[x, y] = info;
        return true;
    }

    /// <summary>
    /// 情報のGetter
    /// </summary>
    /// <param name="pos">指定マス</param>
    /// <returns>指定したマスの情報</returns>
    public S_FIELDINFO GetInfo(Vector2 pos)
    {
        return fieldInfoArray[(int)pos.x, (int)pos.y];
    }

}
