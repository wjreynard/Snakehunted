Shader "Gameboy"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Darkest("Darkest", color) = (0.0588235, 0.21961, 0.0588235)
        _Dark("Dark", color) = (0.188235, 0.38431, 0.188235)
        _Light("Light", color) = (0.545098, 0.6745098, 0.0588235)
        _Lightest("Lightest", color) = (0.607843, 0.7372549, 0.0588235)
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float4 _Darkest, _Dark, _Light, _Lightest;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 originalColor = tex2D(_MainTex, i.uv);  // get current pixel value
                float luma = dot(originalColor.rgb, float3(0.2126, 0.7152, 0.0722));    // calculating pixel brightness
                float posterized = floor(luma * 4) / (4 - 1);   // flooring the luma value times number of steps, divided by number of steps minus 1

                // original lerping technique by Taylor Bai-Woo
                float lumaTimesThree = posterized * 3.0;

                float darkest = saturate(lumaTimesThree);
                float4 color = lerp(_Darkest, _Dark, darkest);

                float light = saturate(lumaTimesThree - 1.0);
                color = lerp(color, _Light, light);

                float lightest = saturate(lumaTimesThree - 2.0);
                color = lerp(color, _Lightest, lightest);

                // invert colors
                //color.rgb = 1 - color.rgb;

                return color;
            }
            ENDCG
        }
    }
}
