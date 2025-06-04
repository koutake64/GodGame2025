using UnityEngine;
using UnityEngine.UI;

public class ScreenPerformance : MonoBehaviour
{
    private TimeManager timeMng;
    private TextManager textMng;
    private UIManager uiMng;
    private GameObject backgroundPanel;
    [SerializeField,Header("メモを表示する時間")]public float waitFrame = 5.0f;
    private float frame = 0.0f;

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
        frame += Time.unscaledDeltaTime;
        if(isPerformance)
        {
            if(frame >= waitFrame && !isNext)
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
    }
    /// <summary>
    /// 画面演出の開始
    /// </summary>
    public void StartPerformance()
    {
        isPerformance = true;
        frame = 0.0f;
        uiMng.SetUIActive(UIManager.E_UI_KIND.backPanel, true);
        // TODO:メモテクスチャの表示
        uiMng.SetUIActive(UIManager.E_UI_KIND.perforMemo, true);
        
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
