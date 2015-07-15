/****************************************************************************************************
 * code by papademos
 * 
 * License: WTFPL (Do What the Fuck You Want to Public License)
 * http://www.wtfpl.net/
 *
 * This software is distributed without any warranty.
 ****************************************************************************************************/
float4x4    World;
float4x4    View;
float4x4    Projection;
float4x4    WorldView;
float4x4    WorldViewProjection;
float4x4    ViewProjection;
float4x4    WorldIT;
float4x4    WorldViewIT;
float4x4    LightPosition;
float4x4    Custom;
float4x4    CustomCredits;

//
Texture2D Texture0      : register (t0);
Texture2D Texture1      : register (t1);
Texture2D Texture2      : register (t2);
Texture2D Texture3      : register (t3);
Texture2D Texture4      : register (t4);
Texture2D Texture5      : register (t5);
Texture2D Texture6      : register (t6);
Texture2D Texture7      : register (t7);
SamplerState Sampler0   : register (s0);

//
#define LerpTime (Custom[0][0])
#define Time (Custom[1][0])
#define Resolution float2(Custom[2][0], Custom[3][0])
#define Aspect (Custom[2][0] / Custom[3][0])

//
#define BeatTime (Custom[0][1])
#define Lead (Custom[1][1])

//
#define Nisse0 (Custom[0][2])
#define Nisse1 (Custom[1][2])
#define Nisse2 (Custom[2][2])
#define Nisse3 (Custom[3][2])

//
#define texHeight (CustomCredits[0][0])
#define outroTime (CustomCredits[1][0])
#define outroFade (CustomCredits[2][0])
