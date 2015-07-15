/****************************************************************************************************
 * original code by ryk
 * https://www.shadertoy.com/view/XtXXRH
 * 
 * License (shadertoy default): Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported
 * http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US
 *
 * hlsl conversion, mashing and modification by papademos
 ****************************************************************************************************/
#include "environment.hlsli"
#include "vanilla.pixel.hlsli"
#include "common.hlsli"
//#include "gl.hlsli"
#define mod(a,b) (sign(b) * (abs(a) % abs(b)))


/****************************************************************************************************
 * 
 ****************************************************************************************************/
float4 metaMonolith(float2 xy);
float4 main(Pixel pixel) : SV_Target
{
    return metaMonolith(pixel.TexCoord0 * Resolution); 
}


/****************************************************************************************************
 *
 ****************************************************************************************************/
static float time;
static float3x3 rotation;
#define PI 3.141592653589


struct Ray
{
	float3 org;
	float3 dir;
};


Ray new_Ray(float3 org, float3 dir)
{
    Ray ray;
    ray.org = org;
    ray.dir = dir;
    return ray;
}


float3x3 rotateX(float a)
{
    return float3x3(1.,0.,0.,
                0.,cos(a), -sin(a),
                0.,sin(a), cos(a));
}


float3x3 rotateY(float a)
{
    return float3x3(cos(a), 0., -sin(a),
                0.,1.,0.,
                sin(a), 0., cos(a));
}


float hash(float f)
{
    return frac(sin(f*32.34182) * 43758.5453);
}


float hash(float2 p)
{
    return frac(sin(dot(p.xy, float2(12.9898,78.233))) * 43758.5453);
}


float3 grid(float3 dir, bool vert)
{
    float2 p = dir.xy / max(0.001, abs(dir.z));
    p *= 3;
    p.y *= 0.06;
    p.y += 2 * time;
    vert = hash(floor(p.y / 5 + 0.5)) < 0.5 ? vert : !vert;
    p += 0.5;
    float h  = hash(floor(p * sign(dir.z)));
    float h2 = hash(floor(p.y / 6));
    float h3 = hash(floor(p.y / 20) + sign(dir.z));
    float band = (abs(p.x) < 2 + floor(30 * h3 * h3)) ? 1 : 0;
    p.x = mod(p.x, 1);
    p.y = mod(p.y, 1);
    p -= 0.5;
    float f = (h2 < 0.5) ? 6 * smoothstep(0.6, 0, length(p)) : 2;
    h = (h < h2 / 1.2 + 0.1 && vert) ? 1 : 0;

    float3 hsl = float3(0.6, 0.9, h2 / 2);
    float3 rgb = hsl2rgb(hsl) * h * band * 3 * f;
    rgb *= pow(abs(dir.z), 0.5);
    return rgb;
}


float3 background(float3 dir)
{
    return grid(dir.zxy, true) + grid(dir.yxz, false);
}


float box(float3 p, float3 w){
    p = abs(p);
    return max(p.x-w.x, max(p.y-w.y, p.z-w.z));
}


float map(float3 p)
{
    for (int i = 0; i < 3; i++){
        p = abs(mul(rotation, p) + float3(0.1, .0, .0));
        p.x -= (sin(time/8.) + 1.)/2.;
        p.y -= (sin(time/7.) + 1.)/3.;
        p.z -= (sin(time/3.) + 1.)/4.;
    }
    return box(p, float3(0.8, 4.4, 0.4));
}


float3 normal(float3 pos)
{
	float3 eps = float3( 0.001, 0.0, 0.0 );
	float3 nor = float3(map(pos+eps.xyy) - map(pos-eps.xyy),
                    map(pos+eps.yxy) - map(pos-eps.yxy),
                    map(pos+eps.yyx) - map(pos-eps.yyx) );
	return normalize(nor);
}


float3 selfReflect(Ray ray)
{
    float dist = 0.01;
    float3 pos;
    float minDist = 1000.;
    float curMap;
    for (int i = 0; i < 30; i++){
        pos = ray.org + dist*ray.dir;
        curMap = map(pos);
        dist+=curMap;
        if(i > 7){
            minDist = min(minDist,curMap);
        }
    }
    float m = map(pos);
    if (m < 0.01){
        float3 n = normal(pos);
        float3 r = reflect(ray.dir, n);
        float3 refl = background(r);
        float rf = 0.8-abs(dot(ray.dir, n))*.4;
        rf *= rf;
        return refl*rf*1.3; 
    }
    float glow = 0.02/minDist;

    return background(ray.dir)*0.5 + glow * float3(1.9, 2.4, 3.2);
}


float3 render(Ray ray)
{
    float dist = 0.;
    float3 pos;
    float minDist = 1000.;
    float curMap;
    for (int i = 0; i < 40; i++){
        pos = ray.org + dist*ray.dir;
        curMap = map(pos);
        dist+=curMap;
        minDist = min(minDist,curMap);
    }
    float m = map(pos);
    if (m < 0.01){
        float3 n = normal(pos);
        float3 r = reflect(ray.dir, n);
        float3 refl = selfReflect(new_Ray(pos, r));
        float rf = 0.8-abs(dot(ray.dir, n))*.4;
        rf *= rf;
        return r.yyy + refl*rf*1.3; 
    }
    float glow = 0.02/minDist;

    return background(ray.dir)*0.5 + glow * float3(1.9, 2.4, 3.2);
}


//Ray createRay(Camera camera, float2 uv, float fov, float aspect)
//{
//	Ray ray;
//	ray.org = camera.Position;
//	float3 dir = Camera_GetDirection(camera);
//	up = normalize(up - dir * dot(dir,up));
//	float3 right = cross(dir, up);
//	uv = 2.*uv - float2(1, 1);
//	fov = fov * 3.1415/180.;
//	ray.dir = dir + tan(fov/2.) * right * uv.x + tan(fov/2.) / aspect * up * uv.y;
//	ray.dir = normalize(ray.dir);	
//	return ray;
//}
Ray createRay(Camera camera, float2 uv, float fov, float aspect)
{
	Ray ray;
	ray.org = camera.Position;
	float3 dir = Camera_GetDirection(camera);
	float3 up = normalize(camera.Up - dir * dot(dir, camera.Up));
	float3 right = cross(dir, up);
	uv = 2 * uv - float2(1, 1);
	fov *= 3.1415 / 180;
	ray.dir = dir + 
        tan(fov / 2) * right * uv.x + 
        tan(fov / 2) / aspect * up * uv.y;
	ray.dir = normalize(ray.dir);	
	return ray;
}


/****************************************************************************************************
 * 
 ****************************************************************************************************/
float4 metaMonolith(float2 xy)
{
    // set up global variables
    time = 2 * BeatTime;
    float xAngle = (floor(time / 8) + saturate(20 * frac(time / 8))) * PI / 2;
    float yAngle = (floor(time / 2) + saturate( 5 * frac(time / 2))) * PI / 4;
    rotation = mul(
        rotateY(yAngle),
        rotateX(xAngle));
        //CreateRotationMatrixY(yAngle),
        //CreateRotationMatrixX(xAngle));


    // set up camera
    Camera camera;
    camera.Position.x = -8;
    camera.Position.y =  2 * sin(time / 10);
    camera.Position.z = -4 * sin(time / 4);
	camera.LookAt = float3(0, 0, 0);
	camera.Up = float3(0, 0, 1);

    // render
    float2 p = xy / Resolution;
	Ray ray = createRay(camera, p, 90, Aspect);
    float3 col = render(ray);

    //
    return float4(saturate(col), 1);
}

