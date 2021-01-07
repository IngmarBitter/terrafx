#include "VrgTypes.hlsl"

Texture1D texture1dInput : register(t0);
Texture3D texture3dInput : register(t1);
SamplerState sampler1dInput : register(s0);
SamplerState sampler3dInput : register(s1);

float4 main(PSInput input) : SV_Target
{
    float stepSize = 1.0 / 512;
    float scaleGradientMagnitudeToBeMax1 = 1.0 / length(float3(1, 1, 1));
    float3 lightDir = -input.rayDir; // direction toward the light
    float ambientFraction = 0.25;
    float diffuseFraction = 1.0 - ambientFraction;
    float3 rgb = float3(0,0,0); // alpha-premultiplied color, meaning how much this color contributes to the image
    float transparency = 1;
    bool isInside = false;
    for (int i = 0; i < 1024; i++)
    {
        // sample location
        float3 uvw = input.uvw + i * stepSize * input.rayDir;

        // skip this sample if it is outside the data FoV
        if (uvw[0] < 0 || uvw[0] > 1 ||
            uvw[1] < 0 || uvw[1] > 1 ||
            uvw[2] < 0 || uvw[2] > 1) {
            if (isInside)
                break;
            else 
                continue;
        }
        isInside = true;

        // sample from 3d texture
        float4 texel = texture3dInput.SampleLevel(sampler3dInput, uvw, 0, 0);

        // lookup matching RGBA
        //float4 sampleRgba = texture1dInput.SampleLevel(sampler1dInput, texel[0], 0, 0); // IB: reactivate this when multi texture allocation is fixed
        float4 sampleRgba = float4(texel[0], texel[0], texel[0], texel[0]);
        float3 sampleRgb = float3(sampleRgba[0], sampleRgba[1], sampleRgba[2]);
        float sampleAlpha = sampleRgba[3];
        if (sampleAlpha < 0.5) sampleAlpha = 0.0;

        // get and apply the gray level intensitiy from the single value float texture
        float voxel = texel[0];
        if (sampleAlpha > 0) {

            // gradient sample locations (y is flipped because geometry Y is up while texture Y is down)
            float gr = 1 / 256.0; // gradient sampling radius for (p)revious and (n)ext samples
            float3 cp00 = uvw + float3(-gr, 0, 0); float gp00 = texture3dInput.SampleLevel(sampler3dInput, cp00, 0, 0)[0];
            float3 cn00 = uvw + float3(+gr, 0, 0); float gn00 = texture3dInput.SampleLevel(sampler3dInput, cn00, 0, 0)[0];
            float3 c0p0 = uvw + float3(0, +gr, 0); float g0p0 = texture3dInput.SampleLevel(sampler3dInput, c0p0, 0, 0)[0];
            float3 c0n0 = uvw + float3(0, -gr, 0); float g0n0 = texture3dInput.SampleLevel(sampler3dInput, c0n0, 0, 0)[0];
            float3 c00p = uvw + float3(0, 0, -gr); float g00p = texture3dInput.SampleLevel(sampler3dInput, c00p, 0, 0)[0];
            float3 c00n = uvw + float3(0, 0, +gr); float g00n = texture3dInput.SampleLevel(sampler3dInput, c00n, 0, 0)[0];
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
                float3 shadedColor = shade * sampleRgb;
                float trans = (1 - sampleAlpha);
                rgb += shadedColor * sampleAlpha * transparency; // front to back compositing with accumulation of alpha-premultiplied colors, but the new sample is not premultiplied
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

