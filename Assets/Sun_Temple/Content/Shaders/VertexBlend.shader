Shader "Sun_Temple/VertexBlend_URP" {
    Properties{
        _Color("BASE Tint (RGB), Tint Fade (A)", Color) = (0.5, 0.5, 0.5, 0)
        _MainTex("BASE Albedo (RGB) Tint Mask (A)", 2D) = "white" {}
        [Normal]_BumpMap("BASE Normal (RGB)", 2D) = "bump" {}
        _Roughness("BASE Roughness", Range(0,1)) = 1
        [NoScaleOffset] _layer1Tex("LAYER*B Albedo (RGB)", 2D) = "white" {}
        [Normal][NoScaleOffset] _layer1Norm("LAYER*B Normal (RGB)", 2D) = "bump" {}
        _layer1Tiling("LAYER*B Tiling", Float) = 1
        _layer1Rough("LAYER*B Roughness", Range(0, 1)) = 1
        [NoScaleOffset] _BlendMask("BLEND*Mask (R)", 2D) = "white" {}
        _BlendTile("BLEND*Tiling", Float) = 1
        _Choke("BLEND*Choke", Range(0, 60)) = 15
        _Crisp("BLEND*Crispyness", Range(1, 20)) = 5

        [NoScaleOffset] _DetailAlbedo("DETAIL*Albedo (R)", 2D) = "grey" {}
        [Normal][NoScaleOffset] _DetailNormal("DETAIL*Normal (RGB)", 2D) = "bump" {}
        _DetailNormalStrength("DETAIL*Normal Strength", Range(0,1)) = 0.4
        _DetailTiling("DETAIL*Tiling", Float) = 2
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
                    float4 color        : COLOR;
                };

                TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
                TEXTURE2D(_BumpMap); SAMPLER(sampler_BumpMap);
                TEXTURE2D(_DetailAlbedo); SAMPLER(sampler_DetailAlbedo);
                TEXTURE2D(_DetailNormal); SAMPLER(sampler_DetailNormal);
                TEXTURE2D(_layer1Tex); SAMPLER(sampler_layer1Tex);
                TEXTURE2D(_layer1Norm); SAMPLER(sampler_layer1Norm);
                TEXTURE2D(_BlendMask); SAMPLER(sampler_BlendMask);

                CBUFFER_START(UnityPerMaterial)
                    float4 _Color;
                    float4 _MainTex_ST;
                    float _Roughness;
                    float _layer1Tiling;
                    float _layer1Rough;
                    float _BlendTile;
                    float _Choke;
                    float _Crisp;
                    float _DetailNormalStrength;
                    float _DetailTiling;
                CBUFFER_END

                Varyings vert(Attributes IN) {
                    Varyings OUT;
                    OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                    OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                    OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                    OUT.tangentWS = float4(TransformObjectToWorldDir(IN.tangentOS.xyz), IN.tangentOS.w);
                    OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                    OUT.color = IN.color;
                    return OUT;
                }

                half4 frag(Varyings IN) : SV_Target {
                    // Base layer textures
                    half4 main = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                    main = lerp(main, _Color, main.a * _Color.a);
                    half3 normal = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, IN.uv));

                    // Layer1 Textures
                    half3 layer1Albedo = SAMPLE_TEXTURE2D(_layer1Tex, sampler_layer1Tex, IN.uv * _layer1Tiling);
                    half3 layer1Normal = UnpackNormal(SAMPLE_TEXTURE2D(_layer1Norm, sampler_layer1Norm, IN.uv * _layer1Tiling));

                    half blendMask = SAMPLE_TEXTURE2D(_BlendMask, sampler_BlendMask, IN.uv * _BlendTile).r;
                    blendMask = clamp(blendMask, 0.2, 0.9);

                    // Detail Normal Texture
                    half detailAlbedo = SAMPLE_TEXTURE2D(_DetailAlbedo, sampler_DetailAlbedo, IN.uv * _DetailTiling).r;
                    half3 detailNormal = UnpackNormal(SAMPLE_TEXTURE2D(_DetailNormal, sampler_DetailNormal, IN.uv * _DetailTiling));

                    // Blend Mask
                    half blend = (IN.color.r * blendMask) * _Choke;
                    blend = pow(blend, _Crisp);
                    blend = saturate(blend);

                    // Blended textures
                    half3 blendedAlbedo = lerp(layer1Albedo, main.rgb, blend);
                    blendedAlbedo = blendedAlbedo * lerp(1, detailAlbedo * 2, 0.5);

                    half3 blendedNormal = lerp(layer1Normal, normal, blend);
                    blendedNormal = blendedNormal + (detailNormal * half3(_DetailNormalStrength, _DetailNormalStrength, 0));
                    half blendedSmoothness = lerp(_layer1Rough, _Roughness, blend);

                    // Calculate world space normal
                    float3 normalWS = TransformTangentToWorld(blendedNormal,
                        half3x3(IN.tangentWS.xyz, cross(IN.normalWS, IN.tangentWS.xyz) * IN.tangentWS.w, IN.normalWS));

                    // Lighting calculation
                    InputData inputData = (InputData)0;
                    inputData.positionWS = IN.positionWS;
                    inputData.normalWS = normalize(normalWS);
                    inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                    inputData.shadowCoord = TransformWorldToShadowCoord(IN.positionWS);

                    SurfaceData surfaceData = (SurfaceData)0;
                    surfaceData.albedo = blendedAlbedo;
                    surfaceData.metallic = 0;
                    surfaceData.smoothness = saturate(1 - blendedSmoothness);
                    surfaceData.normalTS = blendedNormal;

                    half4 color = UniversalFragmentPBR(inputData, surfaceData);
                    return color;
                }
                ENDHLSL
            }

            // You may need to add additional passes like ShadowCaster here
        }
            FallBack "Universal Render Pipeline/Lit"
}