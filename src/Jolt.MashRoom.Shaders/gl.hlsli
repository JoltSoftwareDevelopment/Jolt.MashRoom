/****************************************************************************************************
 * code by papademos
 * 
 * License: WTFPL (Do What the Fuck You Want to Public License)
 * http://www.wtfpl.net/
 *
 * This software is distributed without any warranty.
 ****************************************************************************************************/
#define iGlobalTime Custom[1][0]
#define iResolution float2(Custom[2][0], Custom[3][0])
#define iMouse float2(0, 0)
#define vec2 float2
#define vec3 float3
#define vec4 float4
#define mat2 float2x2
#define mat3 float3x3
#define mat4 float4x4
#define mix lerp
#define fract frac
#define atan(a,b) atan2((a),(b))
#define TIME (iGlobalTime)

// The GLSL mod will always have the same sign as y rather than x. 
// Otherwise its the same -- a value f such that x = i*y + f where i is an integer and |f| < |y|.
// For comparison, the HLSL fmod is equivalent to x - y * trunc(x/y).
// They're the same when x/y is positive, different when x/y is negative.
#define mod(a,b) (sign(b) * (abs(a) % abs(b)))

#define texture2D(t, uv) ((t).Sample(Sampler0, (uv)))
#define iChannel0 Texture0
#define iChannel1 Texture1
#define iChannel2 Texture2
#define iChannel3 Texture3
