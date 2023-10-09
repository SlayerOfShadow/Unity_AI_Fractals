Shader "Custom/GridGlowShader"
{
    Properties
    {
        _MainColor("Main Color", Color) = (1, 1, 1, 1)
        _GlowColor("Glow Color", Color) = (0, 0, 1, 1) // Blue
        _CenterColor("Center Color", Color) = (1, 0, 0, 1) // Red
        _GlowIntensity("Glow Intensity", Range(0, 1)) = 0.5
        _CenterRadius("Center Radius", Range(0, 1)) = 0.2
        _CenterPosition("Center Position", Vector) = (0, 0, 0, 0) // Default center position
        _GlowSpeed("Glow Speed", Range(0, 10)) = 1.0 // Adjust the speed of the glow movement
        _GlowAmplitude("Glow Amplitude", Range(0, 100)) = 10. // Adjust the amplitude of the glow movement
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 pos : TEXCOORD0;
            };

            float4 _MainColor;
            float4 _GlowColor;
            float4 _CenterColor;
            float _GlowIntensity;
            float _CenterRadius;
            float4 _CenterPosition;
            float _GlowSpeed;
            float _GlowAmplitude;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.pos = v.vertex;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Calculate the vertical offset based on the cosinus of time
                float yOffset = cos(_Time.y * _GlowSpeed) * _GlowAmplitude;

            // Apply the vertical offset to the position
            i.pos.y += yOffset;

            // Calculate the distance from the current fragment to the center position
            float distanceToCenter = length(i.pos - _CenterPosition.xyz);

            // Calculate the glow factor based on the distance (closer to center -> more glow)
            float glowFactor = saturate(1.0 - distanceToCenter * _GlowIntensity);


            // Calculate the color based on the distance (closer to center -> more blue, farther -> more red)
            float4 color = lerp(_GlowColor, _CenterColor, distanceToCenter * _CenterRadius);

            // Apply the glow effect
            color += _MainColor * glowFactor;

            return color;
        }
        ENDCG
    }
    }
}
