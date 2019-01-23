Shader "Unlit/Gird"
{
	Properties{
		_Color("Color Tint" , Color) = (1,1,1,1)
		_MainTex("Main Tex" , 2D) = "white"{}
	}

	SubShader{

		Tags {"Queue" = "Transparent+20" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

		Pass{
			ZWrite Off
			ZTest Off
			Blend SrcAlpha OneMinusSrcAlpha

			Cull Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct a2v {
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				return o;
			}

			fixed4 frag(v2f v) : SV_Target{
				fixed4 texColor = tex2D(_MainTex, v.uv);
				return fixed4(_Color.rgb, texColor.a * _Color.a * 1.2);
			}

			ENDCG
		}
	}
}
