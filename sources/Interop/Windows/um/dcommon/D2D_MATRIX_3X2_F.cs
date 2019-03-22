// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um\dcommon.h in the Windows SDK for Windows 10.0.15063.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.InteropServices;
using TerraFX.Utilities;

namespace TerraFX.Interop
{
    /// <summary>Represents a 3-by-2 matrix.</summary>
    [StructLayout(LayoutKind.Explicit)]
    [Unmanaged]
    public unsafe struct D2D_MATRIX_3X2_F
    {
        #region Fields
        #region struct
        /// <summary>Horizontal scaling / cosine of rotation</summary>
        [FieldOffset(0)]
        public float m11;

        /// <summary>Vertical shear / sine of rotation</summary>
        [FieldOffset(4)]
        public float m12;

        /// <summary>Horizontal shear / negative sine of rotation</summary>
        [FieldOffset(8)]
        public float m21;

        /// <summary>Vertical scaling / cosine of rotation</summary>
        [FieldOffset(12)]
        public float m22;

        /// <summary>Horizontal shift (always orthogonal regardless of rotation)</summary>
        [FieldOffset(16)]
        public float dx;

        /// <summary>Vertical shift (always orthogonal regardless of rotation)</summary>
        [FieldOffset(20)]
        public float dy;
        #endregion

        #region struct
        [FieldOffset(0)]
        [NativeTypeName("FLOAT")]
        public float _11;

        [FieldOffset(4)]
        [NativeTypeName("FLOAT")]
        public float _12;

        [FieldOffset(8)]
        [NativeTypeName("FLOAT")]
        public float _21;

        [FieldOffset(12)]
        [NativeTypeName("FLOAT")]
        public float _22;

        [FieldOffset(16)]
        [NativeTypeName("FLOAT")]
        public float _31;

        [FieldOffset(20)]
        [NativeTypeName("FLOAT")]
        public float _32;
        #endregion

        [FieldOffset(0)]
        [NativeTypeName("FLOAT[3][2]")]
        public fixed float m[3 * 2];
        #endregion
    }
}
