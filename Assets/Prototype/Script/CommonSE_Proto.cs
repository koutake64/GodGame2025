using UnityEngine;

/// <summary>
/// 共通して使用する構造体や列挙型を持つ静的クラス
/// </summary>
public static class CommonSE_Proto
{
    public enum E_DIRECTION
    { 
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    public static int DirConvInt(E_DIRECTION dir)
    {
        return (int)dir;
    }

    public static E_DIRECTION DirIncrement(E_DIRECTION inDir)
    {
        // TODO 未実装
        E_DIRECTION outDir = inDir;
        return outDir;
    }

    public static E_DIRECTION DirDecrement(E_DIRECTION inDir)
    {
        // TODO 未実装
        E_DIRECTION outDir = inDir;
        return outDir;
    }

}
