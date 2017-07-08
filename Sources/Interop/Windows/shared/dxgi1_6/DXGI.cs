// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared\dxgi1_6.h in the Windows SDK for Windows 10.0.15063.0
// Original source is Copyright © Microsoft. All rights reserved.

using System;

namespace TerraFX.Interop
{
    unsafe public static partial class DXGI
    {
        #region Constants
        #region IID_*
        public static readonly GUID IID_IDXGIAdapter4 = new Guid(0x3C8D99D1, 0x4FBF, 0x4181, 0xA8, 0x2C, 0xAF, 0x66, 0xBF, 0x7B, 0xD2, 0x4E);

        public static readonly GUID IID_IDXGIOutput6 = new Guid(0x068346E8, 0xAAEC, 0x4B84, 0xAD, 0xD7, 0x13, 0x7F, 0x51, 0x3F, 0x77, 0xA1);
        #endregion
        #endregion
    }
}