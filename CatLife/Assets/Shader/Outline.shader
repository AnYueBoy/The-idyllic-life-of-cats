Shader "Unlit/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineWidth("OutlineWidth",Range(0,1)) = 0.1
        _OutlineColor("OutlineColor",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineWidth;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 result = tex2D(_MainTex, i.uv);
                float leftAlpha = tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(-1, 0) * _OutlineWidth).a;
                float rightAlpha = tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(1, 0) * _OutlineWidth).a;
                float upAlpha = tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(0, 1) * _OutlineWidth).a;
                float downAlpha = tex2D(_MainTex, i.uv + _MainTex_TexelSize.xy * float2(0, -1) * _OutlineWidth).a;
                float resultAlpha = leftAlpha * rightAlpha * upAlpha * downAlpha;
                return float4(lerp(_OutlineColor.rgb, result.rgb, resultAlpha), result.a);
            }
            ENDCG
        }
    }
}