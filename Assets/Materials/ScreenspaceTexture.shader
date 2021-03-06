﻿Shader "Custom/ScreenspaceTexture" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf NoLighting
      struct Input {
          float2 uv_MainTex;
          float4 screenPos;
      };
      sampler2D _MainTex;
	  uniform float4 _MainTex_ST;
      void surf (Input IN, inout SurfaceOutput o) {
          float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
          screenUV *= _MainTex_ST.xy * float2(1, _ScreenParams.y / _ScreenParams.x);
          o.Albedo = tex2D (_MainTex, screenUV).rgb;
      }
	  fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
      {
		  fixed4 c;
          c.rgb = s.Albedo * 0.5f; 
          c.a = s.Alpha;
          return c;
       }
      ENDCG
    } 
    Fallback "Diffuse"
  }
