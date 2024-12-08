Shader "Sun_Temple/Decal_URP"
{
    Properties
    {
        _Color("Color Tint (RGB), Fade (A)", Color) = (0.5, 0.5, 0.5, 0)
        _MainTex("Albedo (RGB), Alpha (A)", 2D) = "white" {}
        _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.7
        [NoScaleOffset] _DetailAlbedo("DETAIL Albedo", 2D) = "grey" {}
        _DetailTiling("DETAIL Tiling", Float) = 2
    }

        SubShader
        {
            Tags {"Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "RenderPipeline" = "UniversalPipeline"}
            LOD 400

            HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            ENDHLSL

            Pass
            {
                Name "ForwardLit"
                Tags {"LightMode" = "UniversalForward"}

                Cull Back
                ZTest LEqual
                ZWrite On

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
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normalWS : TEXCOORD1;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                TEXTURE2D(_MainTex);
                TEXTURE2D(_DetailAlbedo);
                SAMPLER(sampler_MainTex);
                SAMPLER(sampler_DetailAlbedo);

                CBUFFER_START(UnityPerMaterial)
                    float4 _Color;
                    float4 _MainTex_ST;
                    float _Cutoff;
                    float _DetailTiling;
                CBUFFER_END

                Varyings vert(Attributes input)
                {
                    Varyings output = (Varyings)0;
                    UNITY_SETUP_INSTANCE_ID(input);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                    output.positionCS = vertexInput.positionCS;
                    output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                    output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                    return output;
                }

                half4 frag(Varyings input) : SV_Target
                {
                    half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                    half detailAlbedo = SAMPLE_TEXTURE2D(_DetailAlbedo, sampler_DetailAlbedo, input.uv * _DetailTiling).r * 2;
                    albedo.rgb = albedo.rgb * _Color.rgb * lerp(1, detailAlbedo, 0.5);

                    half alphaMask = albedo.a * detailAlbedo * _Color.a;

                    clip(alphaMask - _Cutoff);

                    half3 finalColor = lerp(_Color.rgb, albedo.rgb, alphaMask);

                    InputData inputData = (InputData)0;
                    inputData.normalWS = normalize(input.normalWS);
                    inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionCS);

                    SurfaceData surfaceData = (SurfaceData)0;
                    surfaceData.albedo = finalColor;
                    surfaceData.alpha = alphaMask;
                    surfaceData.metallic = 0;
                    surfaceData.smoothness = 0;

                    half4 color = UniversalFragmentBlinnPhong(inputData, surfaceData);
                    return color;
                }
                ENDHLSL
            }
        }
            FallBack "Universal Render Pipeline/Lit"
}