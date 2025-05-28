Shader "Custom/GlowBeam"
{
    Properties
    {
        _Color ("Glow Color", Color) = (0.2,0.8,1,1)
        _GlowStrength ("Glow Strength", Range(0, 5)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
            Blend SrcAlpha One
        ZWrite Off
        Lighting Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;
            float _GlowStrength;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color * _GlowStrength;
            }
            ENDCG
        }
    }
}
