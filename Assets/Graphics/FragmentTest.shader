Shader "Custom/FragmentTest"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _HighLight ("HighLight", Color) = (1,1,1,1)
        _Size ("Size", Range(0, 100)) = 3
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _Distortion ("Distortion", Range(0, 10)) = .3
        _TimeScale ("Scrolling Speed", Range(0, 2)) = 1
        _Pow ("exponent", Range(0, 10)) = .3
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off 
        LOD 100

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        
        sampler2D _MainTex;
        float4 st;


        float _Size;

        sampler2D _MaskTex;
        float4 _MaskTex_ST;

        float _Distortion;
        float _TimeScale;

        float4 _HighLight;

        float _Pow;
        float _CellSize;


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        float hash( float n )
        {
            return frac(sin(n)*43758.5453);
        }

        float perl( float3 x )
        {
            // The noise function returns a value in the range -1.0f -> 1.0f

            float3 p = floor(x);
            float3 f = frac(x);

            f = f*f*(3.0-2.0*f);
            float n = p.x + p.y*57.0 + 113.0*p.z;

            return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
            lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
            lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
            lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 value = IN.worldPos.xyz / _Size;
            value.z += _Time.y * _TimeScale;
            
            float3 noise = perl(value);
            float3 noise2 = perl(noise + value) * 10;
            float3 noise3 = perl(noise2 + value) * 20;
            float3 noise4 = perl(noise3 + value) * 30;
            float3 finalNoise = noise * noise2 * noise3 * noise4;

            fixed radA = max(1-max(length(half2(.5,.5)-IN.uv_MainTex.xy)-.25,0)/.25,0);

            //Get the mask color from our mask texture;
            half4 mask = tex2D(_MaskTex, IN.uv_MainTex.xy + finalNoise);

            float2 offset = float2(_Distortion*(noise.x-.5),_Distortion*(noise.y-.5));
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex.xy + offset)  * _Color ; //* noise.x * noise2.x;
            float3 final_color = lerp(_HighLight, _Color, c.b * .5).rgb;

            float final_alpha = mask.a;
            final_alpha *= mask.g * mask.r;
            final_alpha *= radA;
            final_alpha = pow(final_alpha, _Pow);


            //fixed4 c = tex2D (_MainTex, IN.uv_MainTex ) * _Color * noise.x * noise2.x;
            o.Albedo = final_color.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = final_alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
