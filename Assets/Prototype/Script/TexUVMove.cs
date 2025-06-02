using UnityEngine;
using UnityEngine.UI;

public class TexUVMove : MonoBehaviour
{
    [SerializeField] private RawImage image;
    public float TexScaleX = 0.32f;
    [SerializeField, Range(0.0f, 0.66f)] float UVX = 0.0f;
    [SerializeField, Header("切り替わる時間")] private float lerpTime = 1.0f;


    private TimeManager timeManager;

    private float noonTimeStart;    // 昼前演出開始時間
    private float nightTimeStart;   // 夜前演出開始時間
    private float curTime;          // 現在の時間
    

    private Vector2 offset;
    private Vector2[] StartUV =
    {
        new Vector2(0.0f / 3.0f, 0.0f),
        new Vector2(1.0f / 3.0f, 0.0f),
    };
    private Vector2[] targetUV =
    {
        new Vector2(0.33f,0.0f),
        new Vector2(0.66f,0.0f),
    };

    private bool isTransition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeManager = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if(!timeManager)
        {
            Debug.LogError("Script:TexUVmove.cs \n" +
               "TimeManagerがnullです");
        }

        Material mat = Instantiate(image.material);
        image.material = mat;

        mat.mainTextureOffset = new Vector2(0.0f / 3.0f, 0.0f);
        mat.mainTextureScale = new Vector2(0.32f, 1.0f);

        noonTimeStart = timeManager.GetTime(CommonSE_Proto.E_TIMEOFDAY.noon) - 5.0f;
        nightTimeStart = timeManager.GetTime(CommonSE_Proto.E_TIMEOFDAY.night) - 5.0f;

        Debug.Log("noonStart:" + noonTimeStart);
        Debug.Log("nightStart:" + nightTimeStart);

        offset = image.material.mainTextureOffset;
    }

    // Update is called once per frame
    void Update()
    {
        curTime = timeManager.GetCurrentTime();

        if (curTime >= noonTimeStart && !isTransition)
        {
            isTransition = true;
        }

        if (isTransition)
        {
            float t = Mathf.Clamp01(curTime / lerpTime);

            offset.x = Mathf.Lerp(StartUV[0].x, targetUV[0].x, t);

            image.material.mainTextureOffset = new Vector2(offset.x, 0.0f);

            if (t >= 1.0f)
                isTransition = false;
        }
    }
}
