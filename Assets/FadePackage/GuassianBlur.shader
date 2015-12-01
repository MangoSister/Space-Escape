Shader "Custom/GuassianBlur" {
	Properties {
		_MainTex ("Main", 2D) = "white" {}
		_Offset ("offset", float) = 0
	}
	SubShader {
		LOD 200
		
		pass{
			
			 Tags { "LightMode" = "Always" }
               
              CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag_realGuassian
				#include "UnityCG.cginc"
				#pragma profileoption NumTemps=64

				sampler2D _MainTex;
				float _Offset;
				uniform float offsets[5] = { 0.0, 1.0, 2.0, 3.0, 4.0 };
				uniform float weight[5] = { 0.2270270270, 0.1945945946, 0.1216216216, 0.0540540541, 0.0162162162 };

				
				
				struct v2f {
				    float4  pos : SV_POSITION;
				    float2  uv : TEXCOORD0;
				};

				float4 _MainTex_ST;

				v2f vert (appdata_base v)
				{
				    v2f o;
				    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				    return o;
				};

				half4 frag_simple (v2f i) : COLOR{				    
				    float4 texcol = float4(0,0,0,0);
				    float remaining=1.0f;
				    float coef=1.0;
				    float fI=0;
				    for (int j = 0; j < 3; j++) {
				    	fI++;
				    	coef*=0.32;
				    	texcol += tex2D(_MainTex, float2(i.uv.x, i.uv.y - fI * _Offset)) * coef;
				    	texcol += tex2D(_MainTex, float2(i.uv.x - fI * _Offset, i.uv.y)) * coef;
				    	texcol += tex2D(_MainTex, float2(i.uv.x + fI * _Offset, i.uv.y)) * coef;
				    	texcol += tex2D(_MainTex, float2(i.uv.x, i.uv.y + fI * _Offset)) * coef;
				    	
				    	remaining-=4*coef;
				    }
				    texcol += tex2D(_MainTex, float2(i.uv.x, i.uv.y)) * remaining;

				    return texcol;
				}
				
				half4 frag_guassian(v2f i): COLOR{
					 float4 FragmentColor = tex2D( _MainTex, float2(i.uv.x, i.uv.y)) * weight[0];
					
					 FragmentColor += tex2D( _MainTex, float2(i.uv.x, i.uv.y + offsets[0]) ) * weight[0];
				     FragmentColor += tex2D( _MainTex, float2(i.uv.x + offsets[0], i.uv.y) ) * weight[0];
				     
				      FragmentColor += tex2D( _MainTex, float2(i.uv.x, i.uv.y + offsets[1]) ) * weight[1];
				     FragmentColor += tex2D( _MainTex, float2(i.uv.x + offsets[1], i.uv.y) ) * weight[1];
				     
				      FragmentColor += tex2D( _MainTex, float2(i.uv.x, i.uv.y + offsets[2]) ) * weight[2];
				     FragmentColor += tex2D( _MainTex, float2(i.uv.x + offsets[2], i.uv.y) ) * weight[2];
				     
				      FragmentColor += tex2D( _MainTex, float2(i.uv.x, i.uv.y + offsets[3]) ) * weight[3];
				     FragmentColor += tex2D( _MainTex, float2(i.uv.x + offsets[3], i.uv.y) ) * weight[3];
				     
				      FragmentColor += tex2D( _MainTex, float2(i.uv.x, i.uv.y + offsets[4]) ) * weight[4];
				     FragmentColor += tex2D( _MainTex, float2(i.uv.x + offsets[4], i.uv.y) ) * weight[4];
				        
				 // return FragmentColor;
					//return float4(1,0,0,1);
				}
				
				half4 frag_realGuassian(v2f i):COLOR {
				
					 float blurAmount = _Offset;//0.0075;
 
	                 float4 sum = float4(0, 0, 0,0);
	 
	                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y - 5.0 * blurAmount)) * 0.025;
	                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y - 4.0 * blurAmount)) * 0.05;
	                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y - 3.0 * blurAmount)) * 0.09;
	                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y - 2.0 * blurAmount)) * 0.12;
	                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y - blurAmount)) * 0.15;
	                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y)) * 0.16;
	                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y + blurAmount)) * 0.15;
	                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y + 2.0 * blurAmount)) * 0.12;
	                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y + 3.0 * blurAmount)) * 0.09;
	                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y + 4.0 * blurAmount)) * 0.05;
	                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y + 5.0 * blurAmount)) * 0.025;
	                 
	                 sum += tex2D(_MainTex, float2(i.uv.x - 5.0 * blurAmount , i.uv.y )) * 0.025;
	                 sum += tex2D(_MainTex, float2(i.uv.x - 4.0 * blurAmount , i.uv.y )) * 0.05;
	                 sum += tex2D(_MainTex, float2(i.uv.x - 3.0 * blurAmount , i.uv.y )) * 0.09;
	                 sum += tex2D(_MainTex, float2(i.uv.x - 2.0 * blurAmount , i.uv.y )) * 0.12;
	                 sum += tex2D(_MainTex, float2(i.uv.x -  blurAmount , i.uv.y )) * 0.15;
	                 sum += tex2D(_MainTex, float2(i.uv.x , i.uv.y)) * 0.16;
	                 sum += tex2D(_MainTex, float2(i.uv.x + blurAmount , i.uv.y + blurAmount)) * 0.15;
	                 sum += tex2D(_MainTex, float2(i.uv.x + 2.0 * blurAmount , i.uv.y )) * 0.12;
	                 sum += tex2D(_MainTex, float2(i.uv.x + 3.0 * blurAmount , i.uv.y )) * 0.09;
	                 sum += tex2D(_MainTex, float2(i.uv.x + 4.0 * blurAmount , i.uv.y )) * 0.05;
	                 sum += tex2D(_MainTex, float2(i.uv.x + 5.0 * blurAmount , i.uv.y )) * 0.025;
				
					//sum =  sum/11;
					return sum;
					
				}
				
				ENDCG
            }
            
           
				
		}
		FallBack "Diffuse"
	} 
	



