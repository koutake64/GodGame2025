using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MenuPanelManager : MonoBehaviour
{
    public struct S_PAGEINFO
    {
        public int id;
        public int page;
        public int maxPage;
    }

    [System.Serializable]
    public struct S_PANELLIST
    {
        public List<GameObject> panelList;
    }

    [SerializeField] private StatusManager statusMng;
    [SerializeField] private StageInfo stageInfo;
    [SerializeField] private DataManager dataMng;

    [SerializeField] private TMP_Text stageSizeX;
    [SerializeField] private TMP_Text stageSizeY;
    [SerializeField] private TMP_Text stageSwitchTimeNoon;
    [SerializeField] private TMP_Text stageSwitchTimeAfternoon;
    [SerializeField] private TMP_Text stageSwitchTimeNight;

    [SerializeField] private GameObject nextButtonObject;
    [SerializeField] private GameObject backButtonObject;
    [SerializeField] private GameObject completeButtonObject;

    [SerializeField] private List<S_PANELLIST> allPanel;

    [SerializeField] private GameObject erorrPanel;

    private S_PAGEINFO info;

    private Button nextButton;
    private Button backButton;
    private Button completeButton;

    private bool onceActiveChangeFlag;

    private void Start()
    {
        info = new S_PAGEINFO();
        info.id = 0;
        info.page = 0;
        info.maxPage = 0;

        if (nextButtonObject)
        {
            nextButton = nextButtonObject.GetComponent<Button>();
        }

        if (backButtonObject)
        {
            backButton = backButtonObject.GetComponent<Button>();
        }

        if (completeButtonObject)
        {
            completeButton = completeButtonObject.GetComponent<Button>();
        }

    }

    private void Update()
    {
        if (onceActiveChangeFlag)
        {
            return;
        }

        if (info.id == 0)
        {
            nextButtonObject.SetActive(false);
            backButtonObject.SetActive(false);
            completeButtonObject.SetActive(false);

            return;
        }
        else
        {
            nextButtonObject.SetActive(true);
            backButtonObject.SetActive(true);
            completeButtonObject.SetActive(true);
        }

        if (info.page < info.maxPage)
        {
            nextButton.interactable = true;
        }
        else
        {
            nextButton.interactable = false;
        }

        if (info.page > 0)
        {
            backButton.interactable = true;
        }
        else
        {
            backButton.interactable = false;
        }

        if (info.page == info.maxPage)
        {
            completeButton.interactable = true;
        }
        else
        {
            completeButton.interactable = false;
        }
        
        for (int i = 0; i < allPanel.Count; ++i)
        {
            for (int j = 0; j < allPanel[i].panelList.Count; ++j)
            {
                allPanel[i].panelList[j].SetActive(false);
            }
        }

        allPanel[info.id].panelList[info.page].SetActive(true);

        onceActiveChangeFlag = true;

    }

    public void OnClickNext()
    {
        onceActiveChangeFlag = false;
        info.page++;
    }

    public void OnClickBack()
    {
        onceActiveChangeFlag = false;
        info.page--;
    }

    public void OnClickComplete()
    {
        onceActiveChangeFlag = false;

        for (int i = 0; i < allPanel.Count; ++i)
        {
            for (int j = 0; j < allPanel[i].panelList.Count; ++j)
            {
                allPanel[i].panelList[j].SetActive(true);
            }
        }

        if (info.id == 1)
        {
            stageInfo.SetSize(
                TMPToInt(stageSizeX),
                TMPToInt(stageSizeY)
                );
            stageInfo.SetSwitchTime(
                TMPToInt(stageSwitchTimeNoon),
                TMPToInt(stageSwitchTimeAfternoon),
                TMPToInt(stageSwitchTimeNight)
                );
            dataMng.CreateNewData();
        }

        statusMng.GenerateButton();
        this.gameObject.SetActive(false);

    }

    public void OnClickNewStage()
    {
        onceActiveChangeFlag = false;
        info.id = 1;
        info.page = 0;
        info.maxPage = 1;
    }

    private int TMPToInt(TMP_Text tmp)
    {
        int value = 0;
        string str = "";
        char[] c = tmp.text.ToCharArray();
        for (int i = 0; i < c.Length - 1; ++i)
        {
            str += c[i];
        }
        value = int.Parse(str);
        return value;
    }

}
