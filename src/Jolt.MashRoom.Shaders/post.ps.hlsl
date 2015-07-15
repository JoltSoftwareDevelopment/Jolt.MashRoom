/****************************************************************************************************
 * code by papademos
 * 
 * License: WTFPL (Do What the Fuck You Want to Public License)
 * http://www.wtfpl.net/
 *
 * This software is distributed without any warranty.
 ****************************************************************************************************/
#include "environment.hlsli"
#include "vanilla.pixel.hlsli"


/****************************************************************************************************
 * 
 ****************************************************************************************************/
// TODO rename texture
//Texture2D Texture0  : register (t0);
SamplerState SamplerNisse
{
	//Texture		= Texture0;
    // TODO make sure we have the right filtertype
	Filter		= ANISOTROPIC; // http://msdn.microsoft.com/en-us/library/windows/desktop/ff476132(v=vs.85).aspx
    //Filter		= MIN_MAG_MIP_LINEAR;
	AddressU	= Wrap;
	AddressV	= Wrap;
};


// TODO rename texture
//Texture1D<float4> Texture1  : register (t1);
SamplerState Sampler1
{
    Filter		= MIN_MAG_MIP_LINEAR;
	AddressU	= Wrap;
	AddressV	= Wrap;
};


// TODO rename texture
//Texture2D Texture2  : register (t2);
SamplerState Sampler2
{
    Filter		= ANISOTROPIC;//MIN_MAG_MIP_LINEAR;
	AddressU	= Wrap;
	AddressV	= Wrap;
};


// TODO rename texture
//Texture2D Texture3  : register (t3);
SamplerState Sampler3
{
    Filter		= ANISOTROPIC;//MIN_MAG_MIP_LINEAR;
	AddressU	= Wrap;
	AddressV	= Wrap;
};


/****************************************************************************************************
 * 
 ****************************************************************************************************/
float4 main(Pixel pixel) : SV_Target
{
    float x = pixel.TexCoord0.x;
    float y = pixel.TexCoord0.y;
    
    // offsets for noisy color separation
    //float4 offset = 0.01.xxxx * (Texture1.SampleLevel(Sampler1, 0.1*y, 0) - 0.5.xxxx);
    float4 offset = 0;//0.1.xxxx * (Texture1.SampleLevel(Sampler1, 0.25*y, 0) - 0.5.xxxx);
    //offset.r
    offset.g = offset.r;
    offset.b = offset.r;
    offset.a = 0;

    //float4 offset = 0.01 * (Texture1.SampleLevel(Sampler1, y, 0) - 0.5.xxxx);
    //offset.r -= 0.01;
    //offset.g += 0;
    //offset.b += 0.01;
    //offset.a = 0;
//
    // noisy color separation
    float4 color;
    color.r = Texture0.SampleLevel(Sampler0, float2(x + offset.r, y), 0).r;
    color.g = Texture0.SampleLevel(Sampler0, float2(x + offset.g, y), 0).g;
    color.b = Texture0.SampleLevel(Sampler0, float2(x + offset.b, y), 0).b;
    color.a = 0;
    return color;

    // ghost
    float4 ghost;
    ghost.r = Texture0.SampleLevel(Sampler0, float2(x + offset.r, y) - float2(0.1, 0.01), 0).r;
    ghost.g = Texture0.SampleLevel(Sampler0, float2(x + offset.g, y) - float2(0.1, 0.01), 0).g;
    ghost.b = Texture0.SampleLevel(Sampler0, float2(x + offset.b, y) - float2(0.1, 0.01), 0).b;
    ghost.a = 0;
    color = 0.9 * color + 
            0.1 * ghost;

    // scanlines
    int count = 50;
    float distanceToCenter = abs(2 * frac(count * y) - 1);
    float scanlineWeight = saturate(1.2 * distanceToCenter);

    // static noise
    float noiseWeight = Texture2.SampleLevel(Sampler2, float2(x, y), 0).r;
    
    // circular fade
    float circularDistance = saturate(1.4 * distance(float2(x, y), float2(0.5, 0.5)));
    float circularWeight = 0.5 + 0.5 * cos(3.1415 * circularDistance);

    // weights
    color *= 
        0.2 +
        0.8 * pixel.Color +
        0.1 * noiseWeight + 
        0.1 * scanlineWeight;
    color *= circularWeight;
	
    // bright black
    float gray = 32 / 256.0;
    color *= 1 - gray;
    color += gray.xxxx;

    //
    color *= pixel.Color / 4;

    //
    color += Texture3.SampleLevel(Sampler3, pixel.TexCoord0.xy, 0).xxxx;

    //
    return color;
}
