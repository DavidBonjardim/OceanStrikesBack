Shader "Custom/specialCardsShader"
{
    Properties
    {
        _MainTex("SereiaTexture", 2D) = "white" {}
        _SecondTex("PoluTexture", 2D) = "white" {}
        _Raio("raio name", Range(0, 1)) = 0.012
        _Speed("speed name", Range(1,25)) = 7.1
        _Wiggle("wiggle name", Range(1,25)) = 9.1
        _dissolveSlider("_dissolveSlider", Float) = 1.1
        oceToPolu("oceToPolu", Float) = 0
    }
        SubShader
        {
            Tags { "Queue" = "Overlay" }
            ZTest Always
            Cull Off 

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

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                sampler2D _MainTex;
                sampler2D _SecondTex;
                float _Raio;
                float _Speed;
                float _Wiggle;
                float _dissolveSlider;
                float oceToPolu;

                fixed4 frag(v2f i) : SV_Target
                {
                    //get the movement
                    float2 newUvs = _Raio * float2(sin(_Time.y * _Speed + i.uv.y * _Wiggle), cos(_Time.x * _Speed + i.uv.x * _Wiggle));

                    fixed4 col = (0, 0, 0, 0);

                    //1 is for oceanya
                    if (oceToPolu == 1) {
                        col = tex2D(_MainTex, i.uv + newUvs);

                    }
                    //if its poluitrum
                    else if (oceToPolu == 2) {
                        col = tex2D(_SecondTex, i.uv + newUvs);

                    }
                    else {
                        col = (0, 0, 0, 0);
                        //put the dissolve slider at 1.1 so texture is "invisible", needs to be 1.1 in this text because if its 1 we can see a bit of the texture still
                        _dissolveSlider = 1.1;                        
                    }

                    clip((col.rgb - _dissolveSlider));

                    return col;
                }
                ENDCG
            }
        }
    FallBack "Diffuse"
}
