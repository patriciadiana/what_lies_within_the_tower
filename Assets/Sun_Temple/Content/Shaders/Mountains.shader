Shader "Sun_Temple/Mountains_URP"
{
    Properties
    {
        _TerrainNormal("Terrain Normal map (overall)", 2D) = "bump" {}
        _MainTex("Layer A Albedo (RGB)", 2D) = "white" {}
        _BumpMap("LAYER A Normal", 2D) = "bump" {}
        _BaseTiling("LAYER A Tiling", Float) = 1
        _Layer1Tex("LAYER B Albedo (RGB) Smoothness (A)", 2D) = "white" {}
        _Layer1Norm("LAYER B Normal", 2D) = "bump" {}
        _Layer1Tiling("LAYER B Tiling", Float) = 1
        _BlendMask("BLEND Mask", 2D) = "white" {}
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }
        LOD 500

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
            };

            TEXTURE2D(_TerrainNormal);
            TEXTURE2D(_MainTex);
            TEXTURE2D(_BumpMap);
            TEXTURE2D(_Layer1Tex);
            TEXTURE2D(_Layer1Norm);
            TEXTURE2D(_BlendMask);

            SAMPLER(sampler_TerrainNormal);
            SAMPLER(sampler_MainTex);
            SAMPLER(sampler_BumpMap);
            SAMPLER(sampler_Layer1Tex);
            SAMPLER(sampler_Layer1Norm);
            SAMPLER(sampler_BlendMask);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _BaseTiling;
                float _Layer1Tiling;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.normalWS = normalInput.normalWS;
                output.tangentWS = float4(normalInput.tangentWS, input.tangentOS.w);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Base layer textures
                half3 layerA_albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv * _BaseTiling).rgb;
                half3 layerA_normal = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv * _BaseTiling));
                half3 terrainNormal = UnpackNormal(SAMPLE_TEXTURE2D(_TerrainNormal, sampler_TerrainNormal, input.uv));

                // Layer1 Textures
                half3 layerB_albedo = SAMPLE_TEXTURE2D(_Layer1Tex, sampler_Layer1Tex, input.uv * _Layer1Tiling).rgb;
                half3 layerB_normal = UnpackNormal(SAMPLE_TEXTURE2D(_Layer1Norm, sampler_Layer1Norm, input.uv * _Layer1Tiling));

                // Blend Mask
                half blendMask = SAMPLE_TEXTURE2D(_BlendMask, sampler_BlendMask, input.uv).r;

                // Blended textures
                half3 blendedAlbedo = lerp(layerB_albedo, layerA_albedo, blendMask);
                half3 blendedNormal = lerp(layerB_normal, layerA_normal, blendMask);
                half3 finalNormal = terrainNormal + half3(blendedNormal.xy, 0);

                // Calculate world space normal
                float3 normalWS = TransformTangentToWorld(finalNormal,
                    half3x3(input.tangentWS.xyz, cross(input.normalWS, input.tangentWS.xyz) * input.tangentWS.w, input.normalWS));

                // Setup SurfaceData and InputData
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = blendedAlbedo;
                surfaceData.metallic = 0;
                surfaceData.smoothness = 0;
                surfaceData.normalTS = finalNormal;

                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = NormalizeNormalPerPixel(normalWS);
                inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);

                // Calculate final color
                half4 color = UniversalFragmentPBR(inputData, surfaceData);

                return color;
            }
            ENDHLSL
        }
    }
        FallBack "Universal Render Pipeline/Lit"
}