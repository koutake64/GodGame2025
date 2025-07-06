using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using System;

namespace SHADERGEAR.OutlineEngine
{
    internal class OutlinePass : ScriptableRenderPass, IDisposable
    {
        Material material;
        
        public OutlinePass(RenderPassEvent renderPassEvent, OutlineSettings outline)
        {
            this.renderPassEvent = renderPassEvent;

            var shader = Resources.Load<Shader>("OutlineEngine/OutlinePass");
            material = CoreUtils.CreateEngineMaterial(shader);
            material.SetInteger("_Width", outline.Width);
            material.SetColor("_Color", outline.Color);

            var softness = outline.Width > 1 ? outline.Softness : 0;
            material.SetFloat("_Softness", softness);

            if (outline.Style == OutlineStyle.Texture && outline.TextureSource != null)
            {
                material.SetKeyword(new LocalKeyword(shader, "USE_TEXTURE"), true);
                material.SetTexture("_TextureSource", outline.TextureSource);
                material.SetTextureScale("_TextureSource", outline.TextureScale);
                material.SetTextureOffset("_TextureSource", outline.TextureOffset);
                material.SetFloat("_TextureRotation", outline.TextureRotation);
                material.SetFloat("_TextureAnimationSpeed", outline.TextureAnimationSpeed);
                material.SetFloat("_TextureAnimationDirection", outline.TextureAnimationDirection);
            }
        }

        public void Dispose()
        {
            CoreUtils.Destroy(material);
        }

        class PassData
        {
            public TextureHandle source;
            public Material material;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Outline (Outline Engine)", out var passData))
            {
                // get source texture
                var outlineData = frameData.Get<OutlineData>();
                var source = outlineData.texture;

                // get destination texture
                var resourceData = frameData.Get<UniversalResourceData>();
                var destination = resourceData.activeColorTexture;

                // set up pass data
                passData.source = source;
                passData.material = material;

                // set up source texture
                builder.UseTexture(source);

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
            Blitter.BlitTexture(context.cmd, passData.source, new Vector4(1f, 1f, 0f, 0f), passData.material, 0);
        }
    }
}