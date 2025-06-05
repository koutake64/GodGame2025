using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

public static class HierarchyEditor {

	private const float Width = 16f;
	private const float Height = 16f;
	private static readonly Color DisabledColor = new Color(1f, 1f, 1f, 0.5f);

	[InitializeOnLoadMethod]
	private static void Initialize() {
		EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
	}

	private static void OnGUI(int instanceID, Rect selectionRect) {
		ToggleActive(instanceID, selectionRect, 0f);
		ToggleComponent(instanceID, selectionRect, Width);
	}

	private static void ToggleActive(int instanceID, Rect selectionRect, float pivotX) {
		GameObject g = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
		if (g == null) return;

		Rect rect = selectionRect;
		rect.x = rect.xMax - Width - pivotX;
		rect.width = Width;

		bool active = GUI.Toggle(rect, g.activeSelf, string.Empty);
		if (active == g.activeSelf) return;
		g.SetActive(active);

		GameObject[] array = Selection.gameObjects;
		if (array == null
			|| array.Length <= 1) {
			return;
		}
		for (int i = 0, iMax = array.Length; i < iMax; ++i) {
			if (array[i] == null
				|| array[i].activeSelf == active) {
				continue;
			}
			array[i].SetActive(active);
		}
	}

	private static void ToggleComponent(int instanceID, Rect selectionRect, float pivotX) {
		GameObject g = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
		if (g == null) return;

		Rect rect = selectionRect;
		rect.x = rect.xMax - Width - pivotX;
		rect.width = Width;
		rect.height = Height;

		IEnumerable<Component> components = g
			.GetComponents<Component>()
			.Where(c => c != null)
			.Where(c => !(c is Transform))
			.Reverse();

//		Event current = Event.current;

		foreach (Component c in components) {
			Texture image = AssetPreview.GetMiniThumbnail(c);

			if (image == null
				&& c is MonoBehaviour) {
				MonoScript ms = MonoScript.FromMonoBehaviour(c as MonoBehaviour);
				string path = AssetDatabase.GetAssetPath(ms);
				image = AssetDatabase.GetCachedIcon(path);
			}

			if (image == null) continue;

//			if (current.type == EventType.MouseDown
//			    && rect.Contains( current.mousePosition )) {
//				c.SetEnable(!c.IsEnabled());
//			}

			Color color = GUI.color;
			GUI.color = c.IsEnabled() ? Color.white : DisabledColor;
			GUI.DrawTexture(rect, image, ScaleMode.ScaleToFit);
			GUI.color = color;
			rect.x -= rect.width;
		}
	}

	public static bool IsEnabled(this Component self) {
		if (self == null) return true;

		Type type = self.GetType();
		PropertyInfo property = type.GetProperty("enabled", typeof(bool));

		if (property == null) {
			return true;
		}

		return (bool)property.GetValue(self, null);
	}

	public static void SetEnable(this Component self, bool isEnabled) {
		if (self == null) return;

		Type type = self.GetType();
		PropertyInfo property = type.GetProperty("enabled", typeof(bool));

		if (property == null) return;

		property.SetValue(self, isEnabled, null);
	}

}
