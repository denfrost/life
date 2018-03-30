Shader "Custom/Test"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OutlineColor ("Outline", Color) = (1,1,1,1)
		_Tint ("Tint", Color) = (1,1,1,1)
		_Flag ("Flag", float) = 1
	}
	SubShader{
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 localPosition : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
            fixed4 _OutlineColor;
            float _TexWidth;
            float _TexHeight;
			float4 _Tint;
			uniform float _Flag;

			v2f vert (appdata v){
				v2f o;
				o.localPosition = v.vertex.xyz;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target{
				fixed4 col = tex2D(_MainTex, i.uv);
				if ((col.x != 0 || col.y != 0 || col.z != 0) && (col.x != 1 || col.y != 1 || col.z != 1)){
					int aux = (i.localPosition.x + i.localPosition.y + i.localPosition.z) % 2;
					//return float4(i.localPosition, 1);
					return float4(aux, aux, aux, 1);
					//return float4(1, 1, 1, 1);
				}
				//else{ return col; }
				

				int up = int(tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y)).r);
                int down = int(tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y)).r);
                int left = int(tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x, 0)).r);
                int right = int(tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, 0)).r);
				int upR = int(tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y)).r);
                int downR = int(tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, -_MainTex_TexelSize.y)).r);
                int upL = int(tex2D(_MainTex, i.uv + fixed2(-_MainTex_TexelSize.x, _MainTex_TexelSize.y)).r);
                int downL = int(tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y)).r);
				
				int count = up + down + left + right + upR + downR + upL + downL;

				//Death
				if (count < 2 || count > 3)
 					return float4(0, 0, 0, 1);
				// Life
				if (count == 3)
 					return float4(1, 1, 1, 1);
				// Stay
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
                //return _Tint;
			}
			ENDCG
		}
	}
}
