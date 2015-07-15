/****************************************************************************************************
 * 
 ****************************************************************************************************/
float3x3 CreateRotationMatrixX(float a)
{
    return float3x3(
        1,  0,      0,
        0,  cos(a), -sin(a),
        0,  sin(a), cos(a));
}


float3x3 CreateRotationMatrixY(float a)
{
    return float3x3(
        cos(a), 0,  -sin(a),
        0,      1,  0,
        sin(a), 0,  cos(a));
}


float3x3 CreateRotationMatrixZ(float a)
{
    return float3x3(
        cos(a), -sin(a),    0,
        sin(a), cos(a),     0,
        0,      0,          1);
}


float2x2 CreateRotationMatrix2D(float a)
{
    return float2x2(
        sin(a),  cos(a),
        cos(a), -sin(a));
}


/****************************************************************************************************
 * 
 ****************************************************************************************************/
float3 hsv2rgb(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}


//float3 hsl2rgb(float3 c)
//{
//    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
//    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
//    float l = saturate(c.z);
//    float3 rgb;
//    if (l < 0.5)
//        rgb = l * 2 * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
//    else
//        rgb = lerp(lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y), float3(1,1,1), 2*(l-0.5));
//    return rgb;
//}


float3 hsl2rgb(float3 hsl)
{
	//
    float hue = 6.0 * frac(hsl.x);
    float saturation = saturate(hsl.y);
    float lightness =  saturate(hsl.z);

    //
    float chroma = (1.0 - abs(2.0 * lightness - 1.0)) * saturation;
    float second = chroma * (1.0 - abs(hue % 2.0 - 1.0));
	float3 rgb = float3(1, 1, 1) * (lightness - 0.5 * chroma);
	if 		(hue < 1.0) rgb += float3(chroma, second, 0.0);
	else if (hue < 2.0) rgb += float3(second, chroma, 0.0);
	else if (hue < 3.0) rgb += float3(0.0,    chroma, second);
	else if (hue < 4.0) rgb += float3(0.0,    second, chroma);
	else if (hue < 5.0) rgb += float3(second, 0.0,    chroma);
	else                rgb += float3(chroma, 0.0,    second);

    //
    return rgb;
}


float3 rgb2hsl(float3 rgb)
{
    //
    float maxComponent = max(rgb.r, max(rgb.g, rgb.b));
    float minComponent = min(rgb.r, min(rgb.g, rgb.b));
    float chroma = maxComponent - minComponent;
    
	//
    float hue;
    if 		(rgb.r == maxComponent) hue = rgb.g - rgb.b + 0.0;
    else if (rgb.g == maxComponent) hue = rgb.b - rgb.r + 2.0;
    else    						hue = rgb.r - rgb.g + 4.0;
    hue = hue % 6.0 / 6.0;   

    //
    float lightness = 0.5 * (maxComponent + minComponent);
    
    //
    float saturation;
    if (0.0 < lightness && lightness < 1.0)
        saturation = chroma / (1.0 - abs(2.0 * lightness - 1.0));
    else
        saturation = 0.0;

    //
    return float3(hue, saturation, lightness);
}


/****************************************************************************************************
 *
 ****************************************************************************************************/
struct Camera
{
    float3 Position;
    float3 LookAt;
    float3 Up;
};


float3 Camera_GetDirection(Camera camera)
{
    return normalize(camera.LookAt - camera.Position);
}
