using UnityEngine;
using System.Collections.Generic;

public class ModelShiftByTime : MonoBehaviour
{
    private TimeManager timeMng = null;

    [Header("本体のゲームオブジェクト")]
    [Header("各時間帯のモデル(nullの場合は非アクティブになる)")]
    [Header("朝"), SerializeField] private GameObject morningModel = null;
    [Header("昼"), SerializeField] private GameObject noonModel = null;
    [Header("夕"), SerializeField] private GameObject afternoonModel = null;
    [Header("夜"), SerializeField] private GameObject nightModel = null;

    private GameObject[] models = new GameObject[CommonSE_Proto.maxTimeOfDay];

    private bool startOnce = true;

    private _FieldDataManager fdMng;

    private void Start()
    {
        timeMng = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if (!timeMng)
        {
            Debug.Log(
                "Script : ModelShiftByTime.cs \n" + 
                "TimeManagerがセットされていません"
                );
        }

        fdMng = GameObject.Find("Field").GetComponent<_FieldDataManager>();
        if (!fdMng)
        {
            Debug.Log(
                "Script : ModelShiftByTime.cs \n" +
                "_FieldDataManagerが見つかりません"
                );
        }

        models[(int)CommonSE_Proto.E_TIMEOFDAY.morning] = morningModel;
        models[(int)CommonSE_Proto.E_TIMEOFDAY.noon] = noonModel;
        models[(int)CommonSE_Proto.E_TIMEOFDAY.afternoon] = afternoonModel;
        models[(int)CommonSE_Proto.E_TIMEOFDAY.night] = nightModel;

    }

    private void LateUpdate()
    {
        // --- 時間帯が変わった時だけ処理を行う
        if (!timeMng.IsChangeState() && !startOnce)
        {
            return;
        }

        startOnce = false;

        // --- 各モデルをすべて非アクティブにする
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

	public void Refresh()
	{
		startOnce = true; // 次の LateUpdate でモデルを再評価させる
	}


}
