Shader "Sun_Temple/Foliage_URP" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Layer*A Albedo (RGB)", 2D) = "black" {}
        _AlphaCutoff("Alpha cutoff", Range(0,1)) = 0.5  // Renamed from _Cutoff to _AlphaCutoff
        _SelfIllum("Self Illumination", Range(0, 1)) = 0
        _NormalModification("Normal Modification", Range(0, 1)) = 1
        _WaveFreq("Wave Frequency", Float) = 20
        _WaveHeight("Wave Height", Float) = 0.1
        _WaveScale("Wave Scale", Float) = 1
    }

        SubShader{
            Tags {"Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "RenderPipeline" = "UniversalPipeline"}
            LOD 200

            HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _MainTex_ST;
                float _AlphaCutoff;  // Renamed from _Cutoff to _AlphaCutoff
                float _SelfIllum;
                float _NormalModification;
                float _WaveFreq;
                float _WaveHeight;
                float _WaveScale;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Simple wind function (you may need to adapt this from your VertexWind.cginc)
            float3 wind_simplified(float3 vertex, float4 color, float freq, float height, float scale) {
                float time = _Time.y;
                float3 worldPos = mul(unity_ObjectToWorld, float4(vertex, 1)).xyz;
                float wave = sin(time * freq + worldPos.x * scale + worldPos.z * scale) * height * color.r;
                return float3(wave, 0, wave);
            }
            ENDHLSL

            Pass {
                Name "ForwardLit"
                Tags {"LightMode" = "UniversalForward"}

                Cull Back

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
                #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
                #pragma multi_compile_fragment _ _SHADOWS_SOFT

                struct Attributes {
                    float4 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                    float2 uv : TEXCOORD0;
                    float4 color : COLOR;
                };

                struct Varyings {
                    float4 positionCS : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 positionWS : TEXCOORD1;
                    float3 normalWS : TEXCOORD2;
                    float4 color : COLOR;
                };

                Varyings vert(Attributes input) {
                    Varyings output;

                    // Apply wind
                    input.positionOS.xyz += wind_simplified(input.positionOS.xyz, input.color, _WaveFreq, _WaveHeight, _WaveScale);

                    // Modify normal
                    float3 modifiedNormal = float3(0, 2, 0);
                    input.normalOS = lerp(input.normalOS, input.normalOS + modifiedNormal, _NormalModification);

                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                    output.positionCS = vertexInput.positionCS;
                    output.positionWS = vertexInput.positionWS;
                    output.normalWS = normalInput.normalWS;
                    output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                    output.color = input.color;

                    return output;
                }

                half4 frag(Varyings input) : SV_Target {
                    half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                    half3 color = albedo.rgb * _Color.rgb;
                    half alpha = albedo.a;

                    // Alpha test
                    clip(alpha - _AlphaCutoff);  // Using _AlphaCutoff instead of _Cutoff

                    InputData inputData = (InputData)0;
                    inputData.positionWS = input.positionWS;
                    inputData.normalWS = normalize(input.normalWS);
                    inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                    inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);

                    SurfaceData surfaceData = (SurfaceData)0;
                    surfaceData.albedo = color;
                    surfaceData.alpha = alpha;
                    surfaceData.emission = color * _SelfIllum;
                    surfaceData.metallic = 0;
                    surfaceData.occlusion = 1;
                    surfaceData.smoothness = 0;

                    return UniversalFragmentPBR(inputData, surfaceData);
                }
                ENDHLSL
            }

                // ShadowCaster pass for casting shadows
                Pass {
                    Name "ShadowCaster"
                    Tags {"LightMode" = "ShadowCaster"}

                    ZWrite On
                    ZTest LEqual
                    Cull Back

                    HLSLPROGRAM
                    #pragma vertex ShadowPassVertex
                    #pragma fragment ShadowPassFragment

                    #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
                    ENDHLSL
                }
        }
            FallBack "Universal Render Pipeline/Lit"
}