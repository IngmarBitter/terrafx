#include "VrgTypes.hlsl"

Texture3D textureInput : register(t0);
SamplerState samplerInput : register(s0);

float4 main(PSInput input) : SV_Target
{
    float stepSize = 1.0 / 1024;
    float scaleGradientMagnitudeToBeMax1 = 1.0 / length(float3(1, 1, 1));
    float3 lightDir = normalize(float3(0, 1, -1)); // direction toward the light
    float ambientFraction = 0.25;
    float diffuseFraction = 1.0 - ambientFraction;
    float r = 0;
    float g = 0;
    float b = 0;
    float a = 0;
    for (int i = 0; i < 1024; i++)
    {
        // sample location
        float3 uvw = float3(input.uvw[0], input.uvw[1], (input.uvw[2] + i * stepSize) % 1.0);

        // sample from 3d texture
        float4 texel = textureInput.Sample(samplerInput, uvw);

        // get and apply the gray level intensitiy from the single value float texture
        float voxel = texel[0];
        if (voxel > input.scale) {

            // gradient sample locations (y is flipped because geometry Y is up while texture Y is down)
            float gr = 1 / 256.0; // gradient sampling radius for (p)revious and (n)ext samples
            float3 cp00 = uvw + float3(-gr, 0, 0); float gp00 = textureInput.Sample(samplerInput, cp00);
            float3 cn00 = uvw + float3(+gr, 0, 0); float gn00 = textureInput.Sample(samplerInput, cn00);
            float3 c0p0 = uvw + float3(0, +gr, 0); float g0p0 = textureInput.Sample(samplerInput, c0p0);
            float3 c0n0 = uvw + float3(0, -gr, 0); float g0n0 = textureInput.Sample(samplerInput, c0n0);
            float3 c00p = uvw + float3(0, 0, -gr); float g00p = textureInput.Sample(samplerInput, c00p);
            float3 c00n = uvw + float3(0, 0, +gr); float g00n = textureInput.Sample(samplerInput, c00n);
            float3 gradient = float3(
                gn00 - gp00,
                g0n0 - g0p0,
                g00n - g00p);

            float gradientMagnitude01 = length(gradient) * scaleGradientMagnitudeToBeMax1;

            if (gradientMagnitude01 > 0.005) {

                float3 normal = -normalize(gradient);
                float diffuseShade = saturate(dot(normal, lightDir));
                float shade = ambientFraction + diffuseFraction * diffuseShade;

                float4 color = voxel * float4(1, 1, 1, 1);
                color = color * shade;
                r = r * a + color[0] * (1 - a);
                g = g * a + color[1] * (1 - a);
                b = b * a + color[2] * (1 - a);
                a = 1 - (1 - a) * (1 - color[3]);
            }
        }
    }
    float4 accumulatedColor = float4(r,g,b,a);
    return accumulatedColor;
}

