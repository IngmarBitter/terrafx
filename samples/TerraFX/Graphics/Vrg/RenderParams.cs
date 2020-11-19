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
    private readonly U32[] _texDims;
    private readonly Vector3 _texSpacingMm;
    private readonly I16[] _texels;

    public U32[] TexDims => _texDims;
    public U32 TexSize => _texDims[0] * _texDims[1] * _texDims[2];
    public Vector3 TexSpacingMm => _texSpacingMm;
    public I16[] Texels => _texels;

    public RenderParams(string ctFilePathAndName)
    {
        (_texDims, _texSpacingMm, _texels) = CtLoad(ctFilePathAndName);
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
}
