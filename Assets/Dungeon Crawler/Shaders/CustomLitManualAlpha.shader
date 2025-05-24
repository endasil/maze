//Shader "Custom/LitManualAlpha"
//{
//    Properties
//    {
//        _BumpMap ("Normal Map", 2D) = "bump" {}
//        _Metallic ("Metallic", Range(0,1)) = 0
//        _Smoothness ("Smoothness", Range(0,1)) = 0.5
//        _ManualAlpha ("Alpha", Range(0,1)) = 1.0
//        _Cull ("Cull", Float) = 2 // 0: Off, 1: Front, 2: Back
//    }
//
//    SubShader
//    {
//        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
//
//        Pass
//        {
//            Name "ForwardLit"
//            Cull [_Cull]
//            ZWrite Off
//            Blend SrcAlpha OneMinusSrcAlpha
//
//            HLSLPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
//            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
//
//            struct Attributes
//            {
//                float4 positionOS : POSITION;
//                float2 uv : TEXCOORD0;
//                float3 normalOS : NORMAL;
//                float4 tangentOS : TANGENT;
//            };
//
//            struct Varyings
//            {
//                float4 positionHCS : SV_POSITION;
//                float2 uv : TEXCOORD0;
//                float3 normalWS : TEXCOORD1;
//                float3 tangentWS : TEXCOORD2;
//                float3 bitangentWS : TEXCOORD3;
//                float3 viewDirWS : TEXCOORD4;
//            };
//            
//            float4 _BaseColor;
//            float _Metallic;
//            float _Smoothness;
//            float _ManualAlpha;
//
//            Varyings vert (Attributes IN)
//            {
//                Varyings OUT;
//
//                float4 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
//                OUT.positionHCS = TransformWorldToHClip(worldPos.xyz);
//
//                VertexNormalInputs normalInput = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);
//                OUT.normalWS = normalInput.normalWS;
//                OUT.tangentWS = normalInput.tangentWS;
//                OUT.bitangentWS = normalInput.bitangentWS;
//
//                OUT.viewDirWS = GetWorldSpaceViewDir(worldPos.xyz);
//                OUT.uv = IN.uv;
//
//                return OUT;
//            }
//
//            half4 frag (Varyings IN) : SV_Target
//            {
//                SurfaceData surfaceData;
//                InputData inputData;
//
//                inputData.positionWS = float3(0,0,0); // Not used here
//                inputData.normalWS = normalize(IN.normalWS);
//                inputData.viewDirectionWS = normalize(IN.viewDirWS);
//                inputData.tangentWS = IN.tangentWS;
//                inputData.bitangentWS = IN.bitangentWS;
//
//                half4 albedoSample = tex2D(_BaseMap, IN.uv);
//                float3 baseColor = albedoSample.rgb * _BaseColor.rgb;
//                float alpha = _ManualAlpha;
//
//                float3 normalTS = UnpackNormal(tex2D(_BumpMap, IN.uv));
//                float3 normalWS = TransformTangentToWorld(normalTS, IN.tangentWS, IN.bitangentWS, IN.normalWS);
//                inputData.normalWS = normalize(normalWS);
//
//                surfaceData.albedo = baseColor;
//                surfaceData.alpha = alpha;
//                surfaceData.metallic = _Metallic;
//                surfaceData.smoothness = _Smoothness;
//                surfaceData.normalTS = normalTS;
//                surfaceData.occlusion = 1;
//                surfaceData.emission = 0;
//
//                return UniversalFragmentPBR(inputData, surfaceData);
//            }
//
//            ENDHLSL
//        }
//    }
//
//    FallBack "Hidden/InternalErrorShader"
//}
