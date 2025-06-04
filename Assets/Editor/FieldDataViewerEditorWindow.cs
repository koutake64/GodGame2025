using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FieldDataViewerEditorWindow : EditorWindow
{
	private _FieldDataManager manager;

	[MenuItem("Tools/Field Data Viewer")]
	public static void ShowWindow()
	{
		GetWindow<FieldDataViewerEditorWindow>("Field Data Viewer");
	}

	private Vector2 scroll;

	private void OnGUI()
	{
		GUILayout.Label("Field Data Viewer", EditorStyles.boldLabel);

		manager = (_FieldDataManager)EditorGUILayout.ObjectField("FieldDataManager", manager, typeof(_FieldDataManager), true);

		if (manager == null)
		{
			EditorGUILayout.HelpBox("FieldDataManagerをシーンから割り当ててください。", MessageType.Info);
			return;
		}

		Vector2Int fieldSize = manager.GetFieldSize();
		scroll = EditorGUILayout.BeginScrollView(scroll);

		for (int y = fieldSize.y - 1; y >= 0; y--)
		{
			EditorGUILayout.BeginHorizontal();

			for (int x = 0; x < fieldSize.x; x++)
			{
				List<_FieldDataManager.S_FIELDINFO> infos = manager.GetInfoList(new Vector2Int(x, y));
				if (infos.Count == 0)
				{
					GUILayout.Label("・", GUILayout.Width(30));
					continue;
				}

				string label = "";
				foreach (var info in infos)
				{
					label += $"{info.state.ToString().Substring(0, 2)}\n";
				}

				GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
				boxStyle.alignment = TextAnchor.MiddleCenter;
				boxStyle.fontSize = 8;

				GUILayout.Box(label, boxStyle, GUILayout.Width(30), GUILayout.Height(30));
			}

			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndScrollView();
	}
}
