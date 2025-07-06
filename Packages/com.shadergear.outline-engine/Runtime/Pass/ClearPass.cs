using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace SHADERGEAR.OutlineEngine
{
    internal class ClearPass : ScriptableRenderPass
    {
        public ClearPass(RenderPassEvent renderPassEvent)
        {
            this.renderPassEvent = renderPassEvent;
        }

        class PassData { }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Clear (Outline Engine)", out var passData))
            {
                // get resources
                var resourceData = frameData.Get<UniversalResourceData>();
                
                // set up render target
                builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture);

                // set up render function
                builder.SetRenderFunc<PassData>((passData, context) => ExecutePass(passData, context));
            }
        }

        static void ExecutePass(PassData passData, RasterGraphContext context)
        {
            context.cmd.ClearRenderTarget(RTClearFlags.Stencil, Color.black, 1, 0);
        }
    }
}