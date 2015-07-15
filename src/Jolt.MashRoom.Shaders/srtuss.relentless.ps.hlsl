/****************************************************************************************************
 * original code by srtuss, 2013
 * https://www.shadertoy.com/view/lss3WS
 *
 * hlsl conversion, mashing and modification by papademos
 *
 * License (shadertoy default): Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported
 * http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US
 *
 * please do NOT use this shader in your own productions/videos/games without permission from srtuss
 * if you'd still like to do so, please mail him (stral@aon.at)
 ****************************************************************************************************/
#include "environment.hlsli"
#include "vanilla.pixel.hlsli"
#include "gl.hlsli"
#include "common.hlsli"
#define A2V(a) vec2(sin((a) * 6.28318531 / 100.0), cos((a) * 6.28318531 / 100.0))


/****************************************************************************************************
 * 
 ****************************************************************************************************/
float4 relentless(float2 xy);
float4 main(Pixel pixel) : SV_Target
{
    return relentless(pixel.TexCoord0 * Resolution); 
}


/****************************************************************************************************
 *
 ****************************************************************************************************/
static const float time = BeatTime;


/****************************************************************************************************
 *
 ****************************************************************************************************/
float2 rotate(float2 p, float a)
{
	return float2(p.x * cos(a) - p.y * sin(a), p.x * sin(a) + p.y * cos(a));
}

float box(float2 p, float2 b, float r)
{
	return length(max(abs(p) - b, 0.0)) - r;
}

// iq's ray-plane-intersection code
float3 intersect(in float3 o, in float3 d, float3 c, float3 u, float3 v)
{
	float3 q = o - c;
	return float3(
		dot(cross(u, v), q),
		dot(cross(q, u), d),
		dot(cross(v, q), d)) / dot(cross(v, u), d);
}

// some noise functions for fast developing
float rand11(float p)
{
    return fract(sin(p * 591.32) * 43758.5357);
}
float rand12(float2 p)
{
    return fract(sin(dot(p.xy, float2(12.9898, 78.233))) * 43758.5357);
}
float2 rand21(float p)
{
	return fract(float2(sin(p * 591.32), cos(p * 391.32)));
}
float2 rand22(in float2 p)
{
	return fract(float2(sin(p.x * 591.32 + p.y * 154.077), cos(p.x * 391.32 + p.y * 49.077)));
}

float noise11(float p)
{
	float fl = floor(p);
	return mix(rand11(fl), rand11(fl + 1.0), fract(p));
}

float fbm11(float p)
{
	return noise11(p) * 0.5 + noise11(p * 2.0) * 0.25 + noise11(p * 5.0) * 0.125;
}

float3 noise31(float p)
{
	return float3(noise11(p), noise11(p + 18.952), noise11(p - 11.372)) * 2.0 - 1.0;
}

// something that looks a bit like godrays coming from the surface
float sky(float3 p)
{
	float a = atan(p.x, p.z);
	float t = time * 0.1;
	float v = rand11(floor(a * 4.0 + t)) * 0.5 + rand11(floor(a * 8.0 - t)) * 0.25 + rand11(floor(a * 16.0 + t)) * 0.125;
	return v;
}

float3 voronoi(in float2 x)
{
	float2 n = floor(x); // grid cell id
	float2 f = fract(x); // grid internal position
	float2 mg; // shortest distance...
	float2 mr; // ..and second shortest distance
	float md = 8.0, md2 = 8.0;
	for (int j = -1; j <= 1; j ++)
	for (int i = -1; i <= 1; i ++)
	{
		float2 g = float2(i, j); // cell id
		float2 o = rand22(n + g); // offset to edge point
		float2 r = g + o - f;
			
		float d = max(abs(r.x), abs(r.y)); // distance to the edge
			
		if (d < md)
			{md2 = md; md = d; mr = r; mg = g;}
		else if (d < md2)
			{md2 = d;}
	}
	return float3(n + mg, md2 - md);
}

float circles(float2 p)
{
	float v, w, l, c;
	float2 pp;
	l = length(p);
	
    float t = floor(time);
    float f = fract(time);
    t += 1. - exp(-f*9.);
	
	pp = rotate(p, t * 3.0);
	c = max(dot(pp, normalize(float2(-0.2, 0.5))), -dot(pp, normalize(float2(0.2, 0.5))));
	c = min(c, max(dot(pp, normalize(float2(0.5, -0.5))), -dot(pp, normalize(float2(0.2, -0.5)))));
	c = min(c, max(dot(pp, normalize(float2(0.3, 0.5))), -dot(pp, normalize(float2(0.2, 0.5)))));
	
	// innerest stuff
	v = abs(l - 0.5) - 0.03;
	v = max(v, -c);
	v = min(v, abs(l - 0.54) - 0.02);
	v = min(v, abs(l - 0.64) - 0.05);
	
	pp = rotate(p, t * -1.333);
	c = max(dot(pp, A2V(-5.0)), -dot(pp, A2V(5.0)));
	c = min(c, max(dot(pp, A2V(25.0 - 5.0)), -dot(pp, A2V(25.0 + 5.0))));
	c = min(c, max(dot(pp, A2V(50.0 - 5.0)), -dot(pp, A2V(50.0 + 5.0))));
	c = min(c, max(dot(pp, A2V(75.0 - 5.0)), -dot(pp, A2V(75.0 + 5.0))));
	
	w = abs(l - 0.83) - 0.09;
	v = min(v, max(w, c));
	
	return v;
}

float shade1(float d)
{
	float v = 1.0 - smoothstep(0.0, mix(0.012, 0.2, 0.0), d);
	float g = exp(d * -20.0);
	return v + g * 0.5;
}

float4 relentless(float2 xy)
{
	float2 uv = xy / Resolution;
	uv = 2 * uv - 1;
	uv.x *= Aspect;
	
	// using an iq styled camera this time :)
	// ray origin
	float3 ro = 0.7 * float3(cos(0.2 * time), 0, sin(0.2 * time));
	ro.y = cos(0.6 * time) * 0.3 + 0.65;
	
    // camera look at
	float3 ta = float3(0, 0.2, 0);
	
	// camera shake intensity
	float shake = clamp(3 * (1 - length(ro.yz)), 0.3, 1);
	float st = mod(time, 10) * 143.0;
	
	// build camera matrix
	float3 ww = normalize(ta - ro + noise31(st) * shake * 0.01);
	float3 uu = normalize(cross(ww, normalize(float3(0, 1, 0.2 * sin(time)))));
	float3 vv = normalize(cross(uu, ww));
	
    // obtain ray direction
	float3 rd = normalize(uv.x * uu + uv.y * vv + ww);
	
	// shaking and movement
	ro += noise31(-st) * shake * 0.015;
	ro.x += 2 * time;;
	
    //
	float inten = 0;
	
	// background
	float sd = dot(rd, float3(0, 1, 0));
	inten = pow(1.0 - abs(sd), 20.0) + pow(sky(rd), 5.0) * step(0.0, rd.y) * 0.2;
	
	float3 its;
	float v, g;
	
	// voronoi floor layers
	for (int layer = 0; layer < 4; layer ++)
	{
		its = intersect(ro, rd, float3(0, -5 - 5 * layer, 0), float3(1, 0, 0), float3(0, 0, 1));
		if (its.x <= 0)
            continue;

		float3 vo = voronoi(0.05 * (its.yz) + 8 * rand21(layer));
		v = exp(-100 * (vo.z - 0.02));
		inten += v * 0.2;
			
		// add some special fx to lowest layer
		if (layer != 3)
            continue;

		float fxi = cos(0.2 * vo.x + time);
		float fx = saturate(smoothstep(0.9, 1, fxi)) * rand12(vo.xy);
		inten += fx * 2 * exp(-3 * vo.z);
	}

	// draw the gates, 4 should be enough
	float gatex = floor(ro.x / 8 + 0.5) * 8 + 4;
	for (int i = 0, go = -16; i < 4; i++, go += 8)
	{
		its = intersect(ro, rd, float3(gatex + go, 0, 0), float3(0, 1, 0), float3(0, 0, 1));
		if (dot(its.yz, its.yz) < 2 && its.x > 0)
        {
            //
		    v = circles(its.yz);
		    float shade = shade1(v);
            shade = pow(1.5 - abs(1.5 - shade), 2);
            inten += shade;
        }
	}
	
	// draw the stream
	for (int j = 0; j < 10; j ++)
    {
		float id = float(j);
		float3 bp = float3(0, (rand11(id) * 2 - 1) * 0.25, 0);
		float3 its = intersect(ro, rd, bp, float3(1, 0, 0), float3(0, 0, 1));
		if (its.x > 0.0)
        {
            //
		    float2 pp = its.yz;
		    float spd = (1 + rand11(id) * 3) * 2.5;
		    pp.y += time * spd;
		    pp += (rand21(id) * 2 - 1) * float2(0.3, 1);
		    float rep = rand11(id) + 1.5;
		    pp.y = mod(pp.y, rep * 2) - rep;
		    float d = box(pp, float2(0.02, 0.3), 0.1);
		    float foc = 0;
		    float v = 1.0 - smoothstep(0, 0.03, abs(d) - 0.001);
		    float g = min(exp(d * -20), 2);
		    inten += (v + g * 0.7) * 0.3;
        }
	}
	
    // draw the lead ray
    float hue = 0.6;
	j = Lead;
    if (j > 0)
    {
		float id = float(j);
		float3 bp = float3(0, (rand11(id) * 2 - 1) * 0.25, 0);
		float3 its = intersect(ro, rd, bp, float3(1, 0, 0), float3(0, 0, 1));
		if (its.x > 0)
        {
            //
	        float2 pp = its.yz;
	        float spd = (1 + rand11(id) * 3) * 2.5;
	        pp.y += time * spd;
	        pp += (rand21(id) * 2 - 1) * float2(0.3, 1);
	        float rep = rand11(id) + 1.5;
	        pp.y = mod(pp.y, rep * 2) - rep;
	        float d = box(pp, float2(0.02, 2.5), 0.1);
	        float foc = 0.0;
	        float v = 1.0 - smoothstep(0, 0.03, abs(d) - 0.001);
	        float g = min(exp(d * -20), 2);
	        float intenRay = (v + g * 0.7) * 1;

	        //
            inten += intenRay;
            if (intenRay > 0.1)
                hue = id / 20;
        }
    }

	inten *= 0.4 + 0.3 * (1 + sin(time));
	return float4(hsl2rgb(float3(hue, 0.9, inten)), 1);
}