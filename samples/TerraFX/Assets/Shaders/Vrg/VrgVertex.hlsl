#include "VrgTypes.hlsl"
cbuffer PerFrameInput : register(b0)
{
    matrix frameTransform;
};

cbuffer PerPrimitiveInput : register(b1)
{
    matrix primitiveTransform;
};

PSInput main(VSInput input)
{
    PSInput output;

    float4 v4 = float4(input.position, 1.0f);
    // the quad position does not change
    output.position = v4;
    // the scale dops of as the smoke rises from bottom to top
    output.scale = (1.0 - v4[1]);

    // the texture coordinates are animated to have the rising smoke visual effect
    v4 = mul(v4, primitiveTransform);
    v4 = mul(v4, frameTransform);
    output.uvw = float3(v4[0], v4[1], v4[2]);

    return output;
}
