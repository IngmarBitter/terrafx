using Ptr = System.IntPtr;
using U16 = System.UInt16;
using U32 = System.UInt32;
using I64 = System.Int64;
using I16 = System.Int16;
using F32 = System.Single;
using U8 = System.Byte;
using U64 = System.UInt64;
using I8 = System.Char;
using I32 = System.Int32;
using F64 = System.Double;
using TerraFX.Numerics;
using System;
using System.Diagnostics;
using System.IO;

public class RenderParams
{

    private readonly U32[] _intensityTexDims;
    private readonly Vector3 _intensityTexSpacingMm;
    private readonly I16[] _intensityTexure3d;
    private readonly U32[] _intensityToRgba;

    public U32[] IntensityTexDims => _intensityTexDims;
    public Vector3 IntensityTexSpacingMm => _intensityTexSpacingMm;
    public I16[] IntensityTexture3d => _intensityTexure3d;
    public U32[] IntensityToRgba => _intensityToRgba;

    public RenderParams(string ctFilePathAndName, string intensityToRgbaPathAndName)
    {
        (_intensityTexDims, _intensityTexSpacingMm, _intensityTexure3d) = CtLoad(ctFilePathAndName);
        _intensityToRgba = LookupTableCreate(intensityToRgbaPathAndName);
        //_intensityToRgba = LookupTableCreate(-1000, 1500); // ramp, no file needed
    }

    private (U32[] dims, Vector3 spacingMm, I16[] voxels) CtLoad(string filePathAndName)
    {
        var dims = new U32[3];
        var spacingMm = new Vector3();
        I16[] voxels;

        using (var binaryReader = new BinaryReader(System.IO.File.Open(filePathAndName, System.IO.FileMode.Open)))
        {
            {
                dims[0] = (U32)binaryReader.ReadSingle();
                dims[1] = (U32)binaryReader.ReadSingle();
                dims[2] = (U32)binaryReader.ReadSingle();
                var x = binaryReader.ReadSingle();
                var y = binaryReader.ReadSingle();
                var z = binaryReader.ReadSingle();
                spacingMm = new Vector3(x, y, z);
            }
            voxels = new I16[256 * 256 * 256]; // dims[0] * dims[1] * dims[2]];
            var xA = (dims[0] / 2) - 127;
            var xB = (dims[0] / 2) + 128;
            var yA = (dims[1] / 2) - 127;
            var yB = (dims[1] / 2) + 128;
            var zA = (dims[2] / 2) - 127 + 4; //shift up FOV
            var zB = (dims[2] / 2) + 128 + 4;

            // write the XYZ=LPH SliceStack data into the XYZ=LFP texture (y and z swap position, y also swaps direction)
            for (var z = 0; z < dims[2]; z++)
            {
                for (var y = 0; y < dims[1]; y++)
                {
                    for (var x = 0; x < dims[0]; x++)
                    {
                        var voxel = binaryReader.ReadInt16();
                        if (x >= xA && x <= xB &&
                            y >= yA && y <= yB &&
                            z >= zA && z <= zB)
                        {
                            var coordSum = x - xA + (y - yA) + (z - zA);
                            if (coordSum < 20)
                            {
                                voxel = (I16)(300 * coordSum);
                            }
                            voxels[x - xA + (256 * (255 - (z - zA))) + (256 * 256 * (y - yA))] = voxel;
                        }
                    }
                }
            }

            binaryReader.Close();
            dims[0] = 256;
            dims[1] = 256;
            dims[2] = 256;
        }
        return (dims, spacingMm, voxels);
    }

    private U32[] LookupTableCreate(string intensityToRgbaPathAndName)
    {
        U32[] lut = null!;
        using (var binaryReader = new BinaryReader(System.IO.File.Open(intensityToRgbaPathAndName, System.IO.FileMode.Open)))
        {
            var size = (U32)binaryReader.ReadUInt32();
            if (size == U16.MaxValue + 2)
            {
                size = U16.MaxValue + 1;
            }

            lut = new U32[size];
            for (var i = 0; i < size; ++i)
            {
                var color = binaryReader.ReadUInt32();
                lut[i] = color;
            }
            binaryReader.Close();
        }
        return lut;
    }

    private U32[] LookupTableCreate(int opacityRampBgn, int opacityRampEnd)
    {
        var lut = new U32[U16.MaxValue + 1];
        for (int i = I16.MinValue; i <= opacityRampBgn; i++)
        {
            lut[i - I16.MinValue] = 0;
        }
        F32 rampWidth = opacityRampEnd - opacityRampBgn;
        for (var i = opacityRampBgn + 1; i < opacityRampEnd; i++)
        {
            var frac = (i - opacityRampBgn) / rampWidth;
            var frac2 = frac * frac;
            // lineal opacity increase, color from red to white
            lut[i - I16.MinValue]
                = (((U32)(255 * 1.00)) << 0)  // r
                | (((U32)(255 * frac)) << 8)  // g 
                | (((U32)(255 * frac)) << 16) // b
                | (((U32)(20 * frac2)) << 24);// a
        }
        for (var i = opacityRampEnd; i <= I16.MaxValue; i++)
        {
            lut[i - I16.MinValue] = 0xffFFffFF;
        }
        return lut;
    }
}
