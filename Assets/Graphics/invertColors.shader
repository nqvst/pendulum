Shader "Sprites/Invert"
{
    Properties 
    {
        _Color ("Color", Color) = (1,1,1,1)
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
    }
    SubShader 
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
 
        Cull Off
        Lighting Off
        ZWrite Off
        Blend OneMinusDstColor OneMinusSrcAlpha

        PASS
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0 Alpha:Blend

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color;
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float _AlphaSplitEnabled;

            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);
                #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                    if (_AlphaSplitEnabled)
                        color.a = tex2D (_AlphaTex, uv).r;
                #endif
  
                return color;
            }
  

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 _Color = SampleSpriteTexture (i.uv) * i.color;
                _Color.rgb *= _Color.a;
                return _Color;
            }
            ENDCG
        }
    }
    FallBack "Sprites"
}