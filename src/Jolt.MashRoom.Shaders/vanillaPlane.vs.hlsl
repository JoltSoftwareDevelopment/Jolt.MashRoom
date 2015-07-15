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
	float4 p = vertex.Position;
	float3 worldPosition = mul(vertex.Position, World).xyz;
	float3 worldNormal = normalize(mul(vertex.Normal, WorldIT).xyz);

	Pixel pixel;
	//pixel.Position = mul(float4(worldPosition, 1), ViewProjection);
    pixel.Position = float4(p.x, -p.z, 0, 1);
	pixel.Color = float4(0, 0, 0, 0);
    pixel.TexCoord0 = vertex.TexCoord0.xy;
	
	return pixel;
}
