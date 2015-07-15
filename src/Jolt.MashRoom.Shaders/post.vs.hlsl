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
	float3 worldPosition = mul(vertex.Position, World).xyz;
    //float3 worldPosition = vertex.Position.xzy * float3(1,-1,1);

	//float x = vertex.Position.x;
	//float y = -vertex.Position.z;
    
    //int id = vertex.VertexID;
	//float x;
	//float y;
         //if (id == 0) { x = -1; y =  1; }
    //else if (id == 1) { x =  1; y =  1; }
    //else if (id == 2) { x =  1; y = -1; }
    //else if (id == 4) { x =  1; y = -1; }
    //else if (id == 5) { x = -1; y = -1; }
    //else if (id == 3) { x = -1; y =  1; }
	//float u = 0.5 * (x + 1);
	//float v = 0.5 * (1 - y);
	float u = vertex.TexCoord0.x;
	float v = vertex.TexCoord0.y;
	//float v = 0.5 * (1 - y);


	Pixel pixel;
	//pixel.Position = float4(x, y, 0, 1);
	pixel.Position = mul(float4(worldPosition, 1), ViewProjection);
	pixel.TexCoord0 = float2(u, v);
    pixel.Color = vertex.Color;
    //pixel.Color = float4(2.5*pixel.TexCoord0, 0, 0);
    //pixel.Color.r = max(pixel.Color.r, pixel.Color.g);
	return pixel;
}
