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

    output.uvw = input.uvw;

    v4 = float4(0,0,0,1);
    v4 = mul(v4, primitiveTransform);
    v4 = mul(v4, frameTransform);
    output.scale = v4[2];

    return output;
}
