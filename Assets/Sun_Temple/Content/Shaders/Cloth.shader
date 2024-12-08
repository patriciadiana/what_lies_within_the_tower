Shader "Sun_Temple/Cloth_URP" {
    Properties{
        _MainTex("Layer*A Albedo (RGB)", 2D) = "white" {}
        _SelfIllum("Self Illumination", Range(0, 1)) = 0
        [NoScaleOffset] _DetailAlbedo("DETAIL*Albedo", 2D) = "grey" {}
        _DetailTiling("DETAIL*Tiling", Float) = 2
        _WaveFreq("Wave Frequency", Float) = 20
        _WaveHeight("Wave Height", Float) = 0.1
        _WaveScale("Wave Scale", Float) = 1
    }

        SubShader{
            Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" }
            LOD 400

            HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _SelfIllum;
                float _DetailTiling;
                float _WaveFreq;
                float _WaveHeight;
                float _WaveScale;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_DetailAlbedo);
            SAMPLER(sampler_DetailAlbedo);

            struct ClothAttributes {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float3 windanim(float3 vertex_xyz, float2 color, float WaveFreq, float WaveHeight, float WaveScale) {
                float phase_slow = _Time.y * WaveFreq;
                float phase_med = _Time.y * 4 * WaveFreq;

                float offset = (vertex_xyz.x + (vertex_xyz.z * WaveScale)) * WaveScale;
                float offset2 = (vertex_xyz.x + (vertex_xyz.z * WaveScale * 2)) * WaveScale * 2;

                float sin1 = sin(phase_slow + offset);
                float sin2 = sin(phase_med + offset2);

                float sin_combined = (sin1 * 4) + sin2;

                float wind_x = sin_combined * WaveHeight * 0.1;
                float3 wind_xyz = float3(wind_x, wind_x * 2, wind_x);
                wind_xyz = wind_xyz * pow(color.r, 2);
                return wind_xyz;
            }

            ENDHLSL

            Pass {
                Name "ForwardLit"
                Tags{"LightMode" = "UniversalForward"}

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
                #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
                #pragma multi_compile_fragment _ _SHADOWS_SOFT

                struct ClothVaryings {
                    float4 positionCS : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normalWS : TEXCOORD1;
                    float3 positionWS : TEXCOORD2;
                };

                ClothVaryings vert(ClothAttributes input)
                {
                    ClothVaryings output;
                    UNITY_SETUP_INSTANCE_ID(input);

                    // Apply wind animation
                    input.positionOS.xyz += windanim(input.positionOS.xyz, input.color.rg, _WaveFreq, _WaveHeight, _WaveScale);

                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);

                    output.positionCS = vertexInput.positionCS;
                    output.positionWS = vertexInput.positionWS;
                    output.normalWS = normalInput.normalWS;
                    output.uv = TRANSFORM_TEX(input.uv, _MainTex);

                    return output;
                }

                half4 frag(ClothVaryings input) : SV_Target
                {
                    half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                    half3 detailAlbedo = SAMPLE_TEXTURE2D(_DetailAlbedo, sampler_DetailAlbedo, input.uv * _DetailTiling).rgb * 2.0; // Approximation of unity_ColorSpaceDouble
                    albedo.rgb = albedo.rgb * lerp(1, detailAlbedo, 0.5);

                    InputData inputData = (InputData)0;
                    inputData.positionWS = input.positionWS;
                    inputData.normalWS = normalize(input.normalWS);
                    inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                    inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);

                    half3 emission = albedo.rgb * _SelfIllum;

                    SurfaceData surfaceData = (SurfaceData)0;
                    surfaceData.albedo = albedo.rgb;
                    surfaceData.emission = emission;
                    surfaceData.metallic = 0;
                    surfaceData.smoothness = 0;
                    surfaceData.normalTS = half3(0, 0, 1);
                    surfaceData.occlusion = 1;

                    half4 color = UniversalFragmentPBR(inputData, surfaceData);
                    return color;
                }
                ENDHLSL
            }

                // ShadowCaster pass for casting shadows
                Pass
                {
                    Name "ShadowCaster"
                    Tags{"LightMode" = "ShadowCaster"}

                    ZWrite On
                    ZTest LEqual
                    ColorMask 0
                    Cull Back

                    HLSLPROGRAM
                    #pragma vertex ShadowPassVertex
                    #pragma fragment ShadowPassFragment

                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

                    float3 _LightDirection;

                    struct ShadowVaryings
                    {
                        float4 positionCS : SV_POSITION;
                    };

                    float4 GetShadowPositionHClip(ClothAttributes input)
                    {
                        float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                        float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                        // Apply wind animation
                        positionWS += windanim(input.positionOS.xyz, input.color.rg, _WaveFreq, _WaveHeight, _WaveScale);

                        float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

                    #if UNITY_REVERSED_Z
                        positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                    #else
                        positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
                    #endif

                        return positionCS;
                    }

                    ShadowVaryings ShadowPassVertex(ClothAttributes input)
                    {
                        ShadowVaryings output;
                        UNITY_SETUP_INSTANCE_ID(input);

                        output.positionCS = GetShadowPositionHClip(input);
                        return output;
                    }

                    half4 ShadowPassFragment(ShadowVaryings input) : SV_TARGET
                    {
                        return 0;
                    }

                    ENDHLSL
                }
        }
            FallBack "Universal Render Pipeline/Lit"
}