using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

namespace SHADERGEAR.OutlineEngine
{
    internal class OutlineData : ContextItem
    {
        public TextureHandle texture;

        public override void Reset()
        {
            texture = TextureHandle.nullHandle;
        }
    }
}