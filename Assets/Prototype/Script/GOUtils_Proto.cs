using UnityEngine;

/// <summary>
/// ゲームオブジェクトに関する便利関数を持つ静的クラス
/// </summary>
public static class GOUtils_Proto
{
    /// <summary>
    /// ゲームオブジェクトが存在するか確認する関数
    /// </summary>
    /// <param name="obj">確認したいゲームオブジェクト</param>
    /// <returns>存在する:true, 存在しない:false</returns>
    public static bool CheckGameObject(GameObject obj)
    {
        if (obj)
        {
            return true;
        }
        else
        {   
            return false;
        }
    }
}
