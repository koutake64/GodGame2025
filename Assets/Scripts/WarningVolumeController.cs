using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class WarningVolumeController : MonoBehaviour
{
    public static WarningVolumeController Instance { get; private set; }

    [Header("警戒範囲の最大距離")]
    public float maxAlertDistance = 10f;

    [Header("赤フラッシュのスピード")]
    public float flashSpeed = 4f;

    [Header("赤フラッシュ持続時間（秒）")]
    public float detectionDuration = 2f;

    // 警備員のTransformリストとプレイヤーのTransform
    private List<Transform> security = new List<Transform>();
    private Transform princess;

    private TimeManager timeManager; // タイムマネージャーの参照

    // ポストプロセス関連
    private Volume volume;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;

    private float lastDetectionTime = -10f;
    private bool hasInitializedThisNight = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // タイムマネージャー取得
        timeManager = GameObject.Find("Canvas")?.GetComponent<TimeManager>();
        if (!timeManager)
        {
            Debug.LogError("WarningVolumeController.cs: TimeManagerが見つかりません。");
        }

        // Volumeエフェクト取得
        volume = GetComponent<Volume>();
        if (volume && volume.profile.TryGet(out chromaticAberration) && volume.profile.TryGet(out colorAdjustments))
        {
            chromaticAberration.intensity.overrideState = true;
            colorAdjustments.colorFilter.overrideState = true;
            chromaticAberration.intensity.value = 0f;
            colorAdjustments.colorFilter.value = Color.white;
        }
        else
        {
            Debug.LogError("WarningVolumeController: Volumeプロファイルに設定が足りません！");
        }
    }

    void Update()
    {
        if (!timeManager || !volume) return;

        var currentState = timeManager.GetCurState();
        bool isNight = currentState == CommonSE_Proto.E_TIMEOFDAY.night;

        // VolumeのON/OFFを制御（夜のみ有効）
        volume.enabled = isNight;

        if (!isNight)
        {
            princess = null; // 朝・昼・夕方はnullに戻す（オプション）
            chromaticAberration.intensity.value = 0f;
            colorAdjustments.colorFilter.value = Color.white;
            return;
        }

        if (isNight && !hasInitializedThisNight)
        {
            // 警備員取得
            security.Clear(); // 念のため初期化
            GameObject[] guards = GameObject.FindGameObjectsWithTag("Security");
            foreach (var g in guards)
            {
                if (g != null)
                {
                    security.Add(g.transform);
                    Debug.Log("警備員取得: " + g.name);
                }
            }

            hasInitializedThisNight = true;
        }

        // プリンセス取得
        if (princess == null)
        {
            GameObject princessObj = GameObject.FindGameObjectWithTag("Princess");
            if (princessObj)
            {
                princess = princessObj.transform;
                Debug.Log("プリンセスを取得しました！");
            }
            else
            {
                Debug.LogWarning("プリンセスがまだ見つかりません。次フレームで再試行します。");
                return;
            }
        }

        UpdateAberration();
        UpdateDetectionFlash();
    }

    private void UpdateAberration()
    {
        float nearestDistance = float.MaxValue;

        foreach (var guard in security)
        {
            if (guard == null) continue;

            float dist = Vector3.Distance(princess.position, guard.position);

            if (dist < nearestDistance)
                nearestDistance = dist;
        }

        float t = Mathf.Clamp01(1f - (nearestDistance / maxAlertDistance));

        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = t;
        }
    }


    private void UpdateDetectionFlash()
    {
        bool shouldFlash = Time.time - lastDetectionTime < detectionDuration;

        if (shouldFlash)
        {
            float ping = Mathf.PingPong(Time.time * flashSpeed, 1f);
            colorAdjustments.colorFilter.value = Color.Lerp(Color.white, Color.red, ping);
        }
        else
        {
            colorAdjustments.colorFilter.value = Color.white;
        }
    }

    /// <summary>
    /// 外部から警告フラグを立てる（カメラ or 警備員発見時）
    /// </summary>
    public void NotifyCameraDetection()
    {
        lastDetectionTime = Time.time;
    }
}
