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

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 coordinates; // srotage for the new set of coordinates to repalce "v_vTexcoord"
    float pixelDistanceX; // storage for the distance of current read pixel fron horizontal center
    float pixelDistanceY; // storage for the distance of current read pixel fron vertical center
    float offset; // storage for the distance we'll displace the pixel on the screen.
    float dir; // direction in which we'll displace the pizels.
    float2 v_vTexcoord = input.TextureCoordinates;
    
    pixelDistanceX = distance(v_vTexcoord.x, 0.5); // calculate the current pixel distance from horizontal center 
    pixelDistanceY = distance(v_vTexcoord.y, 0.5); // calculate the current pixel distance from vertical center 
  
    offset = (pixelDistanceX * 0.7) * pixelDistanceY; // offset will be the Y distance fro, vertical center multiplied by the 0.2 fractiopn of pixelDistanceX
  // basically the further the pixel is from horizontal cetner and vertical center, the further the disnplacement will be
  
    if (v_vTexcoord.y <= 0.5)  
        dir = 1.0; // if the pixel is before the half of the screen (0.5) then dispalce the pixel upwards (1)
    else
        dir = -1.0; // else displace downwards (-1)
    
    // finally prepare the new texture "vector 2" (vec2)
    coordinates = float2(v_vTexcoord.x, v_vTexcoord.y + pixelDistanceX * (offset * 3.0 * dir));
	return tex2D(SpriteTextureSampler,coordinates) * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};