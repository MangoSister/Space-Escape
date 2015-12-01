Shader "Custom/Fade" {

	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_TargetTex ("Target", 2D) = "white" {}
		_Offset ("offset", float) = 0
	}

	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
					
	CGPROGRAM
	#pragma vertex vert_img
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest 
	#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	uniform sampler2D _TargetTex;
	uniform half _Offset;	
	
	fixed4 frag (v2f_img i) : COLOR
	{
		fixed4 main = tex2D(_MainTex, i.uv);
		fixed4 target = tex2D(_TargetTex, half2(0,0));
		fixed4 output= main * (1 -_Offset ) + target * _Offset;
		return output;
	}
	ENDCG

		}
	}

	Fallback off

}