using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
	[SerializeField, SceneSelector, Header("クリアシーン")] private string a;
	
// Hide--------------------------------------------------------------------------------------------------------------------------
	private TimeManager timeManager;    // タイムマネージャー
	private _FieldDataManager fieldManager;
	

	private void Start()
    {
		timeManager = FindAnyObjectByType<TimeManager>();
		fieldManager = FindAnyObjectByType<_FieldDataManager>();
	}

	private void LateUpdate()
	{
        if (fieldManager.GetStatePos(_FieldDataManager.E_FIELDSTATE.goal).Count <= 0)
        {
            return;
        }

        // ゴールの位置を取得
        List<Vector2Int> goalPositions = fieldManager.GetStatePos(_FieldDataManager.E_FIELDSTATE.goal);
		
		Vector2Int goalPos = goalPositions[0];
		
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
			Vector2Int princessPos = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));

			
			// ゴールに到達しているか判定
			if (princessPos == goalPos)
			{
				Debug.Log("お嬢様がゴール");
				SceneChanger.ChangeScene(a);
			}

			
		}

		
	}
}