using UnityEngine;

public class ModelShiftByTime : MonoBehaviour
{
    private TimeManager timeMng = null;

    //[Header("対象タグ")]
    //[SerializeField, SelectTag] private string tag;

    [Header("本体のゲームオブジェクト")]
    [SerializeField] private GameObject mainObj = null;

    [Header("各時間帯のモデル(nullの場合は非アクティブになる)")]
    [Header("朝"), SerializeField] private GameObject morningModel = null;
    [Header("昼"), SerializeField] private GameObject noonModel = null;
    [Header("夕"), SerializeField] private GameObject afternoonModel = null;
    [Header("夜"), SerializeField] private GameObject nightModel = null;

    private GameObject[] models = new GameObject[CommonSE_Proto.maxTimeOfDay];

    private bool startOnce = true;

    private void Start()
    {
        if (!mainObj)
        {
            Debug.Log(
                "Script : ModelShiftByTime.cs \n" +
                "本体のオブジェクトがセットされていません"
                );
        }

        timeMng = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if (!timeMng)
        {
            Debug.Log(
                "Script : ModelShiftByTime.cs \n" + 
                "TimeManagerがセットされていません"
                );
        }

        models[(int)CommonSE_Proto.E_TIMEOFDAY.morning] = morningModel;
        models[(int)CommonSE_Proto.E_TIMEOFDAY.noon] = noonModel;
        models[(int)CommonSE_Proto.E_TIMEOFDAY.afternoon] = afternoonModel;
        models[(int)CommonSE_Proto.E_TIMEOFDAY.night] = nightModel;

    }

    private void Update()
    {
        // --- 時間帯が変わった時だけ処理を行う
        if (!timeMng.IsChangeState() && !startOnce)
        {
            return;
        }

        startOnce = false;

        // --- 本体と各モデルをすべて非アクティブにする
        mainObj.SetActive(false);
        foreach (var model in models)
        {
            if (!model)
            {
                continue;
            }

            model.SetActive(false);
        }

        // --- 時間帯にあったモデルをアクティブにする
        ModelActive(timeMng.GetCurState());

        // --- モデルが1つでもアクティブなら本体オブジェクトをアクティブにする
        foreach (var model in models)
        {
            if (!model)
            {
                continue;
            }

            if (model.activeSelf)
            {
                mainObj.SetActive(true);
                return;
            }

        }

        
    }

    private void ModelActive(CommonSE_Proto.E_TIMEOFDAY time)
    {
        Debug.Log("今は" + time + "です");

        if (models[(int)time])
        {
            models[(int)time].SetActive(true);
            Debug.Log(time + "のモデルがアクティブになりました");
        }
    }

}
