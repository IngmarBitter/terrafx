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
    float3 rgb = float3(0,0,0); // alpha-premultiplied color, meaning how much this color contributes to the image
    float transparency = 1;
    for (int i = 0; i < 1024; i++)
    {
        // sample location
        float3 uvw = float3(input.uvw[0], input.uvw[1], (input.uvw[2] + i * stepSize) % 1.0);

        // sample from 3d texture
        float4 texel = textureInput.SampleLevel(samplerInput, uvw, 0, 0);

        // get and apply the gray level intensitiy from the single value float texture
        float voxel = texel[0];
        if (voxel > input.scale) {

            // gradient sample locations (y is flipped because geometry Y is up while texture Y is down)
            float gr = 1 / 256.0; // gradient sampling radius for (p)revious and (n)ext samples
            float3 cp00 = uvw + float3(-gr, 0, 0); float gp00 = textureInput.SampleLevel(samplerInput, cp00, 0, 0)[0];
            float3 cn00 = uvw + float3(+gr, 0, 0); float gn00 = textureInput.SampleLevel(samplerInput, cn00, 0, 0)[0];
            float3 c0p0 = uvw + float3(0, +gr, 0); float g0p0 = textureInput.SampleLevel(samplerInput, c0p0, 0, 0)[0];
            float3 c0n0 = uvw + float3(0, -gr, 0); float g0n0 = textureInput.SampleLevel(samplerInput, c0n0, 0, 0)[0];
            float3 c00p = uvw + float3(0, 0, -gr); float g00p = textureInput.SampleLevel(samplerInput, c00p, 0, 0)[0];
            float3 c00n = uvw + float3(0, 0, +gr); float g00n = textureInput.SampleLevel(samplerInput, c00n, 0, 0)[0];
            float3 gradient = float3(
                gn00 - gp00,
                g0n0 - g0p0,
                g00n - g00p);

            float gradientMagnitude01 = length(gradient) * scaleGradientMagnitudeToBeMax1;

            if (gradientMagnitude01 > 0.005) {

                float3 normal = -normalize(gradient);
                float diffuseShade = saturate(dot(normal, lightDir));
                float shade = ambientFraction + diffuseFraction * diffuseShade;

                // sample color and alpha
                float3 color = shade * voxel * float4(1, 1, 1, 1);
                float alpha = voxel;
                float trans = (1 - alpha);
                rgb += color * transparency; // front to back compositing with alpha-premultiplied colors
                transparency *= trans;

                if (transparency < 0.03)
                    break;
            }
        }
    }
    float accumulatedAlpha = (1 - transparency);
    rgb /= accumulatedAlpha; // convert to conventional rgb color (no longer premultiplied)
    float4 accumulatedColor = float4(rgb[0], rgb[1], rgb[2], accumulatedAlpha);
    return accumulatedColor;
}

