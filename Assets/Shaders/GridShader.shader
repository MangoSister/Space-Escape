Shader "Space Platform/Grid Shader" 
{
	SubShader
	{
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off Cull Off Fog { Mode Off }
			BindChannels
			{
			  Bind "vertex", vertex
			  Bind "color", color
			}
		} 
	}
		//FallBack "Diffuse"
}
