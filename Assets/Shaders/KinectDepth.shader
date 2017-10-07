// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/KinectDepth" {
	Properties {
		_DepthBuffer ("Depth Buffer", 2DArray) = "" {}
        _SliceRange ("Slices", Range(0,16)) = 6
        _UVScale ("UVScale", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		// to use texture arrays we need to target DX10/OpenGLES3 which
		// is shader model 3.5 minimum
		#pragma target 3.5
		
		#include "UnityCG.cginc"

		struct v2f
		{
			float3 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		float _SliceRange;
		float _UVScale;

		v2f vert (float4 vertex : POSITION)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(vertex);
			o.uv.xy = (vertex.xy + 0.5) * _UVScale;
			o.uv.z = (vertex.z + 0.5) * _SliceRange;
			return o;
		}
		
		UNITY_DECLARE_TEX2DARRAY(_DepthBuffer);

		half4 frag (v2f i) : SV_Target
		{
			return UNITY_SAMPLE_TEX2DARRAY(_DepthBuffer, i.uv);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
