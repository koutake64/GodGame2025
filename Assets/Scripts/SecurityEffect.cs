using UnityEngine;
using UnityEngine.VFX;

/// <summary>
/// 警備員にコンポーネントするスクリプト
/// ビックリマークエフェクトを制御する
/// </summary>
public class SecurityEffect : MonoBehaviour
{
    [Header("シングルビックリマークエフェクト")]
    [SerializeField] private VisualEffect singleAlertEffect;

    [Header("ダブルビックリマークエフェクト")]
    [SerializeField] private VisualEffect doubleAlertEffect;

    void Start()
    {
        singleAlertEffect.Stop(); // シングルビックリマークエフェクトは初期状態で停止
        doubleAlertEffect.Stop(); // ダブルビックリマークエフェクトも初期状態で停止
    }

    /// <summary>
    /// シングルビックリマークエフェクトを開始するメソッド
    /// お嬢様が監視カメラに見つかり、範囲内の警備員に通知する際に呼び出される
    /// </summary>
    public void StartSingleAlertEffect()
    {
        singleAlertEffect.Play(); // シングルビックリマークエフェクトを再生
    }

    /// <summary>
    /// ダブルビックリマークエフェクトを開始するメソッド
    /// お嬢様が警備員に見つかった際に呼び出される
    /// </summary>
    public void StartDoubleAlertEffect()
    {
        doubleAlertEffect.Play(); // ダブルビックリマークエフェクトを再生
    }
}
