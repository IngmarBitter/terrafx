// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um\wincodecsdk.h in the Windows SDK for Windows 10.0.15063.0
// Original source is Copyright © Microsoft. All rights reserved.

using TerraFX.Utilities;

namespace TerraFX.Interop
{
    [Unmanaged]
    public unsafe struct WICMetadataHeader
    {
        #region Fields
        public ULARGE_INTEGER Position;

        [NativeTypeName("ULONG")]
        public uint Length;

        [NativeTypeName("BYTE[]")]
        public byte* Header;

        public ULARGE_INTEGER DataOffset;
        #endregion
    }
}
