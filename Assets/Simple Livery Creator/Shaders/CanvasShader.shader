Shader "CanvasShader"
{
    Properties
    {
        _MainTex ("First", 2D) = "white" {}
        [Enum(UV0, 0, UV1, 1, UV2, 2)] _FirstTexUVSet ("First UV Set", Float) = 0.0

        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Color ("Color", Color) = (1,1,1,1)
 
        _SecondTex ("Second", 2D) = "white" {}
        [Enum(UV0, 0, UV1, 1, UV2, 2)] _SecondTexUVSet ("Second UV Set", Float) = 0.0
 
        _ThirdTex ("Third", 2D) = "white" {}
        [Enum(UV0, 0, UV1, 1, UV2, 2)] _ThirdTexUVSet ("Third UV Set", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        CGPROGRAM
        #pragma surface surf Standard vertex:vert
 
        #pragma target 3.0
 
        sampler2D _MainTex, _SecondTex, _ThirdTex;
        float4 _MainTex_ST, _SecondTex_ST, _ThirdTex_ST;
        uint _FirstTexUVSet, _SecondTexUVSet, _ThirdTexUVSet;
 
        struct Input
        {
            float2 texcoord;
            float2 texcoord1;
            float2 texcoord2;
        };
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
 
        void vert(inout appdata_full v, out Input IN)
        {
            IN.texcoord = v.texcoord;
            IN.texcoord1 = v.texcoord1;
            IN.texcoord2 = v.texcoord2;
        }
 
        // function for more easily getting the UV set you want with multiple inline conditionals
        float2 SelectUVSet(uint set, Input IN)
        {
            return set == 0 ? IN.texcoord : set == 1 ? IN.texcoord1 : IN.texcoord2;
        }
 
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv_MainTex = TRANSFORM_TEX(SelectUVSet(_FirstTexUVSet, IN), _MainTex);
            fixed4 tex = tex2D (_MainTex, uv_MainTex) *_Color;
 
            float2 uv_SecondTex = TRANSFORM_TEX(SelectUVSet(_SecondTexUVSet, IN), _SecondTex);
            fixed4 tex2 = tex2D (_SecondTex, uv_SecondTex);
 
            float2 uv_ThirdTex = TRANSFORM_TEX(SelectUVSet(_ThirdTexUVSet, IN), _ThirdTex);
            fixed4 tex3 = tex2D (_ThirdTex, uv_ThirdTex);
 
            o.Albedo = tex.rgb * tex2.rgb * tex3.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = tex.a;

        }
        ENDCG
    }
    FallBack "Diffuse"
}