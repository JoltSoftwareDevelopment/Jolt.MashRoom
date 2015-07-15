/****************************************************************************************************
 * original code by bear
 * https://www.shadertoy.com/view/ldj3zV
 * 
 * License (shadertoy default): Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported
 * http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US
 *
 * hlsl conversion, mashing and modification by papademos
 ****************************************************************************************************/
#include "environment.hlsli"
#include "vanilla.pixel.hlsli"
#include "gl.hlsli"
#include "common.hlsli"
#define iGlobalTime (Custom[0][1])
#define SKIP_HEADER
    #include "aiekick.alienCavern.ps.hlsl"
#undef SKIP_HEADER
#define iGlobalTime (Custom[0][1])


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
static const float maxDepth = 64.0;
static const int maxIterations = 256;
static const float epsilon = 0.0001;

float plane( vec3 p )
{
    return p.y;
}

float sphere(vec3 p, float s)
{
    return (length(p)-s);
}
static const float jox = 6.3;
float wigglesphere(vec3 p, float s)
{
    //return 0.02*sin(35.0*p.y + 0*8.*iGlobalTime) + 0.1*cos(5.*p.z + 3.0 * iGlobalTime) + sphere(p, s);
    return 0.02*sin(35.0*p.y + BeatTime) + 0.1*cos(5.*p.z + jox*BeatTime) + sphere(p, s);
}
    
float wigglesphere1(vec3 p, float s)
{
    //return 0.1*cos(5.*p.z + 3.0 * iGlobalTime) + sphere(p, s);
    return 0.1*cos(5.*p.z + jox*BeatTime) + sphere(p, s);
}

vec2 opU(vec2 d1, vec2 d2)
{
    return (d1.x < d2.x) ? d1 : d2;
}
    
vec2 map(in vec3 p) 
{
    //vec2 res = opU( vec2(plane(vec3(p.x, p.y + 0.6, p.z)), 1.0), 
    //            vec2(max(wigglesphere(p, 0.55), - sphere(vec3(p.x + 0.2, p.y + 0.2*sin(iGlobalTime)-0.15, p.z+0.1*cos(2.*iGlobalTime)), 0.4)), 2.0)
    //            );
    vec2 res = opU( vec2(plane(vec3(p.x, p.y + 0.6, p.z)), 1.0), 
                vec2(max(wigglesphere(p, 0.55), - sphere(vec3(p.x + 0.2, p.y + 0.2*sin(2*BeatTime)-0.15, p.z+0.1*cos(2.*BeatTime)), 0.4)), 2.0)
                );
    //res = opU(res, vec2(wigglesphere1(p, 0.55), 1.0)); // for debugging
    return res;
}

vec2 castRay( in vec3 ro, in vec3 rd, in float maxd )
{
    float h=epsilon*2.0; // step size
    vec2 result;    // result of distance check
    float t = 0.0;	// distance travelled
    float m = -1.0; // material
    for( int i=0; i<maxIterations; i++ )
    {
        //if( abs(h)<epsilon||t>maxd ) continue;//break;
        if(!( abs(h)<epsilon||t>maxd ))
        {
            t += h;
            result = map(ro+rd*t);
            h = result.x;
            m = result.y;
        }
    }

    return vec2(t,m);
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

float softshadow( in vec3 ro, in vec3 rd, in float mint, in float maxt, in float k )
{
    float res = 1.0;
    float t = mint;
    for( int i=0; i<30; i++ )
    {
        if( t<maxt )
        {
        float h = map( ro + rd*t ).x;
        res = min( res, k*h/t );
        t += 0.02;
        }
    }
    return clamp( res, 0.0, 1.0 );

}

float calcAO( in vec3 pos, in vec3 nor )
{
    float totao = 0.0;
    float sca = 1.0;
    for( int aoi=0; aoi<5; aoi++ )
    {
        float hr = 0.01 + 0.05*float(aoi);
        vec3 aopos =  nor * hr + pos;
        float dd = map( aopos ).x;
        totao += -(dd-hr)*sca;
        sca *= 0.75;
    }
    return clamp( 1.0 - 4.0*totao, 0.0, 1.0 );
}

float4 render( in vec3 ro, in vec3 rd )
{ 
    vec3 col = vec3(0,0,0);
    vec2 result = castRay(ro,rd,maxDepth);
    float t = result.x;
    float m = result.y;
        
    if( t < maxDepth ) // raymarch converged after t steps
    {
        vec3 pos = ro + t*rd; // end position = ray origin + distance traveled in ray direction
        vec3 normal = calcNormal( pos );
        vec3 light = normalize( vec3(0.5, 2.2, 1.1) );
        float ao = calcAO(pos, normal);
            
        if (m > 1.0)
        {
            float dif = sqrt(wigglesphere1(pos, 0.528) - wigglesphere(pos, 0.55));
            float diffuse = (10.0*sin(2.*BeatTime)+20.0)*clamp(dot(normal, light), 0.0, 1.0);
            //col = vec3(dif, dif, dif);
            col = vec3(0.8*diffuse * dif, 0.4*dif*diffuse, 2.5*dif);
        }
        else
        {
            col = float3(0,0,0);
   //         return float4(0,0,0,0);

   //         float amb = clamp( 0.5+0.5*normal.y, 0.0, 1.0 );
   //         float diffuse = clamp(dot(normal, light) - (t / maxDepth), 0.0, 1.0);
   //         float bac = clamp( dot( normal, normalize(vec3(-light.x,0.0,-light.z))), 0.0, 1.0 )*clamp( 1.0-pos.y,0.0,1.0);
   //         vec3 brdf = vec3(0,0,0);
   //         float sh = softshadow( pos, light, 0.02, 10.0, 7.0 );
   //         float pp = clamp( dot( reflect(rd,normal), light ), 0.0, 1.0 );
   //         float spe = sh*pow(pp,16.0);
			//vec3 tex = texture2D(iChannel0, vec2(pos.x, pos.z + iGlobalTime - max(0.0, 0.3*sin(3.*iGlobalTime + 3.)))).rgb;

   //         diffuse *= sh;
   //         brdf += amb*vec3(0.1, 0.11, 0.13) * ao;
   //         brdf += 0.2*bac*vec3(0.15, 0.15, 0.15) * ao;
   //         brdf += 1.2*diffuse*vec3(1.0, 0.9, 0.7);
   //         col = vec3(diffuse,diffuse,diffuse) * brdf * tex + vec3(1,1,1)*col*spe;

        }
    }

    col = clamp(col,0.0,1.0);
    float a;
    if (length(col) < 0.0001)
        a = 0;
    else
        a = 1;

    //return vec4( clamp(col,0.0,1.0), 1);
    return vec4( col, a);
}

#define lerpTime (CustomCredits[0][1])


void mainImage( out vec4 fragColor, in vec2 fragCoord )
{
    vec2 q = fragCoord.xy/iResolution.xy;
    //q.x += 0.1;
    q.x += LerpTime - 0.5;
    vec2 p = -1.0+2.0*q;
    p.x *= iResolution.x/iResolution.y;
    vec2 mo = iMouse.xy/iResolution.xy;
             
    // camera	
    vec3 rorigin = vec3(3.2*cos(6.0*mo.x), 2.0*mo.y, 3.2*sin(6.0*mo.x) );
    vec3 ta = vec3( -0.5, -0.4, 0.5 );
  
    // camera tx
    vec3 cw = normalize( ta-rorigin );
    vec3 cp = vec3( 0.0, 1.0, 0.0 );
    vec3 cu = normalize( cross(cw,cp) );
    vec3 cv = normalize( cross(cu,cw) );
    vec3 rdir = normalize( p.x*cu + p.y*cv + 2.5*cw );

        
    float4 ball = render( rorigin, rdir );
    ball.rgb = hsl2rgb(float3(0.6, 0.9, length(ball.rgb)));
    float4 background = psyCave(fragCoord);
    float3 color = lerp(background.rgb, ball.rgb, ball.a);

    fragColor=vec4( color, 1.0 );
}