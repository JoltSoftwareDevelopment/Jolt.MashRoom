/****************************************************************************************************
 * code by savestate
 * 
 * License: WTFPL (Do What the Fuck You Want to Public License)
 * http://www.wtfpl.net/
 *
 * This software is distributed without any warranty.
 ****************************************************************************************************/
row_major float4x4    World;
row_major float4x4    View;
row_major float4x4    Projection;
row_major float4x4    WorldView;
row_major float4x4    WorldViewProjection;
row_major float4x4    ViewProjection;
row_major float4x4    WorldIT;
row_major float4x4    WorldViewIT;
row_major float4x4    LightPosition;
float4x4    Custom;
row_major float4x4 CustomCredits;
#include "vanilla.pixel.hlsli"
#define radScale CustomCredits[0][3]


/****************************************************************************************************
 *
 ****************************************************************************************************/
struct Vertex
{
	float4 Position	    : POSITION0;
	float2 TexCoord0    : TEXCOORD0;
};


Pixel main(Vertex vertex)
{
	float3 vert = vertex.Position;
    vert.x = vert.x * radScale;
	vert.z = vert.z * radScale;
	float3 worldPosition = mul(vert, World).xyz;
	float u = vertex.TexCoord0.x;
	float v = vertex.TexCoord0.y;

	Pixel pixel;
	pixel.Position = mul(ViewProjection, float4(worldPosition, 1));
	pixel.TexCoord0 = float2(u, v);
	pixel.Color = float4(1,1,1,1);
	return pixel;
}
