using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Experimental.Rendering;
using System.Collections.Generic;
using NUnit.Framework;

namespace SHADERGEAR.OutlineEngine
{
    internal class MaskPass : ScriptableRenderPass, IDisposable
    {
        Material material;
        RenderingLayerMask renderingLayerMask;
        RenderingOcclusion occlusion;
        Texture2D cutoutTexture;
        RTHandle cutoutHandle;
        bool cutout;

        public MaskPass(RenderPassEvent renderPassEvent, RenderingLayerMask renderingLayerMask, RenderingOcclusion occlusion,
            bool cutout, Texture2D cutoutTexture, float cutoutThreshold, CutoutChannel cutoutChannel)
        {
            this.renderPassEvent = renderPassEvent;
            this.renderingLayerMask = renderingLayerMask;
            this.occlusion = occlusion;
            this.cutout = cutout;
            this.cutoutTexture = cutoutTexture;

            var shader = Resources.Load<Shader>("OutlineEngine/MaskPass");
            material = CoreUtils.CreateEngineMaterial(shader);
            if (cutout && cutoutTexture != null)
            {
                cutoutHandle = RTHandles.Alloc(cutoutTexture);
                material.SetKeyword(new LocalKeyword(shader, "USE_CUTOUT"), true);
                material.SetTexture("_CutoutTexture", cutoutTexture);
                material.SetFloat("_CutoutThreshold", cutoutThreshold);
                material.SetInteger("_CutoutChannel", (int)cutoutChannel);
            }
        }

        public void Dispose()
        {
            CoreUtils.Destroy(material);
            cutoutHandle?.Release();
        }

        class PassData
        {
            public RendererListHandle rendererList;
        }

        static readonly List<ShaderTagId> shaderTagIds = new()
        {
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("UniversalForwardOnly"),
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("Universal2D")
        };

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Mask (Outline Engine)", out var passData))
            {
                // get frame data
                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraData = frameData.Get<UniversalCameraData>();
                var renderingData = frameData.Get<UniversalRenderingData>();
                var lightData = frameData.Get<UniversalLightData>();

                // create destination texture
                var textureDescription = renderGraph.GetTextureDesc(resourceData.activeColorTexture);
                textureDescription.name = "_OutlineEngineTextureA";
                textureDescription.format = SystemInfo.GetCompatibleFormat(GraphicsFormat.R16G16_SNorm, GraphicsFormatUsage.Render);
                textureDescription.clearColor = new Color(-1, -1, 0, 0);
                var textureA = renderGraph.CreateTexture(textureDescription);

                // create context data
                var outlineData = frameData.GetOrCreate<OutlineData>();
                outlineData.texture = textureA;

                // create filtering settings
                var renderQueueRange = RenderQueueRange.all;
                var filteringSettings = new FilteringSettings(renderQueueRange: renderQueueRange, renderingLayerMask: renderingLayerMask);

                // create drawing settings
                var sortFlags = cameraData.defaultOpaqueSortFlags;
                var drawingSettings = RenderingUtils.CreateDrawingSettings(shaderTagIds, renderingData, cameraData, lightData, sortFlags);
                drawingSettings.overrideMaterial = material;
                drawingSettings.overrideMaterialPassIndex = (int)occlusion;

                // create rendering list
                var rendererListParams = new RendererListParams(renderingData.cullResults, drawingSettings, filteringSettings);
                var rendererList = renderGraph.CreateRendererList(rendererListParams);

                // set up renderer list
                builder.UseRendererList(rendererList);
                passData.rendererList = rendererList;

                // set up cutout
                if (cutout && cutoutTexture != null)
                {
                    var textureHandle = renderGraph.ImportTexture(cutoutHandle);
                    builder.UseTexture(textureHandle);
                }

                // set up render targets
                builder.SetRenderAttachment(textureA, 0);
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
