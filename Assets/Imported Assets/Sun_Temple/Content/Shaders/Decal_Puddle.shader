Shader "Sun_Temple/Decal_Puddle_URP"
{
    Properties
    {
        _Color("Color", Color) = (0.5, 0.5, 0.5, 0)
        _Mask("Mask (R)", 2D) = "black" {}
        _MaskFade("Mask Fade", Range(0, 1)) = 0
        [Normal] _BumpMap("Normal Map", 2D) = "bump" {}
        _Roughness("Roughness", Range(0, 1)) = 0
        _ScrollSpeed("Scroll Speed", Range(0, 4)) = 2
    }

        SubShader
        {
            Tags {"RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent"}
            LOD 200

            HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            ENDHLSL

            Pass
            {
                Name "Forward"
                Tags {"LightMode" = "UniversalForward"}

                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite Off
                Cull Back

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

                TEXTURE2D(_Mask);
                TEXTURE2D(_BumpMap);
                SAMPLER(sampler_Mask);
                SAMPLER(sampler_BumpMap);

                CBUFFER_START(UnityPerMaterial)
                    float4 _Color;
                    float _MaskFade;
                    float _Roughness;
                    float _ScrollSpeed;
                    float4 _Mask_ST;
                    float4 _BumpMap_ST;
                CBUFFER_END

                Varyings vert(Attributes input)
                {
                    Varyings output;
                    VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                    output.positionCS = vertexInput.positionCS;
                    output.positionWS = vertexInput.positionWS;
                    output.uv = TRANSFORM_TEX(input.uv, _Mask);
                    output.normalWS = normalInput.normalWS;
                    output.tangentWS = float4(normalInput.tangentWS, input.tangentOS.w);

                    return output;
                }

                half4 frag(Varyings input) : SV_Target
                {
                    float scrollX = _ScrollSpeed * _Time.y;
                    float scrollY = _ScrollSpeed * _Time.y;

                    float2 uv1 = input.uv + float2(scrollX, scrollY);
                    float2 uv2 = input.uv - float2(scrollX, scrollY);

                    half3 normalA = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv1));
                    half3 normalB = UnpackNormal(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv2));
                    half3 normalCombined = normalA + normalB;

                    float3 normalWS = TransformTangentToWorld(normalCombined,
                        half3x3(input.tangentWS.xyz, cross(input.normalWS, input.tangentWS.xyz) * input.tangentWS.w, input.normalWS));

                    half alpha = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, input.uv).r;
                    alpha = lerp(alpha, 0, _MaskFade);

                    InputData inputData = (InputData)0;
                    inputData.positionWS = input.positionWS;
                    inputData.normalWS = NormalizeNormalPerPixel(normalWS);
                    inputData.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);

                    SurfaceData surfaceData = (SurfaceData)0;
                    surfaceData.albedo = _Color.rgb;
                    surfaceData.metallic = _Color.a;
                    surfaceData.smoothness = saturate(1 - _Roughness);
                    surfaceData.normalTS = normalCombined;
                    surfaceData.alpha = alpha;

                    half4 color = UniversalFragmentPBR(inputData, surfaceData);
                    return color;
                }
                ENDHLSL
            }
        }
            FallBack "Universal Render Pipeline/Lit"
}