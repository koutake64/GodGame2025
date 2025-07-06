TEXTURE2D_X(_TextureSource);
SAMPLER(sampler_TextureSource);

float2 rotateUV(float2 uv, float rotation)
{
    rotation = radians(rotation);

    uv = uv * 2 - 1;
    float c = cos(rotation);
    float s = sin(rotation);
    
    uv = mul(float2x2(c, -s, s, c), uv);
    uv = uv * 0.5 + 1;

    return uv;
}

float2 rotationVector(float rotation)
{
    rotation = radians(rotation);

    float c = cos(rotation);
    float s = sin(rotation);

    return float2(c, s);
}

half4 sampleTextureSource(float2 uv)
{
    uv.x *= _ScreenParams.x / _ScreenParams.y;

    uv = TRANSFORM_TEX(uv, _TextureSource);

    // rotate using a matrix
    uv = rotateUV(uv, _TextureRotation);

    // animate the uvs
    uv += rotationVector(_TextureAnimationDirection) * _Time.y * _TextureAnimationSpeed;

    // sample the texture
    half4 color = SAMPLE_TEXTURE2D_X(_TextureSource, sampler_TextureSource, uv);
    
    // apply the tint color
    color *= _Color;
    
    return color;
}