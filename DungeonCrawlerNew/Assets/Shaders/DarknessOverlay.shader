Shader "Custom/DarknessOverlay"
{
    Properties
    {
        _Color ("Color", Color) = (0.04, 0.04, 0.06, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry+500"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "DarknessOverlay"
            ZWrite Off
            ZTest Always
            Cull Off

            Stencil
            {
                Ref 1
                Comp NotEqual
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings   { float4 positionCS : SV_POSITION; };

            Varyings vert(Attributes i)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                return o;
            }

            half4 frag(Varyings i) : SV_Target { return half4(_Color.rgb, 1); }
            ENDHLSL
        }
    }
}
