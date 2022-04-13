//Shader "Custom/Portal"
//{
//    Properties
//    {
//        _InactiveColour ("Inactive Colour", Color) = (1, 1, 1, 1)
//    }
//    SubShader
//    {
//        Tags { "RenderType"="Opaque" }
//        LOD 100
//        Cull Off
//
//        Pass
//        {
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #include "UnityCG.cginc"
//
//            struct appdata
//            {
//                float4 vertex : POSITION;
//            };
//
//            struct v2f
//            {
//                float4 vertex : SV_POSITION;
//                float4 screenPos : TEXCOORD0;
//            };
//
//            sampler2D _MainTex;
//            float4 _InactiveColour;
//            int displayMask; // set to 1 to display texture, otherwise will draw test colour
//            
//
//            v2f vert (appdata v)
//            {
//                v2f o;
//                o.vertex = UnityObjectToClipPos(v.vertex);
//                o.screenPos = ComputeScreenPos(o.vertex);
//                return o;
//            }
//
//            fixed4 frag (v2f i) : SV_Target
//            {
//                float2 uv = i.screenPos.xy / i.screenPos.w;
//                fixed4 portalCol = tex2D(_MainTex, uv);
//                return portalCol * displayMask + _InactiveColour * (1-displayMask);
//            }
//            ENDCG
//        }
//    }
//    Fallback "Standard" // for shadows
//}

Shader "Custom/Portal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Bump map", 2D) = "white" {}
        _Refraction("Refraction magnitude", Range(-0.02, 1.0)) = 0.015
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
            #pragma vertex vert
            #pragma fragment frag

            struct vertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct fragmentInput
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 screenUv : TEXCOORD1;
                float3 tangentToWorld[3] : TEXCOORD2;
            };

            fragmentInput vert (vertexInput i)
            {
                fragmentInput o;
                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv = i.uv;
                o.screenUv = float3((o.vertex.xy + o.vertex.w) * 0.5, o.vertex.w);

                o.normal = UnityObjectToWorldNormal(i.normal);
				float4 tangentWorld = float4(UnityObjectToWorldDir(i.tangent.xyz), i.tangent.w);
                const float3x3 tangentToWorld = CreateTangentToWorldPerVertex(o.normal, tangentWorld.xyz, tangentWorld.w);
				o.tangentToWorld[0].xyz = tangentToWorld[0];
				o.tangentToWorld[1].xyz = tangentToWorld[1];
				o.tangentToWorld[2].xyz = tangentToWorld[2];
                
                return o;
            }
            
            TEXTURE2D (MainTex);
            SAMPLER (samplerMainTex);
            TEXTURE2D (BumpMap);
            SAMPLER (samplerBumpMap);
            float _Refraction;

            float4 frag (fragmentInput i) : SV_Target
            {
                float3 tangent = i.tangentToWorld[0].xyz;
				float3 binormal = i.tangentToWorld[1].xyz;
				float3 normal = i.tangentToWorld[2].xyz;
                float3 normalTangent = SAMPLE_TEXTURE2D(BumpMap, samplerBumpMap, i.uv).xyz * 2 - 1;
				//float3 normalTangent = UnpackNormal(SAMPLE_TEXTURE2D(BumpMap, samplerBumpMap, i.uv));
				float3 normalWorld = normalize(tangent * normalTangent.x + binormal * normalTangent.y + normal * normalTangent.z);

                float3 viewSpaceNormal = mul(UNITY_MATRIX_V, normalWorld);
				float2 refractionVector = viewSpaceNormal.xy * viewSpaceNormal.z;

                float2 screenSpaceUv = i.screenUv.xy / i.screenUv.z;
                #ifdef UNITY_UV_STARTS_AT_TOP
                screenSpaceUv.y = 1 - screenSpaceUv.y;
                #endif

                return SAMPLE_TEXTURE2D(BumpMap, samplerBumpMap, screenSpaceUv + refractionVector);
            }
            ENDCG
        }
    }
}
