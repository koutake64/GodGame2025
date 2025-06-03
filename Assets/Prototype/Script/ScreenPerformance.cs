using UnityEngine;
using UnityEngine.UI;

public class ScreenPerformance : MonoBehaviour
{


    private TimeManager timeMng;
    private TextManager textMng;
    private UIManager uiMng;
    private GameObject backgroundPanel;

    private float noonTimeStart;
    private float nightTimeStart;
    private bool isSwitchNoon = false;
    private bool isSwitchNight = false;
    private float gameTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeMng = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if(!timeMng)
        {
            Debug.LogError("Script:Screenperformance.cs \n" +
              "TimeManagerがnullです");
        }

        textMng = GameObject.Find("TextTyper").GetComponent<TextManager>();
        if (!textMng)
            Debug.Log("Script:Screenperformance.cs \n" +
              "TextManagerがnullです");

        uiMng = GameObject.Find("UIManager").GetComponent<UIManager>();
        if (!uiMng)
            Debug.Log("Script:Screenperformance.cs \n" +
              "UIManagerがnullです");
        backgroundPanel = GameObject.Find("BackGroundPanel");

        // 各演出開始時間の取得
        noonTimeStart = timeMng.GetTime(CommonSE_Proto.E_TIMEOFDAY.noon) - 5.0f;
        nightTimeStart = timeMng.GetTime(CommonSE_Proto.E_TIMEOFDAY.night) - 5.0f;
    }

    /// <summary>
    /// 画面演出の開始
    /// </summary>
    void StartPerformance()
    {
        uiMng.SetUIActive(UIManager.E_UI_KIND.backPanel, true);
        // TODO:メモテクスチャの表示

        timeMng.SetTimeScale(0.0f);
        uiMng.SetUIActive(UIManager.E_UI_KIND.backPanel, false);
        backgroundPanel.SetActive(true);
        textMng.StartTalk(2, false);
    }
}
