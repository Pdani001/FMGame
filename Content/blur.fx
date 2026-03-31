#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float2 texelSize;
float strength;

float4 MainPS(VertexShaderOutput input) : COLOR
{
    return tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
}

// Source: https://github.com/MonoGame/MonoGame.Samples/blob/3.8.2/ShipGame/ShipGame.Core/Content/shaders/Blur.fx
#define BLUR_RANGE 5
float4 BlurHorizontalPS(VertexShaderOutput input) : COLOR
{
    float4 color = float4(0, 0, 0, 0);
    for (float i = -BLUR_RANGE; i <= BLUR_RANGE; i++)
    {
        float2 tc = input.TextureCoordinates + float2(i * strength * texelSize.x, 0);
        
        float4 c = tex2D(SpriteTextureSampler, tc) * input.Color;
        
        //c.xyz *= c.w;
        
        color += c;
    }
    return color / (2 * BLUR_RANGE + 1);
}

technique NoBlur
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

technique BlurHorizontal
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL BlurHorizontalPS();
    }
};