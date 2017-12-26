// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Cus/Demo_3"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            //自定义结构体，包含位置和颜色语义
            struct v2f
            {
                float4 pos : POSITION;
                float4 col : COLOR;
            };
            //Vertex shader入口，颜色信息也在此一并处理了
            v2f vert (appdata_base v)
            {
                v2f o;
                //计算旋转角度，利用_SinTime.w为旋转角度加上周期变换性质（_SinTime 是Unity提供的内置变量）
                float angle = length(v.vertex)* _SinTime.w;
                //绕Y轴旋转矩阵
                float4x4 RM={
                    float4(cos(angle) , 0 , sin(angle) , 0),
                    float4(0 , 1 ,0 , 0),
                    float4(-1 * sin(angle) , 0 , cos(angle),0),
                    float4(0 , 0 ,0 ,1)
                };
                //利用RM矩阵影响顶点位置信息
                float4 pos = mul(RM , v.vertex);
                //把顶点信息转换到世界坐标系中
                o.pos = UnityObjectToClipPos(pos);
                
                //由顶点到中心点的距离决定颜色信息
                angle = abs(sin(length(v.vertex)));
                o.col = float4(angle , 1 , 0 ,1);
                return o;
            }
            //片段程序中直接返回顶点Shader中计算得到的颜色信息
            float4 frag (v2f v) : color
            {
                return v.col;
            }
            ENDCG
        }
    }
}