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

    // the quad position does not change
    float4 p4 = float4(input.position, 1.0f);
    output.position = p4;

    // the start location in the texture changes as the camera is moved
    float4 t4 = float4(input.uvw, 1.0f);
    t4 = mul(t4, primitiveTransform);
    t4 = mul(t4, frameTransform);
    output.uvw = float3(t4[0], t4[1], t4[2]);

    // the rayDirection changes as the camera is moved
    float4 d4 = float4(0,0,1,0);
    d4 = mul(d4, primitiveTransform);
    d4 = mul(d4, frameTransform);
    output.rayDir = normalize(float3(d4[0], d4[1], d4[2]));

    return output;
}
