Shader "Hidden/SHADERGEAR/OutlineEngine/OutlinePass"
{
    Properties
    {
        [HDR] _Color("Color", Color) = (1, 1, 1, 1)
        _TextureSource("Texture Source", 2D) = "" {}
        _TextureRotation("Texture Rotation", float) = 0
        _TextureAnimationSpeed("Texture Animation Speed", float) = 0
        _TextureAnimationDirection("Texture Animation Direction", float) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Stencil
        {
            Ref 1
            Comp Equal
        }
        
        Pass
        {
            HLSLPROGRAM            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma multi_compile_local _ USE_TEXTURE
            
            #pragma vertex Vert
            #pragma fragment frag
        
            CBUFFER_START(UnityPerMaterial)
            half4 _Color;
            float4 _TextureSource_ST;
            float _TextureRotation;
            float _TextureAnimationSpeed;
            float _TextureAnimationDirection;
            CBUFFER_END

            #if USE_TEXTURE
            #include "TextureSource.hlsl"
            #endif

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.texcoord;

                #if !USE_TEXTURE
                half4 color = _Color; 
                #else
                half4 color = sampleTextureSource(uv);
                #endif

                return color;
            }
            ENDHLSL
        }
    }
}