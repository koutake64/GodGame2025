using UnityEditor;
using UnityEngine;

public class ObjectDuplication : EditorWindow
{
	private GameObject selectedObject;
	private Vector3 duplicationOffset = Vector3.zero;
	private int duplicationCount = 1;

	[MenuItem("TakelabTools/Duplicate with Options", false, 0)]
	private static void ShowWindow()
	{
		ObjectDuplication window = GetWindow<ObjectDuplication>("Duplicate Objects");
		window.Show();
	}

	private void OnGUI()
	{
		GUILayout.Label("Duplicate Options", EditorStyles.boldLabel);

		selectedObject = Selection.activeGameObject;

		if (selectedObject == null)
		{
			EditorGUILayout.HelpBox("複製するオブジェクトを選択してください！", MessageType.Warning);
			return;
		}

		EditorGUILayout.LabelField("オブジェクト:", selectedObject.name);

		duplicationOffset = EditorGUILayout.Vector3Field("ずらす位置:", duplicationOffset);

		duplicationCount = EditorGUILayout.IntField("複製数:", duplicationCount);
		duplicationCount = Mathf.Max(1, duplicationCount);

		if (GUILayout.Button("確定"))
		{
			DuplicateObjects();
		}
	}

	private void DuplicateObjects()
	{
		if (selectedObject == null)
		{
			Debug.LogError("複製するオブジェクトが選択されていません。");
			return;
		}

		for (int i = 1; i <= duplicationCount; i++)
		{
			GameObject duplicate = Instantiate(selectedObject);
			duplicate.name = selectedObject.name + "_Copy" + i;
			duplicate.transform.position = selectedObject.transform.position + duplicationOffset * i;
			duplicate.transform.rotation = selectedObject.transform.rotation;
			duplicate.transform.parent = selectedObject.transform.parent;

			Undo.RegisterCreatedObjectUndo(duplicate, "Duplicate Object");
		}

		Debug.Log($"Duplicated {duplicationCount} copies of {selectedObject.name}.");
	}
}
