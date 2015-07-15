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
Pixel main(Vertex vertex)
{
	float4 worldPosition = mul(vertex.Position, World);

	float u = vertex.TexCoord0.x;
	float v = vertex.TexCoord0.y;

	Pixel pixel;
	pixel.Position = mul(worldPosition, ViewProjection);
	pixel.TexCoord0 = float2(u, v);
	pixel.Color = vertex.Color;
	pixel.Color = float4(2.5*pixel.TexCoord0, 0, 0);
	pixel.Color.r = max(pixel.Color.r, pixel.Color.g);
	return pixel;
}
