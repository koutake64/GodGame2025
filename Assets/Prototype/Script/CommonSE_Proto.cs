using UnityEngine;

/// <summary>
/// 共通して使用する構造体や列挙型を持つ静的クラス
/// </summary>
public static class CommonSE_Proto
{
    public enum E_DIRECTION
    { 
        up,
        right,
        down,
        left
    }

    public enum E_TIMEOFDAY
    {
        morning,
        noon,
        night
    }


    public static int DirConvInt(E_DIRECTION dir)
    {
        return (int)dir;
    }

}
