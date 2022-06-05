Shader "Unlit/greenBordersShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        //_SecondTex("Texture", 2D) = "white" {}
        _BoardTex("Texture", 2D) = "white" {}
    }
        SubShader
    {

        Tags { "RenderType" = "Opaque" }
        LOD 100

        //desenha a imagem espandida em 1º
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite off
            Cull back

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

            sampler2D _BoardTex;
            float4 _BoardTex_ST;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                v.vertex.xz *= 1;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                fixed4 seTivesseImagem = tex2D(_MainTex, i.uv);
                fixed4 col = fixed4(0,0,0,0);

                col = tex2D(_BoardTex, i.uv);

                //usamos a imagem da projeção para saber a forma usando o alpha da imagem, neste momento ta com textura, mas pode estar com cor
                if (seTivesseImagem.a > 0.3)
                {
                    col = tex2D(_BoardTex, i.uv);
                    //col = fixed4(1,20,1,1);
                }
                else
                {
                    discard;
                }

                return col;
            }
            ENDCG
        }
    }
}
