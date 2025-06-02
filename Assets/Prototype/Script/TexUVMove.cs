using UnityEngine;
using UnityEngine.UI;

public class TexUVMove : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField, Header("切り替わる時間")] private float lerpTime = 1.0f;


    private TimeManager timeManager;

    private float noonTimeStart;    // 昼前演出開始時間
    private float nightTimeStart;   // 夜前演出開始時間
    private float gameTime;         // ゲーム内の時間
    private float currenttime;      // 現在の時間

    private Rect startRect;
    private Rect targetRect;
    private int currentIndex = 0;
    private const int totalState = 3;   // 朝・昼・夜

    private bool isTransition;
    private bool isSwitchedToNoon = false;
    private bool isSwitchedToNight = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeManager = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if(!timeManager)
        {
            Debug.LogError("Script:TexUVmove.cs \n" +
               "TimeManagerがnullです");
        }

        // それぞれの演出開始時間をTimeManagerから取得する
        noonTimeStart = timeManager.GetTime(CommonSE_Proto.E_TIMEOFDAY.noon) - 5.0f;
        nightTimeStart = timeManager.GetTime(CommonSE_Proto.E_TIMEOFDAY.night) - 5.0f;

        // 最初は朝にしておく
        SetUVIndex(0, true);
    }

    // Update is called once per frame
    void Update()
    {
        // ゲーム中の時間を更新する
        gameTime = timeManager.GetCurrentTime();

        if(isTransition)
        {
            currenttime += Time.deltaTime;
            float t = Mathf.Clamp01(currenttime / lerpTime);

            image.uvRect = LerpRect(startRect, targetRect, t);
            if (t >= 1.0f)
                isTransition = false;
        }

        // 昼前の演出開始
        if (gameTime >= noonTimeStart && !isSwitchedToNoon)
        {
            // 昼に移行
            SetUVIndex(1, false);
            isSwitchedToNoon = true;
        }

        if(gameTime >= nightTimeStart && !isSwitchedToNight)
        {
            SetUVIndex(2, false);
            isSwitchedToNight = true;
        }
    }

    /// <summary>
    /// UVの線形補間の値のセット
    /// </summary>
    /// <param name="index"></param>
    /// <param name="immdiate"></param>
    void SetUVIndex(int index,bool immdiate)
    {
        currentIndex = index;
        float uvWidth = 1.0f / (float)totalState;
        Rect newRect = new Rect(index * uvWidth, 0f, uvWidth, 1f);

        if(immdiate)
        {
            image.uvRect = newRect;
            isTransition = false;
        }
        else
        {
            startRect = image.uvRect;
            targetRect = newRect;
            currenttime = 0f;
            isTransition = true;
        }
    }

    /// <summary>
    /// Rect型の線形補間
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    Rect LerpRect(Rect from,Rect to,float t)
    {
        return new Rect(
            Mathf.Lerp(from.x, to.x, t),
            Mathf.Lerp(from.y, to.y, t),
            Mathf.Lerp(from.width, to.width, t),
            Mathf.Lerp(from.height, to.height, t));
    }
}
