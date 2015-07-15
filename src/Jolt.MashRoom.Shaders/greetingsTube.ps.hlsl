/****************************************************************************************************
 * code by savestate
 * 
 * License: WTFPL (Do What the Fuck You Want to Public License)
 * http://www.wtfpl.net/
 *
 * This software is distributed without any warranty.
 ****************************************************************************************************/
#include "environment.hlsli"
#include "vanilla.pixel.hlsli"
#include "gl.hlsli"


float4 main(Pixel pixel) : SV_Target
{
	float2 uv = pixel.TexCoord0;
	float tmpRatio = 1080.0 / texHeight;
	float offset = (texHeight * outroTime) / texHeight;
	float y = -tmpRatio + (((1 - uv.y) * tmpRatio) + offset) + (tmpRatio * outroTime);
	float a = texture2D(iChannel0, float2(uv.x, y)).r;
	float r = a * outroFade;
	return float4(r,r,r,r);
}

