using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// お宝にコンポーネントするスクリプト
/// お宝ゲット時のエフェクト、お宝の後光のエフェクトを出す
/// </summary>
public class TreasureEffect : MonoBehaviour
{
    [Header("ゲットエフェクト")]
    [SerializeField] private VisualEffect getEffect;

    [Header("後光エフェクト")]
    [SerializeField] private VisualEffect haloEffect;

    void Start()
    {
        getEffect.Stop(); // ゲットエフェクトは初期状態で停止
        haloEffect.Play(); // 後光エフェクトは開始時に再生
    }

    /// <summary>
    /// ゲットエフェクトを開始するメソッド
    /// お嬢様がお宝のマスに行ったときに呼び出される
    /// </summary>
    public void StartGetEffect()
    {
        getEffect.Play();
    }
}
