/****************************************************************************************************
 * original code by ryk
 * https://www.shadertoy.com/view/MtSGRt
 * 
 * License (shadertoy default): Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported
 * http://creatifloatommons.org/licenses/by-nc-sa/3.0/deed.en_US
 *
 * hlsl conversion, mashing and modification by papademos
 ****************************************************************************************************/
#include "environment.hlsli"
#include "vanilla.pixel.hlsli"
#include "common.hlsli"
#define SKIP_HEADER
    #include "ryk.frontierOfWires.ps.hlsl"
#undef SKIP_HEADER


/****************************************************************************************************
 * 
 ****************************************************************************************************/
float4 crystalBeacon(float2 xy);
float4 main(Pixel pixel) : SV_Target
{
    return crystalBeacon(pixel.TexCoord0 * Resolution); 
}


/****************************************************************************************************
 *
 ****************************************************************************************************/
static float SyncTime;
static float3x3 rotation;


/****************************************************************************************************
 *
 ****************************************************************************************************/


/****************************************************************************************************
 *
 ****************************************************************************************************/
float3 background(float3 dir, float2 xy)
{
    return wires(xy).rgb;
}


float4 box(float3 p, float w)
{
    p = abs(p);
    float dx = p.x - w;
    float dy = p.y - w;
    float dz = p.z - w;
    float m = max(p.x - w, max(p.y - w, p.z - w));
    return float4(m, dx, dy, dz);
}


float4 map(float3 p)
{
    for (int i = 0; i < 5; i++)
    {
        p = abs(mul(rotation, p) + float3(0.1, 0, 0));
        p.xy = p.yx -float2(0.8, 0.06);
    }
    return box(p, 0.6);
}


float3 normal(float3 pos)
{
	static const float3 eps = float3(0.001, 0, 0);
	return normalize(float3(
	    map(pos + eps.xyy).x - map(pos - eps.xyy).x,
	    map(pos + eps.yxy).x - map(pos - eps.yxy).x,
	    map(pos + eps.yyx).x - map(pos - eps.yyx).x));
}


float3 foreground(float3 dir, float2 xy, float3 pos, float4 m)
{
    float3 n = normal(pos);
    float3 l = normalize(float3(1, 2, 5));
    float3 diffuse = saturate(dot(n, l));
    float3 r = reflect(dir, n);
    float3 refl = background(r, xy);
    float dx = m.y;
    float dy = m.z;
    float dz = m.w;
    float start = 0.00;
    float end = 0.05;
    float f = 1 -
        smoothstep(start, end, abs(dx - dy)) * 
        smoothstep(start, end, abs(dx - dz)) *
        smoothstep(start, end, abs(dz - dy));
    float rf = pow(1 - abs(dot(dir, n)), 3);
    float flash = sqrt(1 - frac(SyncTime));
    float3 color = 
        diffuse * (1 - rf) * 0.8 + 
        flash * f * float3(2.9, 1.4, 1.2) + 
        refl * rf * 1.3;
    color.rgb = hsl2rgb(float3(0.6, 0.9, 0.7 * (color.g + color.b)));
    return color;
}


float4 render(Ray ray, float2 xy)
{
    float3 pos;
    float dist = 0;
    for (int i = 0; i < 60; i++)
    {
        pos = ray.org + dist*ray.dir;
        dist += map(pos).x;
    }

    float4 m = map(pos);
    if (m.x < 0.01)
        return float4(foreground(ray.dir, xy, pos, m), 1);
    else
        return float4(background(ray.dir, xy), 0);

}


/****************************************************************************************************
 * 
 ****************************************************************************************************/
float4 crystalBeacon(float2 xy)
{
    // set up global variables (used for effect synchronization)
    SyncTime = floor(BeatTime) + 1 - exp(-9 * frac(BeatTime));
    rotation = mul(
        CreateRotationMatrixX(1.9 * SyncTime), 
        CreateRotationMatrixY(1.4 * SyncTime));

    // set up camera
    Camera camera;
	camera.Position.x =  7 * sin(BeatTime / 3);
    camera.Position.y =  7 * cos(BeatTime / 3),
    camera.Position.z = -4 * sin(BeatTime / 8);
	camera.LookAt = float3(0, 0, 0);
	camera.Up = float3(0, 0, 1);

    // create ray
    float2 uv = xy / Resolution;
	Ray ray = createRay(camera, uv, 90, Aspect);

    // render
    return Nisse0 * render(ray, xy);
}