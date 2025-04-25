using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    // ====== シリアライズ =====
    [SerializeField, Header("昼開始時刻(秒)")]     private float noonTime;
    [SerializeField, Header("夕方背景時刻(秒)")]   private float afterNoonTime;
    [SerializeField, Header("夜開始時刻(秒)")]     private float nightTime;
    [SerializeField, Header("時間表示テキスト")]   private Text timeText;
    [SerializeField, Header("状態テキスト")]       private Text levelText;
    [SerializeField, Header("太陽光")]             private GameObject sun;

    // --- 変数 ---
    private float time = 0; // 時間管理
    private CommonSE_Proto.E_TIMEOFDAY currentState = CommonSE_Proto.E_TIMEOFDAY.morning;

    private void Start()
    {
        levelText.text = time.ToString("朝");
        SunMove(0f);
    }

    private void Update()
    {
        time += Time.deltaTime;

        timeText.text = time.ToString("0" + "秒");

        if (currentState == CommonSE_Proto.E_TIMEOFDAY.morning && time >= noonTime)
        {
            SetTimeDay(CommonSE_Proto.E_TIMEOFDAY.noon);
            SunMove(30f);
        }
        else if (currentState == CommonSE_Proto.E_TIMEOFDAY.noon && time >= afterNoonTime)
        {
            SetTimeDay(CommonSE_Proto.E_TIMEOFDAY.afternoon);
            SunMove(185f);
        }
        else if (currentState == CommonSE_Proto.E_TIMEOFDAY.afternoon && time >= nightTime)
        {
            SetTimeDay(CommonSE_Proto.E_TIMEOFDAY.night);
            SunMove(200f);
        }
    }

    /// <summary>
    /// 時間の状態からテキスト変更
    /// </summary>
    /// <param name="newState">テキスト</param>
    private void SetTimeDay(CommonSE_Proto.E_TIMEOFDAY newState)
    {
        currentState = newState;

        switch (newState)
        {
            case CommonSE_Proto.E_TIMEOFDAY.morning:
                levelText.text = "朝";
                break;

            case CommonSE_Proto.E_TIMEOFDAY.noon:
                levelText.text = "昼";
                break;

            case CommonSE_Proto.E_TIMEOFDAY.afternoon:
                levelText.text = "夕方";
                break;

            case CommonSE_Proto.E_TIMEOFDAY.night:
                levelText.text = "夜";
                break;

        }
    }

    /// <summary>
    /// 太陽光の向きを変更
    /// </summary>
    /// <param name="angle">太陽光の角度</param>
    private void SunMove(float angle)
    {
        sun.transform.transform.localRotation = Quaternion.Euler(angle, 0, 0);
    }
}
