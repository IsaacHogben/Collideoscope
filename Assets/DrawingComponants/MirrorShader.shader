Shader "Unlit/MirrorShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MirrorMode ("Mirror Mode", Range(0,8)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            float _MirrorMode;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 center = float2(0.5, 0.5);

                if (_MirrorMode == 2)
                {
                    if (uv.x > 0.5) uv.x = 1.0 - uv.x; // Mirror right half
                }
                else if (_MirrorMode == 4)
                {
                    float2 quadrant = floor(uv * 2);
                    if (quadrant.x == 1) uv.x = 1.0 - uv.x; // Mirror X
                    if (quadrant.y == 1) uv.y = 1.0 - uv.y; // Mirror Y
                }
                if (_MirrorMode == 8 || _MirrorMode == 16)
                {
                    // Convert UV to polar coordinates
                    float2 offset = uv - center;
                    float angle = atan2(offset.y, offset.x);
                    float radius = length(offset);
                    
                    // Number of segments
                    float segmentCount = (_MirrorMode == 8) ? 8 : 16;
                    float segmentAngle = 3.14159 / (segmentCount / 2.0);
                    
                    // Determine segment index
                    int segmentIndex = int(floor((angle + 3.14159) / segmentAngle)) % int(segmentCount);
                    bool flip = (segmentIndex % 2) == 1;
                    
                    // Mirror within the segment
                    float mirroredAngle = fmod(abs(angle), segmentAngle);
                    if (angle < 0) mirroredAngle = segmentAngle - mirroredAngle;
                    if (flip) mirroredAngle = segmentAngle - mirroredAngle;
                    
                    // Convert back to Cartesian coordinates
                    uv.x = center.x + cos(mirroredAngle) * radius;
                    uv.y = center.y + sin(mirroredAngle) * radius;
                }

                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
