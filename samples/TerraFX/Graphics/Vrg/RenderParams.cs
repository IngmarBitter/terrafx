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
using System.Linq;
using TerraFX.Interop;

public class RenderParams
{
    private readonly string _vrgFolder = "C:/projects/ClaroNav/Vrg/";
    private readonly U32[] _intensityTexDims;
    private readonly Vector3 _intensityTexSpacingMm;
    private readonly U8[] _intensityTexure3d;
    private readonly U32[] _intensityToRgba;

    public U32[] IntensityTexDims => _intensityTexDims;
    public Vector3 IntensityTexSpacingMm => _intensityTexSpacingMm;
    public U8[] IntensityTexture3d => _intensityTexure3d;
    public U32[] IntensityToRgba => _intensityToRgba;

    public RenderParams(string ctFilePathAndName, string intensityToRgbaPathAndName)
    {
        (_intensityTexDims, _intensityTexSpacingMm, _intensityTexure3d) = CtLoadFull(ctFilePathAndName);
        _intensityToRgba = LookupTableCreate(intensityToRgbaPathAndName);
        //_intensityToRgba = LookupTableCreate(-1000, 1500); // ramp, no file needed
    }

    private (U32[] dims, Vector3 spacingMm, U8[] voxels) CtLoad256x3(string filePathAndName)
    {
        if (!File.Exists(filePathAndName))
        {
            filePathAndName = _vrgFolder + filePathAndName.Split('/').Last();
        }
        var dims = new U32[3];
        var spacingMm = new Vector3();
        U8[] voxels;

        using (var binaryReader = new BinaryReader(System.IO.File.Open(filePathAndName, System.IO.FileMode.Open)))
        {
            // LPH oriented data on disk
            {
                dims[0] = (U32)binaryReader.ReadSingle();
                dims[1] = (U32)binaryReader.ReadSingle();
                dims[2] = (U32)binaryReader.ReadSingle();
                var x = binaryReader.ReadSingle();
                var y = binaryReader.ReadSingle();
                var z = binaryReader.ReadSingle();
                spacingMm = new Vector3(x, y, z);
            }
            // texture (w)idth, (h)eight, (d)epth
            var w = 256u;
            var h = 256u;
            var d = 256u;
            voxels = new U8[w * h * d];
            var dh = w;
            var dd = w * h;
            var hMax = h - 1;
            var xA = (dims[0] / 2) - ((w / 2) - 1);
            var xB = (dims[0] / 2) + (w / 2);
            var yA = (dims[1] / 3) - ((d / 2) - 1);
            var yB = (dims[1] / 3) + (d / 2);
            var zA = (dims[2] / 3) - ((h / 2) - 1);
            var zB = (dims[2] / 3) + (h / 2);

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
                            voxels[x - xA + (dh * (hMax - (z - zA))) + (dd * (y - yA))] = VoxelI16toU8(voxel);
                        }
                    }
                }
            }
            binaryReader.Close();

            // swap the dims for y and z to reflect the new coordinate system meaning
            dims[0] = w;
            dims[1] = h;
            dims[2] = d;
        }
        return (dims, spacingMm, voxels);
    }

    private (U32[] dims, Vector3 spacingMm, U8[] voxels) CtLoadFull(string filePathAndName)
    {
        if (!File.Exists(filePathAndName))
        {
            filePathAndName = _vrgFolder + filePathAndName.Split('/').Last();
        }
        var dims = new U32[3];
        var spacingMm = new Vector3();
        U8[] voxels;

        using (var binaryReader = new BinaryReader(System.IO.File.Open(filePathAndName, System.IO.FileMode.Open)))
        {
            // LPH oriented data on disk
            {
                dims[0] = (U32)binaryReader.ReadSingle();
                dims[1] = (U32)binaryReader.ReadSingle();
                dims[2] = (U32)binaryReader.ReadSingle();
                var x = binaryReader.ReadSingle();
                var y = binaryReader.ReadSingle();
                var z = binaryReader.ReadSingle();
                spacingMm = new Vector3(x, y, z);
            }
            // texture (w)idth, (h)eight, (d)epth
            var w = 256u;
            var h = 280u;
            var d = 256u;
            voxels = new U8[w * h * d];
            var dh = w;
            var dd = w * h;
            var hMax = h - 1;
            var xA = (dims[0] / 2) - ((w / 2) - 1);
            var xB = (dims[0] / 2) + (w / 2);
            var yA = (dims[1] / 3) - ((d / 2) - 1);
            var yB = (dims[1] / 3) + (d / 2);
            var zA = (dims[2] / 3) - ((h / 2) - 1);
            var zB = (dims[2] / 3) + (h / 2);

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
                            voxels[x - xA + (dh * (hMax - (z - zA))) + (dd * (y - yA))] = VoxelI16toU8(voxel);
                        }
                    }
                }
            }
            binaryReader.Close();

            // swap the dims for y and z to reflect the new coordinate system meaning
            dims[0] = w;
            dims[1] = h;
            dims[2] = d;
        }
        return (dims, spacingMm, voxels);
    }

    private static U8 VoxelI16toU8(I16 voxel)
    {
        var value01 = voxel + 1000f;
        if (value01 < 0)
        {
            value01 = 0;
        }

        value01 /= 3000;
        value01 = Sigmoid0To1WithCenterAndWidth(value01, 0.5f, 0.5f);
        var texel = (U8)(255 * value01);
        return texel;
    }

    private static F32 Sigmoid0To1WithCenterAndWidth(F32 x0to1, F32 center0to1, F32 width0to1)
    {
        var mappedValue = 1.0f / (1 + MathF.Pow(1.5f / width0to1, -10 * (x0to1 - center0to1)));
        return mappedValue;
    }

    private U32[] LookupTableCreate(string intensityToRgbaPathAndName)
    {
        if (!File.Exists(intensityToRgbaPathAndName))
        {
            intensityToRgbaPathAndName = _vrgFolder + intensityToRgbaPathAndName.Split('/').Last();
        }
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

        var lut256 = new U32[256];

        for (uint n = 0; n < lut.Length; n++)
        {
            var texel = lut[n];
            lut256[VoxelI16toU8((I16)((I64)n + I16.MinValue))] = texel;
            //pTextureData[n]
            //    = 0 << 0       // r
            //    | texel << 8   // g
            //    | texel << 16  // b
            //    | texel << 24; // a
        }

        return lut256;
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
