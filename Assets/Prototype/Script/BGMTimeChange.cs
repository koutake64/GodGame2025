using UnityEngine;

public class BGMTimeManager : MonoBehaviour
{
    private TimeManager timeManager;
    private CommonSE_Proto.E_TIMEOFDAY currentTimeState = CommonSE_Proto.E_TIMEOFDAY.morning;

    private bool once = true;

    private void Start()
    {
        timeManager = FindFirstObjectByType<TimeManager>();
        if (timeManager == null)
        {
            Debug.LogError("TimeManager‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ");
            return;
        }

        currentTimeState = timeManager.CurrentState;
        PlayBGMByTime(currentTimeState);
    }

    private void Update()
    {
        if (timeManager == null) return;

        var newState = timeManager.CurrentState;

        if (newState != currentTimeState || once)
        {
            currentTimeState = newState;
            PlayBGMByTime(newState);
            once = false;
        }
    }

    private void PlayBGMByTime(CommonSE_Proto.E_TIMEOFDAY timeOfDay)
    {
        switch (timeOfDay)
        {
            case CommonSE_Proto.E_TIMEOFDAY.morning:
                AudioManager.Instance.PlayBGM(0);
                break;
            case CommonSE_Proto.E_TIMEOFDAY.noon:
                AudioManager.Instance.PlayBGM(1);
                break;
            case CommonSE_Proto.E_TIMEOFDAY.afternoon:
                AudioManager.Instance.PlayBGM(1);
                break;
            case CommonSE_Proto.E_TIMEOFDAY.night:
                AudioManager.Instance.PlayBGM(2);
                break;
        }
    }
}
