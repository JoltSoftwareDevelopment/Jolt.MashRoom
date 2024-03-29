/****************************************************************************************************
 * original code by Stephane Cuillerdier - Aiekick/2015
 * https://www.shadertoy.com/view/XljGR3
 *
 * License (shadertoy default): Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported
 * http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US
 *
 * hlsl conversion, mashing and modification by papademos
 ****************************************************************************************************/
#ifndef SKIP_HEADER
    #include "environment.hlsli"
    #include "vanilla.pixel.hlsli"
    #include "gl.hlsli"
    #define iGlobalTime (Custom[0][1])
    #define outroFade CustomCredits[0][2]
    #define outroTime CustomCredits[0][1]


    /****************************************************************************************************
     * 
     ****************************************************************************************************/
    float4 psyCave(float2 fragCoord);
    float4 main(Pixel pixel) : SV_Target
    {
	    float x = pixel.TexCoord0.x * iResolution.x;
	    float y = pixel.TexCoord0.y * iResolution.y;

        float4 color = psyCave(float2(x, y));
        return color;
    }
#endif


/****************************************************************************************************
 *
 ****************************************************************************************************/
static const vec2 uScreenSize = iResolution.xy;
static float uTime = iGlobalTime;

static const vec2 RMPrec = vec2(0.6, 0.01); // ray marching tolerance precision // low, high
static const vec2 DPrec = vec2(1e-3, 30.); // ray marching distance precision // low, high
    
// light
static const vec3 LCol = vec3(0.8,0.5,0.2);
static const vec3 LPos = vec3(-0.6, 0.7, -0.5);
static const vec3 LAmb = vec3( 0,0,0 );
static const vec3 LDif = vec3( 1. , 0.5, 0. );
static const vec3 LSpe = vec3( 0.8,0.8,0.8 );

// material
static const vec3 MCol = vec3(0,0,0);
static const vec3 MAmb = vec3( 0,0,0 );
static const vec3 MDif = vec3( 1. , 0.5, 0. );
static const vec3 MSpe = vec3( 0.6, 0.6, 0.6 );
static const float MShi =30.;
    
#define mPi 3.14159
#define m2Pi 6.28318

static vec2 s,g,uv,m;

vec2 uvs(vec3 p) // uv sphere
{
	p = normalize(p);
	vec2 sp;
	sp.x = atan(p.z, p.x) / (m2Pi+1.27);
	sp.y = asin(p.y) / (mPi);
	return sp;
}

float smin( float a, float b, float k )
{
	float h = clamp( .5+.5*(b-a)/k, 0., 1. );
	return mix( b, a, h ) - k*h*(1.-h);
}

float sdCyl( vec3 p, vec2 h )
{
  	vec2 d = abs(vec2(length(p.xz),p.y)) - h;
  	return min(max(d.x,d.y),0.0) + length(max(d,0.0));
}

// return color from temperature 
//http://www.physics.sfasu.edu/astro/color/blackbody.html
//http://www.vendian.org/mncharity/dir3/blackbody/
//http://www.vendian.org/mncharity/dir3/blackbody/UnstableURLs/bbr_color.html
vec3 getHotColor(float Temp) // blackbody temperature color
{
	vec3 col = 255*vec3(1,1,1);
	col.x = 56100000. * pow(Temp,(-3. / 2.)) + 148.;
   	col.y = 100.04 * log(Temp) - 623.6;
   	if (Temp > 6500.) col.y = 35200000. * pow(Temp,(-3. / 2.)) + 184.;
   	col.z = 194.18 * log(Temp) - 1448.6;
   	col = clamp(col, 0., 255.)/255.;
	if (Temp < 1000.) col *= Temp/1000.;
    col.g += col.r - 0.3;
    //col.g *= 0.9;
    //col.g = pow(clamp(col.g, 0., 1.), 100.);
    //col.g = clamp(col.g, 0., 1.);
    col.b += col.r;
    col.r *= 0.5;
    col *= 0.7;
   	return col;
}

vec2 getTemp(vec3 p)
{
	p*=2.;
	float r = fract(p.x+p.z);
	return vec2(dot(p,p)*(1000.)*r,r);
}

//--------------------------------------------------------------------------
// Grab all sky information for a given ray from camera
// from Dave Hoskins
vec3 GetSky(in vec3 rd, in vec3 sunDir, in vec3 sunCol)
{
	float sunAmount = max( dot( rd, sunDir), 0.0 );
	float v = 1;//pow(1.0-max(rd.y,0.0),6.);
	vec3  sky = mix(vec3(.1, .2, .3), vec3(.32, .32, .32), v);
	sky = sky + sunCol * sunAmount * sunAmount * .25;
	sky = sky + sunCol * min(pow(sunAmount, 800.0)*1.5, .3);
	return clamp(sky, 0.0, 1.0);
}

vec4 cave_map(vec3 p)
{
	vec2 res = vec2(0,0);
    
	float t = sin(uTime*0.2)*.5+.5;
	t*=2.;
    
	float rugo=cos(2.*p.x)*sin(1.5*p.z)*sin(3.*p.y)*cos(1.3);
    
	vec3 sci = vec3(0.9,2.,0.9);// scale in
	vec3 pob = vec3(0.,5.,0.);// pos bottom
	vec3 pocy = vec3(0.,-5.,0.);//pos cyl
    
	vec3 col = getHotColor(getTemp(p).x);
    
	float diamhole = 1.1;
	float spi = length(p*sci) - 4.5 - rugo;//in
	float spb = length(p+pob) - 4.5 + dot(col, vec3(0.01, 0.01, 0.01));//bottom
	float spo = spi - 1. ;//out

	float cyl = sdCyl(p+pocy, vec2(diamhole,4.));//top hole
    
	float disp = dot(col, vec3(0.03,0.03,0.03));
    
	float spicyl = smin(spi,cyl,0.6) + disp;
	float cavern = smin(max(-spicyl, spo ), spb, 3.5);
        
	return vec4(cavern, col);
}

vec3 nor(vec3 p, float prec)
{
	vec2 e = vec2(prec, 0.);
    
	vec3 n;
    
	n.x = cave_map(p+e.xyy).x - cave_map(p-e.xyy).x; 
	n.y = cave_map(p+e.yxy).x - cave_map(p-e.yxy).x; 
	n.z = cave_map(p+e.yyx).x - cave_map(p-e.yyx).x;  
    
	return normalize(n); 
}

vec3 ads( vec3 p, vec3 n )
{
	vec3 ldif = normalize( LPos - p);
	vec3 vv = normalize( vec3(0,0,0) - p );
	vec3 refl = reflect( vec3(0,0,0) - ldif, n );
    
	vec3 amb = MAmb*LAmb;
	vec3 dif = max(0., dot(ldif, n.xyz)) * MDif * LDif;
	vec3 spe = vec3( 0,0,0 );
	if( dot(ldif, vv) > 0.)
		spe = pow(max(0., dot(vv,refl)),MShi)*MSpe*LSpe;
    
	vec3 col = amb*1.2 + dif*1.5 + spe*0.8;
    
	return col;
}

vec4 scn(vec4 col, vec3 ro, vec3 rd)
{
	float s = DPrec.x;
	float d = 0.;
	vec3 p = ro+rd*d;
	vec4 c = col;
    
	float b = 0.35;
    
	for(int i=0;i<200;i++)
	{
		if(s<DPrec.x||s>DPrec.y) break;
		s = cave_map(p).x;
		d += s*(s>DPrec.x?RMPrec.x:RMPrec.y);
		p = ro+rd*d;
	}	
    
	float lightIntensity = sin(uTime*0.2)*.5;

	if (s<DPrec.x)
	{
		vec2 r = getTemp(p);
	
		vec3 n = nor(p, r.y); 
      	
		c.rgb = cave_map(p).yzw + dot(n,rd) + ads(p,n) * lightIntensity;
	}
	else
	{
		vec3 dir = -normalize(vec3(2.,10.,0.));
		vec3 col = vec3(lightIntensity,lightIntensity,lightIntensity);
		c.rgb = GetSky(rd, dir, col);
	}
    
	return c;
}

vec3 cam(vec2 uv, vec3 ro, vec3 cu, vec3 org, float persp)
{
	vec3 rorg = normalize(org-ro);
	vec3 u =  normalize(cross(cu, rorg));
	vec3 v =  normalize(cross(rorg, u));
	vec3 rd = normalize(rorg + u*uv.x + v*uv.y);
	return rd;
}

vec4 Image(in vec2 fragCoord )
{
	s = uScreenSize;
	g = fragCoord;
	uv = (2.*g-s)/s.y;
	
	float t = uTime*.2;
	float ts = sin(t)*.5+.5;
    
	float axz = -t/2.; // angle XZ
	float axy = 2.6 + 0.42*ts; // angle XY // inf 3.02 // sup 2.60
	float cd = 3.;// cam dist to scene origine

	//axy = 2.6; // on bloque la camera an haut pour mise au point
    
	float ap = 1.; // angle de perspective
	vec3 cu = vec3(0.,1.,0.); // cam up 
	vec3 org = vec3(0., 0.8, 0.); // scn org
	vec3 ro = vec3(cos(axz),sin(axy),sin(axz))*cd; // cam org
    
	vec3 rd = cam(uv, ro, cu, org, ap);
    
	vec4 c = vec4(0.,0.,0.,1.); // col
    
	c = scn(c, ro, rd);//scene
	
	return c;
}

float4 psyCave(float2 fragCoord)
{
    float4 fragColor = saturate(Image(fragCoord));
	vec2 q = fragCoord.xy / iResolution.xy;
    float a = texture2D( Texture0, q*float2(1,-1)).r;
    float b = texture2D( Texture1, q*float2(1,-1)).r;
    fragColor *= 0.3 * Nisse0;
    fragColor += float4(a,a,a,a) * Nisse1;
    fragColor += float4(b,b,b,b) * Nisse2;
    return fragColor;
}