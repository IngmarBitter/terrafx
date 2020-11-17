struct VSInput
{
    float3 position : POSITION;
    float3 uvw : TEXCOORD;
};

struct PSInput
{
    float4 position : SV_Position;
    float3 uvw : TEXCOORD;
    float scale : FLOAT;
};
