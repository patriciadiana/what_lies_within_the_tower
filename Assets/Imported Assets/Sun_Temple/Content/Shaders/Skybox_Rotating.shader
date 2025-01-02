Shader "Sun_Temple/Skybox_Rotating_URP"
{
    Properties
    {
        _Color("Color", Color) = (.5, .5, .5, .5)
        _FogColorMix("Match Fog Color", Range(0, 1)) = 0
        [Gamma] _Exposure("Exposure", Range(0, 8)) = 1.0
        _Rotation("Rotation", Range(0, 360)) = 0
        [NoScaleOffset] _Tex("Cubemap (HDR)", Cube) = "grey" {}
    }

        SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" "RenderPipeline" = "UniversalPipeline" }
        Cull Off ZWrite Off

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL

        Pass
        {
            Name "Forward"
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURECUBE(_Tex);
            SAMPLER(sampler_Tex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Tex_HDR;
                half4 _Color;
                half _FogColorMix;
                half _Exposure;
                float _Rotation;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float3 RotateAroundYInDegrees(float3 vertex, float degrees)
            {
                float alpha = degrees * PI / 180.0;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float3(mul(m, vertex.xz), vertex.y).xzy;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                _Rotation = _Rotation + (_Time.y * _Rotation);
                float3 rotated = RotateAroundYInDegrees(input.positionOS.xyz, _Rotation);
                output.positionCS = TransformObjectToHClip(rotated);
                output.texcoord = input.positionOS.xyz;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURECUBE(_Tex, sampler_Tex, input.texcoord);
                half3 c = tex.rgb;
                c = lerp(c, unity_FogColor.rgb, _FogColorMix) * _Exposure;
                return half4(c, 1);
            }
            ENDHLSL
        }
    }
        Fallback Off
}