Shader "Unlit/ReadBarCode"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_FontTex ("FontTex", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
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

			float mod(float a, float b)
			{
				return a - floor((a / b)) * b;
			}

			sampler2D _MainTex;
			sampler2D _FontTex;
			float4 _MainTex_ST;
		


			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			const float cols = 22;

				fixed4 col = tex2D(_FontTex, i.uv);

				float Display_word_w = 1./cols;
				float Display_word_h = 1./1.;

				int Display_col = (int)((i.uv.x)*cols);//整数n列目



				const float width = 1024;
				float LengthLog[width];
				int bit = 0;
				float last = 0;
				float last_pos = 0;
				for(int now_pos = 0;now_pos < width;now_pos++)
				{
					float3 clr = tex2D(_MainTex, float4((float)now_pos/width,0.5, 0, 0));
					float now = step((clr.r+clr.g+clr.b)/3,0.2);
					if(now != last)
					{
						LengthLog[bit] = now_pos - last_pos;
						bit++;
						last_pos = now_pos;
					}
					last = now;
				}

				float OneLen = LengthLog[1]/2;
				int ByteLog[cols];

				for(int k = 0;k < cols;k++)
				{
					float Bit_7 = step(1.5,LengthLog[k*8+0+2]/OneLen);
					float Bit_6 = step(1.5,LengthLog[k*8+1+2]/OneLen);
					float Bit_5 = step(1.5,LengthLog[k*8+2+2]/OneLen);
					float Bit_4 = step(1.5,LengthLog[k*8+3+2]/OneLen);
					float Bit_3 = step(1.5,LengthLog[k*8+4+2]/OneLen);
					float Bit_2 = step(1.5,LengthLog[k*8+5+2]/OneLen);
					float Bit_1 = step(1.5,LengthLog[k*8+6+2]/OneLen);
					float Bit_0 = step(1.5,LengthLog[k*8+7+2]/OneLen);

					float over = step(k,bit/8);

					ByteLog[k] = 
					Bit_7*128+
					Bit_6*64+
					Bit_5*32+
					Bit_4*16+
					Bit_3*8+
					Bit_2*4+
					Bit_1*2+ 
					Bit_0*1;
					ByteLog[k] *= over;
				}

				float BBLog[cols];
				for(int k = 0;k < cols;k++)
				{
					BBLog[k] = (int)round(LengthLog[k+2]/OneLen);
				}

			

				const float Font_word_w = 1./16.;
				const float Font_word_h = 1./16.;

				float word_x = mod(i.uv.x,Display_word_w) / Display_word_w *  Font_word_w;
				float word_y = mod(i.uv.y,Display_word_h) / Display_word_h *  Font_word_h;

				int ByteData = ByteLog[Display_col];
				//ByteData = step(1.5,LengthLog[0*8+Display_col+2]/OneLen)+0x30;
				//ByteData = BBLog[Display_col] + 0x30;

				//ByteData = 0x30 + Display_col;

				//ByteData = Display_col*16 + Display_row;


				float FontCordX = Font_word_w*(ByteData/16) + word_x;
				float FontCordY = Font_word_h*(15-ByteData%16) + word_y;

				col = tex2D(_FontTex, float2(FontCordX,FontCordY));

				return col;
			}
			ENDCG
		}
	}
}
