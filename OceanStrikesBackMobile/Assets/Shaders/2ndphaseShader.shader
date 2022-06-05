Shader "Hidden/2ndphaseShader"
{
    Properties
    {
        _SecondTex("Texture", 2D) = "white" {}
        _MainTex("Texture", 2D) = "white" {}
        _Amount("Slider", Range(0,1)) = 1
        SecondPhaseValue("Float", Float) = 1
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

                sampler2D _MainTex;
                sampler2D _SecondTex;
                float4 _SecondTex_ST;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _SecondTex);
                    //o.uv = v.uv;
                    return o;
                }

                half _Amount;
                float SecondPhaseValue;

                fixed4 frag(v2f i) : SV_Target
                {

                    //we set the value in a script, to be 2, when we reach the 2nd phase
                    if (SecondPhaseValue == 2) {

                        _Amount += (sin(_Time * 20));
                    }
                    else {
                        _Amount = 0;
                    }

                    fixed4 col = tex2D(_MainTex, i.uv);

                    if (_Amount > 1 - i.uv.x) {
                        //save the alpha of the texture in a variable
                        float b = tex2D(_SecondTex, i.uv).a;
                        //if the alpha is > 0, we draw the texture
                        if (b > 0) {
                            col = tex2D(_SecondTex, i.uv);
                        }
                    }
                    return col;
                }
                ENDCG
            }
        }
}
