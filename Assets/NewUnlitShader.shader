﻿Shader "Custom/FogOfWar"  {
	Properties{
			_Color("Main Color", Color) = (1,1,1,1)
			_MainTex("Base (RGB) Trans (A)", 2D) = "white"{}
	}

		SubShader{
			Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
			LOD 200
		  Pass {

		CGPROGRAM
		#pragma surface surf Lambert alpha alpha:blend


		sampler2D _MainTex;
		fixed4 _Color;

		struct Input {
			float2 uv_Maintex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rpg;
			o.Alpha = c.a;
		}
		ENDCG
	}
	}
		Fallback "/Transparent/VertexLit"
}
