using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Runtime.CompilerServices;
using UnityEngine.Profiling;

[assembly: InternalsVisibleTo("SHADERGEAR.OutlineEngine.Tests")]
[assembly: InternalsVisibleTo("SHADERGEAR.OutlineEngine.Editor")]

namespace SHADERGEAR.OutlineEngine
{
    public class OutlineEngine : ScriptableRendererFeature
    {
        [field: SerializeField]
        public RenderingSettings Rendering { get; set; } = new();
        
        [field: SerializeField]
        public OutlineSettings Outline { get; set; } = new();
        
        [field: SerializeField]
        public OverlaySettings Overlay { get; set; } = new();

#if UNITY_EDITOR
        [field: SerializeField]
        internal int Tab { get; set; } = 0;
#endif

        ClearPass clearPass;
        OcclusionPass occlusionPass;
        MaskPass maskPass;
        JumpPass jumpPass;
        OutlinePass outlinePass;
        OverlayPass overlayPass;

        bool UsesOcclusionPass => Rendering.Occlusion != RenderingOcclusion.RenderEverything;
        bool UsesOutlinePass => Outline.Style != OutlineStyle.None;
        bool UsesOverlayPass => Overlay.Style != OverlayStyle.None;

        static bool UsesCameraType(CameraType cameraType)
        {
            return !cameraType.HasFlag(CameraType.Preview) && !cameraType.HasFlag(CameraType.Reflection);
        }

        public void Refresh() => Create();

        public override void Create()
        {
            Profiler.BeginSample("OutlineEngine.Create");

            Profiler.BeginSample("Validate");
            Rendering.Validate();
            Outline.Validate();
            Overlay.Validate();
            Profiler.EndSample();
            
            if (!UsesOutlinePass && !UsesOverlayPass) return;

            var renderPassEvent = Rendering.GetRenderPassEvent();

            Profiler.BeginSample("ClearPass.ClearPass");
            clearPass ??= new(renderPassEvent);
            Profiler.EndSample();

            if (UsesOcclusionPass)
            {
                Profiler.BeginSample("OcclusionPass.OcclusionPass");
                occlusionPass = new(renderPassEvent, Rendering.Occluders);
                Profiler.EndSample();
            }
            
            Profiler.BeginSample("MaskPass.MaskPass");
            maskPass = new(renderPassEvent, Rendering.Targets, Rendering.Occlusion, Rendering.Cutout,
                Rendering.CutoutTexture, Rendering.CutoutThreshold, Rendering.CutoutChannel);
            Profiler.EndSample();

            if (UsesOutlinePass)
            {
                Profiler.BeginSample("JumpPass.JumpPass");
                jumpPass = new(renderPassEvent, Outline.Width);
                Profiler.EndSample();

                Profiler.BeginSample("OutlinePass.OutlinePass");
                outlinePass = new(renderPassEvent, Outline);
                Profiler.EndSample();
            }

            if (UsesOverlayPass)
            {
                Profiler.BeginSample("OverlayPass.OverlayPass");
                overlayPass = new(renderPassEvent, Overlay);
                Profiler.EndSample();
            }

            Profiler.EndSample();
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!UsesOutlinePass && !UsesOverlayPass) return;
            if (!UsesCameraType(renderingData.cameraData.cameraType)) return;

            renderer.EnqueuePass(clearPass);

            if (UsesOcclusionPass)
            {
                renderer.EnqueuePass(occlusionPass);
            }

            renderer.EnqueuePass(maskPass);

            if (UsesOutlinePass)
            {
                renderer.EnqueuePass(jumpPass);
                renderer.EnqueuePass(outlinePass);
            }

            if (UsesOverlayPass)
            {
                renderer.EnqueuePass(overlayPass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            occlusionPass?.Dispose();
            maskPass?.Dispose();
            jumpPass?.Dispose();
            outlinePass?.Dispose();
            overlayPass?.Dispose();
        }
    }
}