Shader "Sun_Temple/Clouds_URP" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}

        _DistortionTexture("Distortion Texture", 2D) = "black" {}
        _DistortionIntensity("Distortion Intensity", Range(0, 1)) = 0.5
        _ScrollSpeed("Scroll Speed", Float) = 0.5
    }
        SubShader{
            Tags { "RenderType" = "Opaque" "Queue" = "Overlay" "RenderPipeline" = "UniversalPipeline" }
            LOD 200
            Cull Off
            ZWrite Off
            Blend OneMinusDstColor One

            HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _MainTex_ST;
                float4 _DistortionTexture_ST;
                float _DistortionIntensity;
                float _ScrollSpeed;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_DistortionTexture);
            SAMPLER(sampler_DistortionTexture);

            ENDHLSL

            Pass {
                Name "ForwardLit"
                Tags{"LightMode" = "UniversalForward"}

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_fog

                struct Attributes {
                    float4 positionOS : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct Varyings {
                    float4 positionCS : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float fogFactor : TEXCOORD1;
                };

                Varyings vert(Attributes input) {
                    Varyings output;
                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                    output.positionCS = vertexInput.positionCS;
                    output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                    output.fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                    return output;
                }

                half4 frag(Varyings input) : SV_Target {
                    float scrollX = _ScrollSpeed * _Time.y;
                    float2 uv_scrolled = input.uv + float2(scrollX, 0);
                    float distortion = SAMPLE_TEXTURE2D(_DistortionTexture, sampler_DistortionTexture, uv_scrolled).r;

                    float uv_distorted_x = (distortion * _DistortionIntensity * 0.1) - 0.05;
                    float2 uv_distorted_xy = input.uv + float2(uv_distorted_x, 0);
                    half3 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv_distorted_xy).rgb;
                    half3 finalAlbedo = col * _Color.rgb;
                    half3 color = saturate(finalAlbedo);

                    // Apply fog
                    color = MixFog(color, input.fogFactor);

                    return half4(color, 1);
                }
                ENDHLSL
            }
        }
            FallBack "Universal Render Pipeline/Lit"
}