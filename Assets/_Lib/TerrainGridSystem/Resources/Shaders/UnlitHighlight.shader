Shader "Terrain Grid System/Unlit Highlight" {
Properties {
    _Color ("Tint Color", Color) = (1,1,1,0.5)
    _Intensity ("Intensity", Range(0.0, 2.0)) = 1.0
    _Offset ("Depth Offset", Int) = -1
}
SubShader {
    Tags {
        "Queue"="Geometry+230"
        "IgnoreProjector"="True"
        "RenderType"="Transparent"
    }
    
	Offset [_Offset], [_Offset]
	Cull Off		
	Lighting Off	
	ZWrite Off		
	ZTest Always	
	Fog { Mode Off }
	Blend SrcAlpha OneMinusSrcAlpha
	
	CGPROGRAM
    #pragma surface surf Unlit alpha

    struct Input {
        float2 dummy;
    };
    
    half4 _Color;
    half _Intensity;
      
	half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half atten) {
	   half4 c;
       c.rgb = s.Albedo * atten;
       c.a = s.Alpha;
       return c;
    }
                
    void surf (Input IN, inout SurfaceOutput o) {
        o.Albedo = _Color.rgb * _Intensity;
        o.Alpha = _Color.a;
    }
    ENDCG
    	
	
//	Pass {
//		CGPROGRAM						
//			#pragma target 2.0				
//			#pragma fragment frag			
//			#pragma vertex vert				
//			#pragma addshadow
//			#include "UnityCG.cginc"	
//
//			fixed4 _Color;			
//			float _Intensity;
//
//			struct AppData {
//				float4 vertex : POSITION;		
//			};
//
//			struct VertexToFragment {
//				float4 pos : SV_POSITION;
//			};
//
//			//Vertex shader
//			VertexToFragment vert(AppData v) {
//				VertexToFragment o;					
//				o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
//				return o;							
//			}
//
//			fixed4 frag(VertexToFragment i) : SV_Target {
//				return _Color * _Intensity;			
//			}
//
//			ENDCG
//
//		}
	}	
}
