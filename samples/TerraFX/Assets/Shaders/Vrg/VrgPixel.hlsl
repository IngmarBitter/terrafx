#include "VrgTypes.hlsl"

Texture3D textureInput : register(t0);
SamplerState samplerInput : register(s0);

float4 main(PSInput input) : SV_Target
{
    float r = 0;
    float g = 0;
    float b = 0;
    float a = 0;
    for (int i = 0; i < 100; i++)
    {
        // get and apply the gray level intensitiy from the single value float texture
        float3 uvw = float3(input.uvw[0], input.uvw[1], (input.uvw[2] + i * 0.01) % 1.0);
        float4 texel = textureInput.Sample(samplerInput, uvw);
        float4 color = texel[0] * float4(1, 1, 1, 1);
        r = r * a + color[0] * (1 - a);
        g = g * a + color[1] * (1 - a);
        b = b * a + color[2] * (1 - a);
        a = 1 - (1 - a) * (1 - color[3]);
    }
    float4 accumulatedColor = float4(r,g,b,a);
    return accumulatedColor;
}

