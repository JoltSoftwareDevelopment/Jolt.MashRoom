/****************************************************************************************************
 * code by papademos
 * 
 * License: WTFPL (Do What the Fuck You Want to Public License)
 * http://www.wtfpl.net/
 *
 * This software is distributed without any warranty.
 ****************************************************************************************************/
struct Pixel
{
	float4 Position     : SV_POSITION;
	float4 Color	    : COLOR0;
    float2 TexCoord0    : TEXCOORD0;
};

