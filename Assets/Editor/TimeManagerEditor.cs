using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TimeManager))]
public class TimeManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		TimeManager manager = (TimeManager)target;

		GUILayout.Space(10);
		GUILayout.Label("デバッグ用 時間帯切替", EditorStyles.boldLabel);

		if (GUILayout.Button("朝に切り替え"))
		{
			manager.ForceChangeState(CommonSE_Proto.E_TIMEOFDAY.morning);
		}

		if (GUILayout.Button("昼に切り替え"))
		{
			manager.ForceChangeState(CommonSE_Proto.E_TIMEOFDAY.noon);
		}

		if (GUILayout.Button("夕方に切り替え"))
		{
			manager.ForceChangeState(CommonSE_Proto.E_TIMEOFDAY.afternoon);
		}

		if (GUILayout.Button("夜に切り替え"))
		{
			manager.ForceChangeState(CommonSE_Proto.E_TIMEOFDAY.night);
		}
	}
}
