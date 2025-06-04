using UnityEngine;
using UnityEngine.UI;

public class ScreenPerformance : MonoBehaviour
{
    public enum E_PerformanceTime
    {
        Noon,
        Night,
    }

    private TimeManager timeMng;
    private TextManager textMng;
    private UIManager uiMng;
    private GameObject backgroundPanel;
    [SerializeField,Header("メモを表示する時間")]public float waitFrame = 5.0f;
    private float frame = 0.0f;
    private E_PerformanceTime curPerTime;

    private bool isPerformance;     // 演出中
    private bool isNext = false;

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
        backgroundPanel = GameObject.Find("BackgroundPanel");
        if(!backgroundPanel)
        {
            Debug.Log("Script:Screenperformance.cs \n" +
             "backgroundがnullです");
        }

        
    }

    private void Update()
    {
        switch(curPerTime)
        {
            case E_PerformanceTime.Noon:
                frame += Time.unscaledDeltaTime;
                if (isPerformance)
                {
                    if (frame >= waitFrame && !isNext)
                    {
                        isNext = true;
                        NextPerformance();
                    }
                }

                if (isPerformance && textMng.talkFlg == false && isNext && frame >= waitFrame)
                {
                    isPerformance = false;
                    uiMng.SetUIActive(UIManager.E_UI_KIND.perforMemo, false);
                    timeMng.SetTimeScale(1.0f);
                }
                break;
            case E_PerformanceTime.Night:
                if(isPerformance && textMng.talkFlg == false)
                {
                    isPerformance = false;
                    timeMng.SetTimeScale(1.0f);
                }
                break;
        }

        
    }
    /// <summary>
    /// 画面演出の開始
    /// </summary>
    public void StartPerformance(E_PerformanceTime time)
    {
        // 演出開始
        isPerformance = true;
        curPerTime = time;
        switch(time)
        {
            case E_PerformanceTime.Noon:
                frame = 0.0f;
                uiMng.SetUIActive(UIManager.E_UI_KIND.backPanel, true);
                // メモの表示
                uiMng.SetUIActive(UIManager.E_UI_KIND.perforMemo, true);
                break;
            case E_PerformanceTime.Night:
                backgroundPanel.SetActive(true);
                textMng.StartTalk(3, true);
                break;
        }
        
        
    }

    private void NextPerformance()
    {
        uiMng.SetUIActive(UIManager.E_UI_KIND.backPanel, false);
        backgroundPanel.SetActive(true);
        textMng.StartTalk(2, false);
    }

    public bool GetIsPerformance()
    {
        return isPerformance;
    }
}
