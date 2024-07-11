Shader "Custom/OverlayShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormalTex ("Normal Map", 2D) = "bump" {}
        _Color ("Visible Tint", Color) = (1,1,1,1)
        _ObstructedColor ("Obstructed Color", Color) = (0.5,0.5,0.5,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Overlay" }
        LOD 200
        ZTest Always

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalTex;
        sampler2D _CameraDepthTexture;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalTex;
            float4 screenPos;
        };

        fixed4 _Color;
        fixed4 _ObstructedColor;
        half _Glossiness;
        half _Metallic;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float screenDepth = IN.screenPos.z / IN.screenPos.w;
            float currentDepth = UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)));
            bool isVisible = currentDepth < screenDepth;

            fixed4 color = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = isVisible ? color.rgb : _ObstructedColor;
            if (isVisible) o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_NormalTex));
            o.Alpha = color.a;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}