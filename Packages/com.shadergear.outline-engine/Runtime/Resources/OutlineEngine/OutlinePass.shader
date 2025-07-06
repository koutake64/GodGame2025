Shader "Hidden/SHADERGEAR/OutlineEngine/OutlinePass"
{
    Properties
    {
        _Width("Width", Integer) = 1
        [HDR] _Color("Color", Color) = (1, 1, 1, 1)
        _Softness("Softness", float) = 0.0
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
            Comp NotEqual
        }

        Pass
        {
            HLSLPROGRAM            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            #pragma multi_compile_local _ USE_TEXTURE

            #pragma vertex Vert
            #pragma fragment frag
        
            SAMPLER(sampler_BlitTexture);

            CBUFFER_START(UnityPerMaterial)
            int _Width;
            half4 _Color;
            float _Softness;
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
                float2 pixel = IN.positionCS.xy;
        
                float2 rg = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, uv).rg;
                rg *= _ScreenParams.xy;
        
                float dist = distance(pixel, rg);
        
                #if !USE_TEXTURE
                half4 color = _Color; 
                #else
                half4 color = sampleTextureSource(uv);
                #endif
                
                if (_Softness > 0)
                {
                    color.a *= smoothstep(_Width,  (1 - _Softness) * _Width, dist);
                }
        
                color *= step(dist, _Width + 0.05);
                
                return color;
            }
            ENDHLSL
        }
    }
}