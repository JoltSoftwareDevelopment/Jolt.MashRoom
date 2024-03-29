/****************************************************************************************************
 * original code by Cale Bradbury 2015
 * https://www.shadertoy.com/view/MlS3Rt
 *
 * based on code by inigo quilez - iq/2013
 * https://www.shadertoy.com/view/Xds3zN 
 *
 * hlsl conversion, mashing and modification by papademos
 *
 * License (shadertoy default): Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported
 * http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US
 ****************************************************************************************************/
#include "environment.hlsli"
#include "vanilla.pixel.hlsli"
#include "gl.hlsli"
#include "common.hlsli"
#define SKIP_HEADER
    #include "ryk.frontierOfWires.ps.hlsl"
#undef SKIP_HEADER


/****************************************************************************************************
 * 
 ****************************************************************************************************/
void mainImage(out float4 fragColor, in float2 fragCoord);
float4 main(Pixel pixel) : SV_Target
{
	float x = pixel.TexCoord0.x * iResolution.x;
	float y = pixel.TexCoord0.y * iResolution.y;

    float4 color;
    mainImage(color, float2(x, y)); 
    return color;
}


/****************************************************************************************************
 *
 ****************************************************************************************************/
float opS( float d1, float d2 )
{
    return max(-d2,d1);
}

vec2 opU( vec2 d1, vec2 d2 )
{
	return (d1.x<d2.x) ? d1 : d2;
}

vec3 opRep( vec3 p, vec3 c )
{
    return mod(p,c)-0.5*c;
}

float smin( float a, float b, float k )
{
    float h = clamp( 0.5+0.5*(b-a)/k, 0.0, 1.0 );
    return mix( b, a, h ) - k*h*(1.0-h);
}

float sphere( vec3 p, float s ){
  return length(p)-s;
}

float box( vec3 p, float s )
{
  return length(max(abs(p)-vec3(s,s,s),0.0));
}

vec2 map( in vec3 pos )
{
    float res = sphere(pos+vec3(0.0,0.,0.),.5);
    float t = iGlobalTime;
    float amp = sin(t*.5)*.5+1.;
    for(float i = 0.; i<10.;i++){
    	float a = i+t;
        vec3 off = vec3(sin(t*1.2+i*.3)*amp*5.,sin(t+cos(i*.5+t))*amp,cos(t*.45-i*2.)*amp*2.);
		vec3 ppos = pos+off;
        float d = length(ppos)*.3;
        ppos.xy = mul(mat2(cos(a),sin(a),-sin(a),cos(a)) ,ppos.xy);
        ppos.zy = mul(mat2(cos(-a),sin(a),-sin(a),cos(a)),ppos.zy);
      	//ppos*=mat3(d,0.,0.,0.,d,0.,0.,0.,d);
        ppos = mul(mat3(1.,d,d,d,1.,d,d,d,1.), ppos);
        //ppos*=mat3(d,d,d,d,d,d,d,d,d);
        ppos = mul(mat3(sin(d+t),(d),d*d,d,d,d,-d,i*sin(d+i),-1.), ppos);
        
        res = smin(res, box(ppos,.2), 4.);
    }
	
    return vec2(res,5.0);
}

vec2 castRay( in vec3 ro, in vec3 rd )
{
    float tmin = 0.;
    float tmax = 20.0;
    
#if 0
    float tp1 = (0.0-ro.y)/rd.y; if( tp1>0.0 ) tmax = min( tmax, tp1 );
    float tp2 = (1.6-ro.y)/rd.y; if( tp2>0.0 ) { if( ro.y>1.6 ) tmin = max( tmin, tp2 );
                                                 else           tmax = min( tmax, tp2 ); }
#endif
    
	float precis = 0.01;
    float t = tmin;
    float m = 0.0;
    for( int i=0; i<30; i++ )
    {
	    vec2 res = map( ro+rd*t );
        if( res.x<precis || t>tmax ) break;
        t += res.x;
	    m = res.y;
    }

    if( t>tmax ) m=-1.0;
    return vec2( t, m );
}

float softshadow( in vec3 ro, in vec3 rd, in float mint, in float tmax )
{
	float res = 1.0;
    float t = mint;
    for( int i=0; i<16; i++ )
    {
		float h = map( ro + rd*t ).x;
        res = min( res, 8.0*h/t );
        t += clamp( h, 0.02, 0.10 );
        if( h<0.001 || t>tmax ) break;
    }
    return clamp( res, 0.0, 1.0 );

}

vec3 calcNormal( in vec3 pos )
{
	vec3 eps = vec3( 0.001, 0.0, 0.0 );
	vec3 nor = vec3(
	    map(pos+eps.xyy).x - map(pos-eps.xyy).x,
	    map(pos+eps.yxy).x - map(pos-eps.yxy).x,
	    map(pos+eps.yyx).x - map(pos-eps.yyx).x );
	return normalize(nor);
}

float calcAO( in vec3 pos, in vec3 nor )
{
	float occ = 0.0;
    float sca = 1.0;
    for( int i=0; i<5; i++ )
    {
        float hr = 0.01 + 0.12*float(i)/4.0;
        vec3 aopos =  nor * hr + pos;
        float dd = map( aopos ).x;
        occ += -(dd-hr)*sca;
        sca *= 0.95;
    }
    return clamp( 1.0 - 3.0*occ, 0.0, 1.0 );    
}

vec3 background(float2 xy)
{
    return wires(xy).xyz;
}

vec4 render( in vec3 ro, in vec3 rd, float c, float2 xy )
{ 
    vec2 res = castRay(ro,rd);
    float t = res.x;
	float m = res.y;
    vec3 col = vec3(c,c,c);
    if( m>-.5 )
    {
        vec3 pos = ro + t*rd;
        vec3 nor = calcNormal( pos );
        vec3 ref = reflect( rd, nor );
        
        // light
        float occ = 0.5;//calcAO( pos, nor );
		vec3  lig = normalize( vec3(-0.6, 0.7, -0.5) );
		float amb = clamp( 0.5+0.5*nor.y, 0.5, 1.0 );
        float dif = clamp( dot( nor, lig ), 0.5, 1.0 );
        float bac = clamp( dot( nor, normalize(vec3(-lig.x,0.0,-lig.z))), 0.0, 1.0 )*clamp( 1.0-pos.y,0.0,1.0);
        float dom = smoothstep( -0.1, 0.1, ref.y );
        float fre = pow( clamp(1.0+dot(nor,rd),0.5,1.0), 2.0 );
		float spe = pow(clamp( dot( ref, lig ), 0.5, 1.0 ),16.0);
        
      // dif *= softshadow( pos, lig, 0.01, .5 );
        dom *= softshadow( pos, ref, 0.02, 2.5 );

		vec3 brdf = vec3(0,0,0);
        brdf += 1.20*dif;
		brdf += 1.20*spe*dif;
        brdf += 0.30*amb*occ;
        brdf += 0.40*dom*occ;
        brdf += 0.30*bac*occ;
        brdf += 0.40*fre*occ;
		//brdf += 0.02;
		col = brdf;

    	//col = mix( col, vec3(0.8,0.9,1.0), 1.0-exp( -0.0005*t*t ) );
    	return vec4( clamp(col,0.0,1.0), 1 );

    }

	return float4(background(xy), 0);
}

mat3 setCamera( in vec3 ro, in vec3 ta, float cr )
{
	vec3 cw = normalize(ta-ro);
	vec3 cp = vec3(sin(cr), cos(cr),0.0);
	vec3 cu = normalize( cross(cw,cp) );
	vec3 cv = normalize( cross(cu,cw) );
    return mat3( cu, cv, cw );
}

void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
	vec2 q = fragCoord.xy/iResolution.xy;
    vec2 p = -1.0+2.0*q;
    p *= (1 + pow(frac(BeatTime), 10));
	p.x *= iResolution.x/iResolution.y;
		 
	float time = 15.0 + iGlobalTime*5.;
	float camDist = 13.;
	// camera	
	vec3 ro = vec3( -0.5+camDist*cos(0.1*time), 5.0, 0.5 + camDist*sin(0.1*time) );
	vec3 ta = vec3( -0.001, -0., 0. );
	
	// camera-to-world transformation
    mat3 ca = setCamera( ro, ta, 0.0 );
    
    // ray direction
	vec3 rd = mul(normalize( vec3(p.xy,2.5) ), ca);

    // render	
    vec4 col = render( ro, rd, q.y*.3+.1, fragCoord );

    if (col.a > 0.5)
    {
	    col.rgb = pow( col, vec3(0.4545, 0.4545, 0.4545) );
        col.rgb *= 1.-length((q*2.-1.))*.3;
        col.rgb = hsl2rgb(float3(0.6, 0.9, 0.5*length(col)));
    }
    fragColor = vec4( col );
}
