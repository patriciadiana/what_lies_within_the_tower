Shader "Sun_Temple/WindowGlass_URP" {
    Properties{
        _MainTex("Albedo (RGB) Glass Mask(A)", 2D) = "white" {}
        [NoScaleOffset]_RoughnessTexture("Roughness (R)", 2D) = "white" {}
        [Normal][NoScaleOffset]_BumpMap("Normal", 2D) = "bump" {}
        [NoScaleOffset]_Emission("Emission(RGB)", 2D) = "black" {}

        _EmissionIntensity("Emission Intensity", Range(0, 8)) = 0
        _EmissionVertexMask("Emission Vertex Mask", Range(0, 1)) = 0
        _Reflection("Reflection (CUBE)", CUBE) = ""{}
        _SkyColor("Sky Color (RGB)", Color) = (1, 1, 1, 1)
    }

        SubShader{
            Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" }
            LOD 500

            HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            ENDHLSL

            Pass {
                Name "ForwardLit"
                Tags { "LightMode" = "UniversalForward" }

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
                #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
                #pragma multi_compile_fragment _ _SHADOWS_SOFT

                struct Attributes {
                    float4 positionOS   : POSITION;
                    float3 normalOS     : NORMAL;
                    float4 tangentOS    : TANGENT;
                    float2 uv           : TEXCOORD0;
                    float4 color        : COLOR;
                };

                struct Varyings {
                    float4 positionCS   : SV_POSITION;
                    float2 uv           : TEXCOORD0;
                    float3 positionWS   : TEXCOORD1;
                    float3 normalWS     : TEXCOORD2;
                    float4 tangentWS    : TEXCOORD3;
                    float3 viewDirWS    : TEXCOORD4;
                    float3 reflectionWS : TEXCOORD5;
                    float4 color        : COLOR;
                };

                TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
                TEXTURE2D(_RoughnessTexture); SAMPLER(sampler_RoughnessTexture);
                TEXTURE2D(_BumpMap); SAMPLER(sampler_BumpMap);
                TEXTURE2D(_Emission); SAMPLER(sampler_Emission);
                TEXTURECUBE(_Reflection); SAMPLER(sampler_Reflection);

                CBUFFER_START(UnityPerMaterial)
                    float4 _MainTex_ST;
                    float _EmissionIntensity;
                    float _EmissionVertexMask;
                    float3 _SkyColor;
                CBUFFER_END

                Varyings vert(Attributes IN) {
                    Varyings OUT;
                    UNITY_SETUP_INSTANCE_ID(IN);
                    UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                    VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                    VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);

                    OUT.positionCS = positionInputs.positionCS;
                    OUT.positionWS = positionInputs.positionWS;
                    OUT.normalWS = normalInputs.normalWS;
                    OUT.tangentWS = float4(normalInputs.tangentWS, IN.tangentOS.w);
                    OUT.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);
                    OUT.reflectionWS = reflect(-OUT.viewDirWS, OUT.normalWS);
                    OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                    OUT.color = IN.color;
                    return OUT;
                }

                half4 frag(Varyings IN) : SV_Target {
                    half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                    half3 normal = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, IN.uv));
                    half roughness = SAMPLE_TEXTURE2D(_RoughnessTexture, sampler_RoughnessTexture, IN.uv).r;
                    half emissionMask = lerp(0, 1, pow(IN.color.r, 4));
                    half3 emission = SAMPLE_TEXTURE2D(_Emission, sampler_Emission, IN.uv).rgb * emissionMask * _EmissionIntensity * _SkyColor;

                    float3 normalWS = TransformTangentToWorld(normal, half3x3(IN.tangentWS.xyz, cross(IN.normalWS, IN.tangentWS.xyz) * IN.tangentWS.w, IN.normalWS));

                    half fresnel = 1.0 - saturate(dot(normalize(IN.viewDirWS), normalWS));

                    half3 reflection = SAMPLE_TEXTURECUBE(_Reflection, sampler_Reflection, IN.reflectionWS).rgb;
                    reflection = reflection * (1 - roughness * 2) * pow(fresnel, 2);

                    // Lighting calculation
                    InputData inputData = (InputData)0;
                    inputData.positionWS = IN.positionWS;
                    inputData.normalWS = normalize(normalWS);
                    inputData.viewDirectionWS = SafeNormalize(IN.viewDirWS);
                    inputData.shadowCoord = TransformWorldToShadowCoord(IN.positionWS);

                    SurfaceData surfaceData = (SurfaceData)0;
                    surfaceData.albedo = color.rgb;
                    surfaceData.metallic = 0;
                    surfaceData.smoothness = 1 - roughness;
                    surfaceData.normalTS = normal;
                    surfaceData.emission = emission + saturate(reflection);

                    half4 finalColor = UniversalFragmentPBR(inputData, surfaceData);
                    return finalColor;
                }
                ENDHLSL
            }

            // You may need to add ShadowCaster pass here if you want the object to cast shadows
        }
            FallBack "Universal Render Pipeline/Lit"
}