Shader "Unlit/Outline"
{
    Properties
    {
        _Color("Main Color", Color) = (0.5,200.5,0.5,1)
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor("Outline color", Color) = (255,0,255,0.4)
        _OutlineWidth("Outline width", Range(1.0, 5.0)) = 50000000
    }

    CGINCLUDE 
    #include "UnityCG.cginc"

    struct appdata 
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
    };

    struct v2f 
    {
        float4 pos : POSITION;
        float3 normal : NORMAL;
    };

    float _OutlineWidth;
    float _OutlineColor;

    v2f vert(appdata v){
        v.vertex.xyz *= _OutlineWidth;
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        return o;
    }
    ENDCG

    SubShader
    {
        Tags{"Queue" = "Transparent"} // Render the shader on top. 
        Pass // Render the Outline
        {
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            half4 frag(v2f i) : COLOR
            {
                return _OutlineColor;
            }
            ENDCG
        }

        Pass // Normal render
        {
            ZWrite On

            Material 
            {
                Diffuse[_Color]
                Ambient[_Color]
            }

            Lighting On

            SetTexture[_MainTex]
            {
                ConstantColor[_Color]
            }

            SetTexture[_MainTex]{
                Combine previous * primary DOUBLE
            }
        }
    }
}
