using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneSelectorAttribute))]
public class SceneSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            var scenes = EditorBuildSettings.scenes;
            var sceneNames = new string[scenes.Length];
            int currentIndex = -1;

            // 登録されているシーン取得
            for (int i = 0; i < scenes.Length; i++)
            {
                sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(scenes[i].path);
                if (property.stringValue == sceneNames[i])
                {
                    currentIndex = i;
                }
            }

            // プルダウンリスト作成
            int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, sceneNames);

            // シーン名をプロパティ設定
            if (selectedIndex >= 0 && selectedIndex < sceneNames.Length)
            {
                property.stringValue = sceneNames[selectedIndex];
            }
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.HelpBox(position, "SceneSelector属性はstring型のプロパティにのみ使用可能。", MessageType.Error);
        }
    }
}
