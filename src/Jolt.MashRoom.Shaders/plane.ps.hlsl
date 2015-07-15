/****************************************************************************************************
 * code by papademos
 * 
 * License: WTFPL (Do What the Fuck You Want to Public License)
 * http://www.wtfpl.net/
 *
 * This software is distributed without any warranty.
 ****************************************************************************************************/
#include "environment.hlsli"
#include "vanilla.vertex.hlsli"
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
	AddressU	= Clamp;
	AddressV	= Clamp;
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
    // todo: dont use .xxxx
    return Texture3.SampleLevel(Sampler3, pixel.TexCoord0.xy, 0).xxxx;
}
