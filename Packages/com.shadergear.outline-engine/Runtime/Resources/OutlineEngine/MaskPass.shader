Shader "Hidden/SHADERGEAR/OutlineEngine/MaskPass"
{
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    
    #pragma multi_compile_local _ USE_CUTOUT
    #pragma multi_compile _ DOTS_INSTANCING_ON

    #if DOTS_INSTANCING_ON
    #pragma target 4.5
    #endif

    struct Attributes
    {
        float4 positionOS : POSITION;
        
        #if USE_CUTOUT
        float2 cutoutUV : TEXCOORD0;
        #endif

        #if DOTS_INSTANCING_ON
        UNITY_VERTEX_INPUT_INSTANCE_ID
        #endif
    };
    
    struct Varyings
    {
        float4 positionCS : SV_POSITION;

        #if USE_CUTOUT
        float2 cutoutUV : TEXCOORD0;
        #endif

        #if DOTS_INSTANCING_ON
        UNITY_VERTEX_INPUT_INSTANCE_ID
        #endif
    };

    #if USE_CUTOUT
    TEXTURE2D_X(_CutoutTexture);
    SAMPLER(sampler_CutoutTexture);
    
    CBUFFER_START(UnityPerMaterial)
    float4 _CutoutTexture_ST;
    float _CutoutThreshold;
    int _CutoutChannel;
    CBUFFER_END
    #endif
    
    Varyings vert(Attributes IN)
    {
        Varyings OUT;

        #if DOTS_INSTANCING_ON
        UNITY_SETUP_INSTANCE_ID(IN);
        UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
        #endif

        #if DOTS_INSTANCING_ON
            OUT.positionCS = GetVertexPositionInputs(IN.positionOS.xyz).positionCS;
        #else
            OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
        #endif

        #if USE_CUTOUT
        OUT.cutoutUV = TRANSFORM_TEX(IN.cutoutUV, _CutoutTexture);
        #endif

        return OUT;
    }
    
    float4 frag(Varyings IN) : SV_Target
    {
        #if DOTS_INSTANCING_ON
        UNITY_SETUP_INSTANCE_ID(IN);
        #endif

        #if USE_CUTOUT
        float4 cutoutSample = SAMPLE_TEXTURE2D_X(_CutoutTexture, sampler_CutoutTexture, IN.cutoutUV);
        clip(cutoutSample[_CutoutChannel] - _CutoutThreshold);
        #endif

        float2 uv = IN.positionCS.xy / _ScreenParams.xy;
        return float4(uv, 0, 0);
    }
    ENDHLSL
    Properties
    {
        _CutoutTexture("Cutout Texture", 2D) = "" {}
        _CutoutThreshold("Cutout Threshold", float) = 0.001
        _CutoutChannel("Cutout Threshold", Integer) = 3
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        
        Pass 
        {
            Name "OutlineEverything"
            ZTest Always
            
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
        
        Pass 
        { 
            Name "OutlineNotOccluded"
            ZTest Always
            
            Stencil
            {
                Ref 0
                Comp Equal
                Pass IncrSat
            }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
        
        Pass 
        { 
            Name "OutlineOccluded"
            ZTest Always

            Stencil
            {
                Ref 2
                Comp Equal
                Pass DecrSat
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }

}