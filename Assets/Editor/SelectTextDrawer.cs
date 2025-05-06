using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;


#if UNITY_EDITOR	// エディタ上のみ有効

// カスタムプロパティクラス
[CustomPropertyDrawer(typeof(SelectTextAttribute))]
public class SelectTextDrawer : PropertyDrawer
{
	// インスペクタ上でのGUI描画処理
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Resourcesフォルダ内のすべての.text/.txtファイルを取得
		string[] guids = AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets/Resources" });
		string[] names = guids
			.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
			.Where(path => path.EndsWith(".txt") || path.EndsWith(".text"))	// .trxt/.textのみに絞る
			.Select(path => Path.GetFileNameWithoutExtension(path))			// ファイル名を探索
			.Distinct()
			.ToArray();

		if (names.Length == 0)
		{
			EditorGUI.HelpBox(position, "Resourcesフォルダに.txt/.textファイルが見つかりません", MessageType.Warning);
			return;
		}

		int currentIndex = Mathf.Max(0, System.Array.IndexOf(names, property.stringValue));
		int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, names);

		property.stringValue = names[selectedIndex];
	}
}
#endif
