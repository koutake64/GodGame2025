Shader "Hidden/SHADERGEAR/Outline/OcclusionPass"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        ZTest LEqual

        Stencil
        {
            Ref 2
            Comp Always
            Pass Replace
        }
        
        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma multi_compile _ DOTS_INSTANCING_ON

            #if DOTS_INSTANCING_ON
            #pragma target 4.5
            #endif

            #pragma vertex vert
            #pragma fragment frag

            struct Attributes
            {
                float4 positionOS : POSITION;

                #if DOTS_INSTANCING_ON
                UNITY_VERTEX_INPUT_INSTANCE_ID
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;

                #if DOTS_INSTANCING_ON
                UNITY_VERTEX_INPUT_INSTANCE_ID
                #endif
            };

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
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                #if DOTS_INSTANCING_ON
                UNITY_SETUP_INSTANCE_ID(IN);
                #endif

                return float4(0, 0, 0, 0);
            }
            ENDHLSL
        }
    }
}