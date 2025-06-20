using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TimeManager : MonoBehaviour
{
	[SerializeField] private StageData stageData;
	[SerializeField] private System.Collections.Generic.List<StageTimeData> stageTimeSet;
    // ====== シリアライズ =====
    [SerializeField, Header("昼開始時刻(秒)")]		private float noonTime;
    [SerializeField, Header("夕方背景時刻(秒)")]	private float afterNoonTime;
    [SerializeField, Header("夜開始時刻(秒)")]		private float nightTime;
	[SerializeField, Header("フェード時間(秒)")]	private float fadeDuration = 5f;
	[SerializeField, Header("時間表示テキスト")]	private Text timeText;
    [SerializeField, Header("状態テキスト")]		private Text levelText;
    [SerializeField, Header("太陽光")]				private GameObject sun;
	[SerializeField, Header("ゲーム内速度"), Range(0, 2)] private float gameSpeed;


	[System.Serializable]
	public class StageTimeData
	{
		public StageData stageData;
		public float noonTime;
		public float afterNoonTime;
		public float nightTime;
	}

    // --- 変数 ---
    private float time = 0; // 時間管理
    private CommonSE_Proto.E_TIMEOFDAY currentState = CommonSE_Proto.E_TIMEOFDAY.morning;
	private CommonSE_Proto.E_TIMEOFDAY prevState = CommonSE_Proto.E_TIMEOFDAY.morning;

	private void Start()
    {
        var fieldDataManager = FindFirstObjectByType<_FieldDataManager>();
        if (fieldDataManager != null)
        {
            stageData = fieldDataManager.GetStageData();
        }
        else
        {
            Debug.LogWarning("_FieldDataManager が見つかりませんでした。");
        }

        ApplyTimeSettings();

        levelText.text = "朝";
        SunMove(0f);
        SetTimeScale(gameSpeed);
    }

    private void ApplyTimeSettings()
    {
        foreach (var setting in stageTimeSet)
        {
            if (setting.stageData == stageData)
            {
                noonTime = setting.noonTime;
                afterNoonTime = setting.afterNoonTime;
                nightTime = setting.nightTime;
                return;
            }
        }

        Debug.LogWarning("一致するステージ時間設定が見つかりませんでした。");
    }

	private void Update()
	{
        prevState = currentState;

        time += Time.deltaTime;
        timeText.text = time.ToString("0" + "秒");

        float sunAngle = 0f;

        if (time < noonTime - fadeDuration)
        {
            currentState = CommonSE_Proto.E_TIMEOFDAY.morning;
            sunAngle = 0f;
        }
        else if (time < noonTime)
        {
            float t = (time - (noonTime - fadeDuration)) / fadeDuration;
            currentState = CommonSE_Proto.E_TIMEOFDAY.morning;
            sunAngle = Mathf.Lerp(0f, 30f, t);
        }
        else if (time < afterNoonTime - fadeDuration)
        {
            currentState = CommonSE_Proto.E_TIMEOFDAY.noon;
            sunAngle = 30f;
        }
        else if (time < afterNoonTime)
        {
            float t = (time - (afterNoonTime - fadeDuration)) / fadeDuration;
            currentState = CommonSE_Proto.E_TIMEOFDAY.noon;
            sunAngle = Mathf.Lerp(30f, 185f, t);
        }
        else if (time < nightTime - fadeDuration)
        {
            currentState = CommonSE_Proto.E_TIMEOFDAY.afternoon;
            sunAngle = 185f;
        }
        else if (time < nightTime)
        {
            float t = (time - (nightTime - fadeDuration)) / fadeDuration;
            currentState = CommonSE_Proto.E_TIMEOFDAY.afternoon;
            sunAngle = Mathf.Lerp(185f, 200f, t);
        }
        else
        {
            currentState = CommonSE_Proto.E_TIMEOFDAY.night;
            sunAngle = 200f;
        }

        // 状態が変わったときのみテキスト更新
        if (currentState != prevState)
        {
            UpdateLevelText();
        }

        SunMove(sunAngle);
    }

   
	/// <summary>
	/// 現在の状態に応じて状態テキストを更新
	/// </summary>
	private void UpdateLevelText()
	{
		switch (currentState)
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

	public CommonSE_Proto.E_TIMEOFDAY CurrentState => currentState;

/*#if UNITY_EDITOR
	// デバッグ用：特定の時間帯に強制変更
	public void ForceChangeState(CommonSE_Proto.E_TIMEOFDAY newState)
	{
		prevState = currentState;
		currentState = newState;
		//prevState = newState;

		switch (newState)
		{
			case CommonSE_Proto.E_TIMEOFDAY.morning:
				time = 0;
				SunMove(0f);
				break;
			case CommonSE_Proto.E_TIMEOFDAY.noon:
				time = noonTime;
				SunMove(30f);
				break;
			case CommonSE_Proto.E_TIMEOFDAY.afternoon:
				time = afterNoonTime;
				SunMove(185f);
				break;
			case CommonSE_Proto.E_TIMEOFDAY.night:
				time = nightTime;
				SunMove(200f);
				break;
		}

		UpdateLevelText();
		FindAnyObjectByType<ModelShiftByTime>()?.Refresh();

	}
#endif
*/

	public CommonSE_Proto.E_TIMEOFDAY GetCurState()
	{
		return currentState;
	}

	public bool IsChangeState()
	{
		if (currentState != prevState)
		{
			Debug.Log("change");
			return true;
		}

		return false;
	}

	public void SetTimeScale(float num)
	{
		Time.timeScale = num;
	}

	/// <summary>
	/// それぞれの開始時間の取得
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	public float GetTime(CommonSE_Proto.E_TIMEOFDAY time)
	{
		float Time = 0.0f;
		switch (time)
		{
			case CommonSE_Proto.E_TIMEOFDAY.noon:
				Time = noonTime;
				break;
			case CommonSE_Proto.E_TIMEOFDAY.night:
				Time = nightTime;
				break;
			default:
				Time = 0.0f;
				break;
		}
		return Time;
	}

	/// <summary>
	/// 現在時間の取得
	/// </summary>
	/// <returns></returns>
	public float GetCurrentTime()
	{
		return time;
	}
}
