Shader "Hidden/SHADERGEAR/OutlineEngine/JumpPass"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            #define INFINITY 9999999

            SAMPLER(sampler_BlitTexture);
            int _OutlineWidth;

            float4 frag(Varyings IN) : SV_Target
            {
                float2 pixelSize = _BlitTexture_TexelSize.xy;

                float2 originUV = IN.texcoord;
                float2 originRG = IN.positionCS.xy;

                float2 minRG = float2(-1, -1);
                float minDist = INFINITY;

                [unroll]
                for (int x = -1; x <= 1; x++)
                {
                    [unroll]
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 offsetUV = originUV + float2(x, y) * pixelSize * _OutlineWidth;
                        float2 offsetRG = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, offsetUV).rg;
                        offsetRG *= _ScreenParams.xy;

                        float dist = distance(originRG, offsetRG);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            minRG = offsetRG;
                        }
                    }
                }

                minRG /= _ScreenParams.xy;
                return float4(minRG, 0, 0);
            }
            ENDHLSL
        }
    }
}