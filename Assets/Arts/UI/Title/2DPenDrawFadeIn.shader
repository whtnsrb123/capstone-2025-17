Shader "Custom/2DPenDrawFadeIn_UV"
{
    Properties
    {
        _PenUV ("Pen UV (0~1)", Vector) = (0.5,0.5,0,0)
        _Radius ("Radius", Float) = 0.1
        _FadeAmount ("Fade Step", Range(0,1)) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            ZWrite Off
            Cull Off
            Blend One One // Additive

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _PenUV; // ¹Ýµå½Ã float4!
            float _Radius;
            float _FadeAmount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = distance(i.uv, _PenUV.xy);
                float fade = _FadeAmount * (1.0 - smoothstep(_Radius * 0.5, _Radius, dist));
                return fixed4(fade, fade, fade, fade);
            }
            ENDHLSL
        }
    }
}