using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
	private TimeManager timeManager;    // タイムマネージャー
	private _FieldDataManager fdMng;
	// TOdo 後で直す
	[SerializeField, SceneSelector] private string a;
	

	private void Start()
    {
		timeManager = FindAnyObjectByType<TimeManager>();
		fdMng = FindAnyObjectByType<_FieldDataManager>();
	}

    private void FixedUpdate()
    {
        
    }

	private void LateUpdate()
	{
        if (fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.goal).Count <= 0)
        {
            return;
        }

        // ゴールの位置を取得
        List<Vector2Int> goalPositions = fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.goal);
		
		Vector2Int goalPos = goalPositions[0];
		//Debug.Log("ゴール" +  goalPos);

		// 夜のみチェック
		if (timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.night)
		{
			GameObject princessObj = GameObject.FindWithTag("Princess");

			if (princessObj == null)
			{
				Debug.Log("princessのタグを取得できん！");
				return;
			}

			// お嬢様の現在位置を取得
			//List<Vector2Int> princessPositions = fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.princess);

			Vector3 pos = princessObj.transform.position;
			Vector2Int pripos = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));

			
			// ゴールに到達しているか判定
			if (pripos == goalPos)
			{
				Debug.Log("お嬢様がゴール");
				SceneChanger.ChangeScene(a);
			}

			
		}

		
	}
}