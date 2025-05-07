#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SelectTagAttribute))]
public class SelectTagDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType == SerializedPropertyType.String)
		{
			// タグ取得
			string currentTag = property.stringValue;
			string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
			int index = Mathf.Max(0, System.Array.IndexOf(tags, currentTag));

			// プルダウンメニューを表示
			int selectedIndex = EditorGUI.Popup(position, label.text, index, tags);
			property.stringValue = tags[selectedIndex];
		}
		else
		{
			EditorGUI.LabelField(position, label.text, "Use SelectTag with string.");
		}
	}
}
#endif
