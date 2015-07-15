/****************************************************************************************************
 * original code by ryk
 * https://www.shadertoy.com/view/4sSGDG
 * 
 * License (shadertoy default): Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported
 * http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US
 *
 * hlsl conversion, mashing and modification by papademos
 ****************************************************************************************************/
#ifndef SKIP_HEADER
    #include "environment.hlsli"
    #include "vanilla.pixel.hlsli"
    #include "common.hlsli"


    /****************************************************************************************************
     *
     ****************************************************************************************************/
    float4 wires(float2 xy);
    float4 main(Pixel pixel) : SV_Target
    {
        return wires(pixel.TexCoord0 * Resolution); 
    }
#endif


/****************************************************************************************************
 *
 ****************************************************************************************************/
struct Ray
{
	float3 org;
	float3 dir;
};


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
struct Hit
{
	float dist;
	int   index;
};


Hit new_Hit(float dist, float index)
{
	Hit hit;
	hit.dist = dist;
	hit.index = index;
	return hit;
}


/****************************************************************************************************
 *
 ****************************************************************************************************/
float onOff(float a, float b, float c, float time)
{
	return clamp(c*sin(time + a*cos(time*b)), 0., 1.);
}


float glows(float index, float time)
{
	return onOff(5. + index*0.5, index + 3., 3., time);
}


float box(float3 pos, float3 dims)
{
	pos = abs(pos) - dims;
	return max(max(pos.x, pos.y), pos.z);
}


Hit hitUnion(Hit h1, Hit h2)
{
	if (h1.dist < h2.dist)
		return h1;
	else
		return h2;
}


Hit hitSub(Hit h1, Hit h2)
{
	if (h1.dist > -h2.dist)
		return new_Hit(h1.dist, h1.index);
	else
		return new_Hit(-h2.dist, h2.index);
}


Hit scene(float3 pos, float time)
{
	Hit totalHit;
	totalHit.dist = 9000;

    for (int i = 0; i < 5; i++)
    {
        //
        float angle = time + 0.5 * i;
        float3 p = pos;
        p.xz = mul(CreateRotationMatrix2D(angle), p.xz);

        //
        angle *= 1.3 * i;
        p.x += 4;
        p.xz = mul(CreateRotationMatrix2D(angle), p.xz);

        //
        Hit h = new_Hit(box(p, float3(0.4, 20, 0.2)), i);
        totalHit = hitUnion(h, totalHit);
    }
	return totalHit;
}


Hit raymarch(Ray ray, inout float glowAmt, float time)
{
	float3 pos;
	Hit hit;
	hit.dist = 0.;
	Hit curHit;
	for (int i = 0; i < 40; i++)
	{
		pos = ray.org + hit.dist * ray.dir;
		curHit = scene(pos, time);
		hit.dist += curHit.dist;
		glowAmt += clamp(pow(curHit.dist + 0.1, -8.), 0.0, 0.15) * glows(curHit.index, time);
	}
	hit.index = curHit.index;
	hit.index = curHit.dist < 0.01 ? hit.index : -1.;
	return hit;
}


float3 calcNormal(float3 pos, float time)
{
	float3 eps = float3(0.001, 0.0, 0.0);
	float3 nor = float3(
		scene(pos + eps.xyy, time).dist - scene(pos - eps.xyy, time).dist,
		scene(pos + eps.yxy, time).dist - scene(pos - eps.yxy, time).dist,
		scene(pos + eps.yyx, time).dist - scene(pos - eps.yyx, time).dist);
	return normalize(nor);
}


float3 render(Ray ray, float time)
{
    float glowAmt = 0;
	Hit hit = raymarch(ray, glowAmt, time);
	float3 pos = ray.org + hit.dist * ray.dir;
	float3 col = float3(0, 0, 0);
	if (hit.index != -1)
	{
		float3 nor = calcNormal(pos, time);
		float3 l = normalize(float3(3, 0, 0) - pos);
		col = float3(0.3, 0.5, 0.7);

		float diff = saturate(dot(nor, l));
		float3 r = normalize(2 * dot(nor, l) * nor - l);
		float3 v = normalize(ray.org - pos);
		float spec = saturate(dot(v, r));
		float ao = 1;
		col = diff * col * ao + pow(spec, 10) * ao + float3(0.5, 0.7, 1.0) * 1.9 * glows(hit.index, time);
		col *= saturate(1 - 0.03 * hit.dist);
	}
	col += saturate(glowAmt * 0.4) * float3(0.3, 0.5, 0.7);

 //   int j = Lead;
 //   if (j > 0)
 //   {
 //       j %= 5;
 //       if (hit.index == int(BeatTime) % 5)
 //       {
 //           float3 hsl = rgb2hsl(col);
 //           hsl.x = j / 20.0;
 //           hsl.z += 0.5;
 //           col = hsl2rgb(hsl);
 //       }
 //   }
	return col;
}


float4 wires(float2 xy)
{
    //
	float2 uv = xy / Resolution;
    float time = Time - uv.y * (0.33 - 0.28 * saturate(sin(1.2 * Time) / 2));
	time *= 0.2;

    // set up camera
    Camera camera;
	camera.Position = float3(6, 3, -6);
	camera.LookAt = float3(0, 0, 0);
	camera.Up.x = sin(0.6 * sin(1.4 * time));
    camera.Up.y = cos(0.6 * sin(1.4 * time));
    camera.Up.z = 0;

    // render
	Ray ray = createRay(camera, uv, 90, Aspect);
	float3 col = saturate(render(ray, time));

    //
	float2 texcoord = float2(uv.x, 1 - uv.y);
    float3 a = Texture0.Sample(Sampler0, texcoord).rgb;
	float3 finalCol = saturate(0.8 * col + a);
	return Nisse0 * float4(finalCol, 1);
}