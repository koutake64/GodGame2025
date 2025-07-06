using UnityEngine.Rendering.Universal;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Experimental.Rendering;

namespace SHADERGEAR.OutlineEngine
{
    internal class JumpPass : ScriptableRenderPass, IDisposable
    {
        Material material;
        int width;
        int steps;

        public JumpPass(RenderPassEvent renderPassEvent, int width)
        {
            this.renderPassEvent = renderPassEvent;
            this.width = width;

            var shader = Resources.Load<Shader>("OutlineEngine/JumpPass");
            material = CoreUtils.CreateEngineMaterial(shader);
            steps = (int)Mathf.Log(width, 2) + 1;
        }

        public void Dispose()
        {
            CoreUtils.Destroy(material);
        }

        class PassData
        {
            public TextureHandle textureA;
            public TextureHandle textureB;
            public Material material;
            public int width;
            public int steps;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            using (var builder = renderGraph.AddUnsafePass<PassData>("Jump (Outline Engine)", out var passData))
            {
                // get the first texture
                var outlineData = frameData.Get<OutlineData>();
                var textureA = outlineData.texture;

                // create the second texture
                var textureDescription = renderGraph.GetTextureDesc(textureA);
                textureDescription.name = "_OutlineEngineTextureB";
                textureDescription.format = SystemInfo.GetCompatibleFormat(GraphicsFormat.R16G16_SNorm, GraphicsFormatUsage.Render);
                textureDescription.clearColor = new Color(-1, -1, 0, 0);
                var textureB = renderGraph.CreateTexture(textureDescription);

                // set up the pass data
                passData.textureA = textureA;
                passData.textureB = textureB;
                passData.material = material;
                passData.width = width;
                passData.steps = steps;

                // set up textures
                builder.UseTexture(textureA, AccessFlags.ReadWrite);
                builder.UseTexture(textureB, AccessFlags.ReadWrite);

                // set up render function
                builder.SetRenderFunc<PassData>((passData, context) => ExecutePass(passData, context));

                // set up context data
                outlineData.texture = steps % 2 == 0 ? textureA : textureB;
            }
        }

        static void ExecutePass(PassData passData, UnsafeGraphContext context)
        {
            var nativeCmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);

            int w = passData.width;
            for (int i = 0; i < passData.steps; i++)
            {
                var order = i % 2 == 0;
                var source = order ? passData.textureA : passData.textureB;
                var destination = order ? passData.textureB : passData.textureA;

                context.cmd.SetGlobalInteger("_OutlineWidth", w);                
                Blitter.BlitTexture(nativeCmd, source, destination, passData.material, pass: 0);

                w /= 2;
            }
        }
    }
}
