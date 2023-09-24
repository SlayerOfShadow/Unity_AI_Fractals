Shader "Unlit/RaymarchMandelbulb"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Power ("Power", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define MAX_STEPS 1000
            #define MAX_DIST 100
            #define SURF_DIST 1e-4
            #define MAX_ITERATIONS 20
            #define BAILOUT 10

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ro : TEXCOORD1;
                float3 hitPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Power;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
                o.hitPos = v.vertex;
                return o;
            }

            float GetDist(float3 p)
            {
                float3 z = p;
                float dr = 5.0;
                float r = 0.0;

                for (int i = 0; i < MAX_ITERATIONS; i++)
                {
                    r = length(z);
                    if (r > BAILOUT)
                        break;

                    float theta = acos(z.y / r);
                    float phi = atan2(z.z, z.x);
                    dr = pow(r, _Power - 1.0) * _Power * dr + 1.0;

                    float3 zr = pow(r, _Power);
                    theta = theta * _Power;
                    phi = phi * _Power;

                    z = zr * float3(sin(theta) * cos(phi), sin(theta) * sin(phi), cos(theta)) + p;
                }

                float dist = 0.5 * log(r) * r / dr;
                return dist;
            }

            float Raymarch(float3 ro, float3 rd)
            {
                float dO = 0;
                float dS;
                for (int i = 0; i < MAX_STEPS; i++)
                {
                    float3 p = ro + dO * rd;
                    dS = GetDist(p);
                    dO += dS;
                    if (dS < SURF_DIST || dO > MAX_DIST)
                    {
                        break;
                    }
                }

                return dO;
            }

            float3 GetNormal(float3 p)
            {
                float2 e = float2(1e-2, 0);
                float3 n = GetDist(p) - float3(
                    GetDist(p - e.xyy),
                    GetDist(p - e.yxy),
                    GetDist(p - e.yyx)
                );
                return normalize(n);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5f;
                float3 ro = i.ro;
                float3 rd = normalize(i.hitPos - ro);

                float d = Raymarch(ro, rd);
                fixed4 col = 0;

                if (d >= MAX_DIST || d < 0.0f)
                {
                    discard; // Discard pixels where there's no surface or the distance is negative.
                }
                else
                {
                    float3 p = ro + rd * d;
                    float3 n = GetNormal(p);

                    // Get the world space position of the first directional light.
                    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz - p);

                    // Calculate the dot product between the surface normal and light direction.
                    float diff = max(dot(n, lightDir), 0.0);

                    // Set a static color here based on your desired color scheme and apply lighting.
                    float3 baseColor = float3(1.0, 0.5, 0.2); // Base color (e.g., orange).
                    col.rgb = baseColor * diff; // Apply simple Lambertian reflection.

                    // Add a specular component for highlights.
                    float3 specularColor = float3(1.0, 1.0, 1.0); // White
                    float shininess = 32.0; // Adjust shininess for desired highlight size.
                    float3 reflectionDir = reflect(-lightDir, n);
                    float specular = pow(max(dot(rd, reflectionDir), 0.0), shininess);
                    col.rgb += specularColor * specular;

                    // Add ambient lighting to simulate global illumination.
                    float3 ambientColor = float3(0.1, 0.1, 0.1); // Low-intensity ambient color.
                    col.rgb += ambientColor;
                }

                return col;
            }
            ENDCG
        }
    }
}
