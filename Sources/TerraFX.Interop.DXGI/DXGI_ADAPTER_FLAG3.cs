// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared\dxgi1_6.h in the Windows SDK for Windows 10.0.15063.0
// Original source is Copyright © Microsoft. All rights reserved.

using System;

namespace TerraFX.Interop.DXGI
{
    [Flags]
    public enum DXGI_ADAPTER_FLAG3 : uint
    {
        NONE = 0,

        REMOTE = 1,

        SOFTWARE = 2,

        ACG_COMPATIBLE = 4,

        FORCE_DWORD = 0xFFFFFFFF
    }
}