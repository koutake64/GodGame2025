using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements.Experimental;
using System.Reflection;
using UnityEngine.Rendering;

namespace SHADERGEAR.OutlineEngine.Editor
{
    public class IntroductionWindow : EditorWindow
    {
        [SerializeField] VisualTreeAsset visualTreeAsset;

        const string DID_INTRODUCTION_OPEN_KEY = "SHADERGEAR:OutlineEngine:DidIntroductionOpen";

        [MenuItem("Window/Rendering/Outline Engine/Introduction")]
        public static void Open()
        {
            var window = GetWindow<IntroductionWindow>();
            window.minSize = new Vector2(250, 400);
            window.maxSize = new Vector2(400, 400);
            window.titleContent = new GUIContent("Welcome to Outline Engine");
        }

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            EditorApplication.update += OpenOnce;
        }

        static void OpenOnce()
        {
            EditorApplication.update -= OpenOnce;

            if (!EditorPrefs.GetBool(DID_INTRODUCTION_OPEN_KEY, false))
            {
                EditorPrefs.SetBool(DID_INTRODUCTION_OPEN_KEY, true);
                Open();
            }
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            var tree = visualTreeAsset.Instantiate();
            root.Add(tree);

            root.RegisterCallback<PointerDownLinkTagEvent>(ev =>
            {
                switch (ev.linkID)
                {
                    case "renderer-asset":
                        SelectRendererAsset();
                        break;
                }
            });

            root.RegisterCallback<PointerOverLinkTagEvent>(ev =>
            {
                (ev.target as Label).AddToClassList("hover");
            });

            root.RegisterCallback<PointerOutLinkTagEvent>(ev =>
            {
                (ev.target as Label).RemoveFromClassList("hover");
            });

            var renderPipeline = GetRenderPipelineAsset();
            if (!renderPipeline)
            {
                var helpBox = new HelpBox("Outline Engine requires a <b>Universal Render Pipeline Asset</b> set in graphics or quality settings.", HelpBoxMessageType.Warning);
                root.Q("getting-started").Add(helpBox);
            }
            else
            {
                var rendererData = GetRendererData(renderPipeline);
                if (rendererData == null)
                {
                    var helpBox = new HelpBox("Outline Engine requires a <b>Universal Renderer Data</b> set in the current <b>Universal Render Pipeline Asset</b>.", HelpBoxMessageType.Warning);
                    root.Q("getting-started").Add(helpBox);
                }
            }
        }

        void SelectRendererAsset()
        {
            var renderPipeline = GetRenderPipelineAsset();
            if (renderPipeline == null)
            {
                Debug.LogWarning("(Outline Engine Introduction) Cannot select the current renderer asset, because there is no render pipeline asset set.");
                return;
            }

            var rendererData = GetRendererData(renderPipeline);
            if (rendererData == null)
            {
                Debug.LogWarning("(Outline Engine Introduction) Cannot select the current renderer asset, because there is no renderer assigned in the render pipeline asset.");
                return;
            }

            AssetDatabase.OpenAsset(rendererData);
            EditorGUIUtility.PingObject(rendererData);
        }

        static int GetDefaultRendererIndex(UniversalRenderPipelineAsset renderPipeline)
        {
            var field = typeof(UniversalRenderPipelineAsset).GetField("m_DefaultRendererIndex", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) return 0;

            var value = (int)field.GetValue(renderPipeline);
            return value;
        }

        static UniversalRenderPipelineAsset GetRenderPipelineAsset()
        {
            return (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        }

        static ScriptableRendererData GetRendererData(UniversalRenderPipelineAsset renderPipeline)
        {
            var index = GetDefaultRendererIndex(renderPipeline);
            var renderer = renderPipeline.rendererDataList[index];

            return renderer;
        }
    }
}