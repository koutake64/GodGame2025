using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using System;

namespace SHADERGEAR.OutlineEngine
{
    internal class OverlayPass : ScriptableRenderPass, IDisposable
    {
        Material material;
        
        public OverlayPass(RenderPassEvent renderPassEvent, OverlaySettings overlay)
        {
            this.renderPassEvent = renderPassEvent;

            var shader = Resources.Load<Shader>("OutlineEngine/OverlayPass");
            material = CoreUtils.CreateEngineMaterial(shader);
            material.SetColor("_Color", overlay.Color);

            if (overlay.Style == OverlayStyle.Texture && overlay.TextureSource != null)
            {
                material.SetKeyword(new LocalKeyword(shader, "USE_TEXTURE"), true);
                material.SetTexture("_TextureSource", overlay.TextureSource);
                material.SetTextureScale("_TextureSource", overlay.TextureScale);
                material.SetTextureOffset("_TextureSource", overlay.TextureOffset);
                material.SetFloat("_TextureRotation", overlay.TextureRotation);
                material.SetFloat("_TextureAnimationSpeed", overlay.TextureAnimationSpeed);
                material.SetFloat("_TextureAnimationDirection", overlay.TextureAnimationDirection);
            }
        }

        public void Dispose()
        {
            CoreUtils.Destroy(material);
        }

        class PassData
        {
            public Material material;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Overlay (Outline Engine)", out var passData))
            {
                // get destination texture
                var resourceData = frameData.Get<UniversalResourceData>();
                var destination = resourceData.activeColorTexture;

                // set up pass data
                passData.material = material;

                // set up depth texture (read-only)
                builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Read);
                
                // set up render target
                builder.SetRenderAttachment(destination, 0);                

                // set up render function
                builder.SetRenderFunc<PassData>((passData, context) => ExecutePass(passData, context));
            }
        }

        static void ExecutePass(PassData passData, RasterGraphContext context)
        {
            Blitter.BlitTexture(context.cmd, new Vector4(1f, 1f, 0f, 0f), passData.material, 0);
        }
    }
}