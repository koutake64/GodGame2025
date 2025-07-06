using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;
using System.Collections.Generic;

namespace SHADERGEAR.OutlineEngine
{
    internal class OcclusionPass : ScriptableRenderPass, IDisposable
    {
        RenderingLayerMask renderingLayerMask;
        Material material;

        static readonly List<ShaderTagId> shaderTagIds = new()
        {
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("Universal2D")
        };

        public OcclusionPass(RenderPassEvent renderPassEvent, RenderingLayerMask renderingLayerMask)
        {
            this.renderPassEvent = renderPassEvent;
            this.renderingLayerMask = renderingLayerMask;

            var shader = Resources.Load<Shader>("OutlineEngine/OcclusionPass");
            material = CoreUtils.CreateEngineMaterial(shader);
        }

        public void Dispose()
        {
            CoreUtils.Destroy(material);
        }

        class PassData
        {
            public RendererListHandle rendererList;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Occlusion (Outline Engine)", out var passData))
            {
                // get frame data
                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraData = frameData.Get<UniversalCameraData>();
                var renderingData = frameData.Get<UniversalRenderingData>();
                var lightData = frameData.Get<UniversalLightData>();

                // create filtering settings
                var renderQueueRange = RenderQueueRange.all;
                var filteringSettings = new FilteringSettings(renderQueueRange: renderQueueRange, renderingLayerMask: renderingLayerMask);

                // create drawing settings
                var sortFlags = cameraData.defaultOpaqueSortFlags;
                var drawingSettings = RenderingUtils.CreateDrawingSettings(shaderTagIds, renderingData, cameraData, lightData, sortFlags);
                drawingSettings.overrideMaterial = material;

                // create rendering list
                var rendererListParams = new RendererListParams(renderingData.cullResults, drawingSettings, filteringSettings);
                var rendererList = renderGraph.CreateRendererList(rendererListParams);

                // set up renderer list
                builder.UseRendererList(rendererList);
                passData.rendererList = rendererList;

                // set up render targets
                builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);

                // set up render function
                builder.SetRenderFunc<PassData>((passData, context) => ExecutePass(passData, context));
            }
        }

        static void ExecutePass(PassData passData, RasterGraphContext context)
        {
            context.cmd.DrawRendererList(passData.rendererList);
        }
    }
}