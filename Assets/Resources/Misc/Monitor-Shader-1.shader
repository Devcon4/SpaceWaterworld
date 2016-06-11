Shader "Devcon/Monitor-Shader-1" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		
		_LCDTex("LCD (RGB)", 2D) = "white" {}
		_Pixels("Pixels", Vector) = (1,1,0,0)
		_LCDPixels("LCD Pixels", Vector) = (3,3,0,0)
	
		_Frq("Frequency", Float) = 1.0
		_Amp("Amplitude", Float) = 1.0
		_Oct("Gain", Int) = 1.0
		_Durration("Time", Float) = 1.0
	
		_DistanceOne("Distace of full effect", Float) = 1
		_DistanceZero("Distance of zero effect", Float) = 2
	}
	SubShader {
		Tags { "RenderType"="Opaque"}
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma multi_compile DUMMY PIXELSNAP_ON
		#include "SNNG.cginc"
		#include "UnityCG.cginc"
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 localPos;
		};

		half _Glossiness;
		half _Metallic;

		sampler2D _LCDTex;
		int2 _Pixels;
		int2 _LCDPixels;
		
		uniform float _Durration;
		uniform float _Frq;
		uniform float _Amp;
		uniform int _Oct;
		
		float _DistanceOne;
		float _DistanceZero;
		
		void vert(inout appdata_full v, out Input o) {
			o.localPos = v.vertex.xyz;
			v.vertex = UnityPixelSnap(v.vertex);
			
			UNITY_INITIALIZE_OUTPUT(Input, o);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			float2 uv = round(IN.uv_MainTex * _Pixels.xy + 0.5) / _Pixels.xy;
			fixed4 a = tex2D(_MainTex, uv);
			
			
			float2 finalPos = float2(IN.uv_MainTex.y + (_Time.y * _Durration), 1);
			float noise = FractalNoise(finalPos, _Oct, _Frq, _Amp);
			
			float2 uv_lcd = IN.uv_MainTex * _Pixels.xy / _LCDPixels;
			fixed4 d = tex2D(_LCDTex, uv_lcd);
			
			float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
			float alpha = saturate((dist - _DistanceOne) / (_DistanceZero-_DistanceOne));
			
			o.Albedo = lerp(a * d, a, alpha) * (noise + 1);
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Emission = o.Albedo;
			o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
