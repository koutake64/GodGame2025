using UnityEngine;
using UnityEditor;
using System;

namespace SHADERGEAR.OutlineEngine.Editor
{
    [CustomEditor(typeof(OutlineEngine))]
    public class OutlineEditor : UnityEditor.Editor
    {
        [SerializeField] Texture2D outlineIcon, overlayIcon;

        OutlineEngine Engine => target as OutlineEngine;

        static string GetBackingField(string propertyName) => $"<{propertyName}>k__BackingField";

        void DrawProperty(string parentPropertyName, string childPropertyName, GUIContent content = null)
        {
            var parent = serializedObject.FindProperty(GetBackingField(parentPropertyName)); 
            var child = parent.FindPropertyRelative(GetBackingField(childPropertyName)); 

            EditorGUILayout.PropertyField(child, content);
        }

        public override void OnInspectorGUI()
        {
            DrawToolbar();

            switch(Engine.Tab)
            {
                case 0:
                    DrawRenderingTab();
                    break;
                
                case 1:
                    DrawOutlineTab();
                    break;

                case 2:
                    DrawOverlayTab();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Engine.Tab));
            }
        }

        void DrawToolbar()
        {
            var renderingIcon = EditorGUIUtility.IconContent("d_SceneViewCamera").image;
            var darkTheme = EditorGUIUtility.isProSkin;

            var tabs = new GUIContent[]
            {
                new GUIContent("Rendering", darkTheme ? renderingIcon : null),
                new GUIContent("Outline", darkTheme ? outlineIcon : null),
                new GUIContent("Overlay", darkTheme ? overlayIcon : null)
            };

            Engine.Tab = GUILayout.Toolbar(Engine.Tab, tabs);
            EditorGUILayout.Separator();
        }

        void DrawRenderingTab()
        {
            EditorGUILayout.LabelField(new GUIContent("Rendering", "Settings that affect how outlines and overlays are rendered."), EditorStyles.boldLabel);

            DrawProperty(nameof(Engine.Rendering), nameof(Engine.Rendering.Targets));

            DrawProperty(nameof(Engine.Rendering), nameof(Engine.Rendering.Occlusion));
            if (Engine.Rendering.Occlusion != RenderingOcclusion.RenderEverything)
            {
                EditorGUI.indentLevel++;
                DrawProperty(nameof(Engine.Rendering), nameof(Engine.Rendering.Occluders));
                EditorGUI.indentLevel--;
            }
            
            DrawProperty(nameof(Engine.Rendering), nameof(Engine.Rendering.Event));

            DrawProperty(nameof(Engine.Rendering), nameof(Engine.Rendering.Cutout));
            if (Engine.Rendering.Cutout)
            {
                EditorGUI.indentLevel++;
                DrawProperty(nameof(Engine.Rendering), nameof(Engine.Rendering.CutoutTexture));
                DrawProperty(nameof(Engine.Rendering), nameof(Engine.Rendering.CutoutThreshold));
                DrawProperty(nameof(Engine.Rendering), nameof(Engine.Rendering.CutoutChannel));
                EditorGUI.indentLevel--;
            }
        }

        void DrawOutlineTab()
        {
            EditorGUILayout.LabelField(new GUIContent("Outline", "Settings that affect how the outline looks like."), EditorStyles.boldLabel);

            DrawProperty(nameof(Engine.Outline), nameof(Engine.Outline.Style));

            switch(Engine.Outline.Style)
            {
                case OutlineStyle.None:
                    break;

                case OutlineStyle.Color:
                    DrawProperty(nameof(Engine.Outline), nameof(Engine.Outline.Width));
                    DrawProperty(nameof(Engine.Outline), nameof(Engine.Outline.Color));
                    DrawProperty(nameof(Engine.Outline), nameof(Engine.Outline.Softness));
                    break;

                case OutlineStyle.Texture:
                    DrawProperty(nameof(Engine.Outline), nameof(Engine.Outline.Width));
                    DrawProperty(nameof(Engine.Outline), nameof(Engine.Outline.Color));
                    DrawProperty(nameof(Engine.Outline), nameof(Engine.Outline.Softness));

                    EditorGUILayout.LabelField(new GUIContent("Texture", "Settings that affect how the outline's texture looks like."), EditorStyles.boldLabel);
                    DrawProperty(nameof(Engine.Outline), nameof(Engine.Outline.TextureSource), new("Source"));
                    DrawProperty(nameof(Engine.Outline), nameof(Engine.Outline.TextureScale), new("Scale"));
                    DrawProperty(nameof(Engine.Outline), nameof(Engine.Outline.TextureOffset), new("Offset"));
                    DrawProperty(nameof(Engine.Outline), nameof(Engine.Outline.TextureRotation), new("Rotation"));
                    EditorGUILayout.LabelField(new GUIContent("Animation", "Settings that affect how the outline's texture is animated."), EditorStyles.boldLabel);
                    DrawProperty(nameof(Engine.Outline), nameof(Engine.Outline.TextureAnimationSpeed), new("Speed"));
                    DrawProperty(nameof(Engine.Outline), nameof(Engine.Outline.TextureAnimationDirection), new("Direction"));
                    break;
            }
        }

        void DrawOverlayTab()
        {
            EditorGUILayout.LabelField(new GUIContent("Overlay", "Settings that affect how the overlay looks like."), EditorStyles.boldLabel);

            DrawProperty(nameof(Engine.Overlay), nameof(Engine.Overlay.Style));

            switch(Engine.Overlay.Style)
            {
                case OverlayStyle.None:
                    break;

                case OverlayStyle.Color:
                    DrawProperty(nameof(Engine.Overlay), nameof(Engine.Overlay.Color));
                    break;

                case OverlayStyle.Texture:
                    DrawProperty(nameof(Engine.Overlay), nameof(Engine.Overlay.Color));

                    EditorGUILayout.LabelField(new GUIContent("Texture", "Settings that affect how the overlays's texture looks like."), EditorStyles.boldLabel);
                    DrawProperty(nameof(Engine.Overlay), nameof(Engine.Overlay.TextureSource), new("Source"));
                    DrawProperty(nameof(Engine.Overlay), nameof(Engine.Overlay.TextureScale), new("Scale"));
                    DrawProperty(nameof(Engine.Overlay), nameof(Engine.Overlay.TextureOffset), new("Offset"));
                    DrawProperty(nameof(Engine.Overlay), nameof(Engine.Overlay.TextureRotation), new("Rotation"));
                    EditorGUILayout.LabelField(new GUIContent("Animation", "Settings that affect how the outline's texture is animated."), EditorStyles.boldLabel);
                    DrawProperty(nameof(Engine.Overlay), nameof(Engine.Overlay.TextureAnimationSpeed), new("Speed"));
                    DrawProperty(nameof(Engine.Overlay), nameof(Engine.Overlay.TextureAnimationDirection), new("Direction"));
                    break;
            }
        }
    }
}