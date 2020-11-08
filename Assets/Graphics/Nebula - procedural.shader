Shader "Hidden/Nebula - procedural"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _HighLight ("HighLight", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "black" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _Distortion ("Distortion", Range(0, 10)) = .3
        _TimeScale ("Scrolling Speed", Range(0, 2)) = 1
        _Pow ("exponent", Range(0, 10)) = .3
        _CellSize ("cellSize", Range(0, 100)) = 1

    }
    SubShader
    {

        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off 
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "random.inc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            sampler2D _MaskTex;
            float4 _MaskTex_ST;

            float _Distortion;
            float _TimeScale;

            float4 _Color;
            float4 _HighLight;

            float _Pow;
            float _CellSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 tex : TEXCOORD0;
            };

            struct v2f
            {
                float2 tex : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos: TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.tex = v.tex;
                return o;
            }

            float hash( float n )
            {
                return frac(sin(n)*43758.5453);
            }

            float simplex( float3 x )
            {
                // The noise function returns a value in the range -1.0f -> 1.0f

                float3 p = floor(x);
                float3 f = frac(x);

                f       = f*f*(3.0-2.0*f);
                float n = p.x + p.y*57.0 + 113.0*p.z;
                return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
                lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
                lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
                lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
            }

            float easeIn(float interpolator){
                return interpolator * interpolator * interpolator * interpolator * interpolator;
            }

            float easeOut(float interpolator){
                return 1 - easeIn(1 - interpolator);
            }

            float easeInOut(float interpolator){
                float easeInValue = easeIn(interpolator);
                float easeOutValue = easeOut(interpolator);
                return lerp(easeInValue, easeOutValue, interpolator);
            }

            float gradientNoise(float value){
                float fraction = frac(value);
                float interpolator = easeInOut(fraction);

                float3 previousCellInclination = rand1dTo3d(floor(value)) * 2 - 1;
                float3 previousCellLinePoint = previousCellInclination * fraction;

                float3 nextCellInclination = rand1dTo3d(ceil(value)) * 2 - 1;
                float3 nextCellLinePoint = nextCellInclination * (fraction - 1);

                return lerp(previousCellLinePoint, nextCellLinePoint, interpolator);
            }

            float4 frag(v2f i) : COLOR
            {
                //float3 value = ( i.worldPos / _CellSize);
                float3 value = ( i.vertex / _CellSize);
                value.z += _Time.y * _TimeScale;

                float3 baseNoise = simplex(value * _Distortion);
                float3 noise = baseNoise;
                float3 noise1 = simplex(noise * 2);
                float3 noise2 = simplex(noise1 * 4);
                float3 noise3 = simplex(noise2 * 10);
                float3 finalNoise = noise3 + noise2 + noise1;
                
                return gradientNoise(value * _Distortion )* 0.5 + 0.5 ;

                half4 col = half4(finalNoise* 0.5 + 0.5, 1);
                
                float2 offset = float2(noise.z * (col.x - 0.5), noise.x * (col.y - 0.5));
                
                half4 col2 = half4(saturate(noise1) * _CellSize, 1);
                
                //Create a circular mask: if we're close to the edge the value is 0
                //If we're by the center the value is 1
                //By multipling the final alpha by this, we mask the edges of the box
                fixed radA = max(1-max(length(half2(.5,.5)-i.tex.xy)-.25,0)/.25,0);

                //Get the mask color from our mask texture;
                //half4 mask = tex2D(_MaskTex,i.tex.xy*_MaskTex_ST.xy + _MaskTex_ST.zw);
                half4 mask = half4(finalNoise, 1);
                //Add the color portion : apply the gradient from the highlight to the color
                //To the gray value from the blue channel of the distorted noise
                float3 final_color = lerp(_HighLight, _Color, noise2.z).rgb;
                
                //calculate the final alpha value:
                //First combine several of the distorted noises together/
                float final_alpha = finalNoise.z / 2;

                //Apply the a combination of two tendril masks/
                
                final_alpha *= radA;
                //final_alpha *= mask.g*mask.r;

                //Apply the circular mask
                
                //Raise it to a power to dim it a bit 
                //it should be between 0 and 1, so the higher the power
                //the more transparent it becomes
                final_alpha = pow(final_alpha, _Pow);
                
                //Finally, makes sure its never more than 90% opaque
                final_alpha = min(final_alpha, .9);
                
                //We're done! Return the final pixel color!
                //return float4(final_color, final_alpha);
                return float4(final_color, final_alpha);
            }
            ENDCG
        }
    }
}
