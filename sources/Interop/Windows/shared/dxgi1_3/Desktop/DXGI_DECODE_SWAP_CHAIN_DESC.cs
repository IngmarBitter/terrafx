// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared\dxgi1_3.h in the Windows SDK for Windows 10.0.15063.0
// Original source is Copyright © Microsoft. All rights reserved.

using TerraFX.Utilities;

namespace TerraFX.Interop.Desktop
{
    [Unmanaged]
    public struct DXGI_DECODE_SWAP_CHAIN_DESC
    {
        #region Fields
        [NativeTypeName("UINT")]
        public uint Flags;
        #endregion
    }
}
