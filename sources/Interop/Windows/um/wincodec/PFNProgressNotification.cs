// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um\wincodec.h in the Windows SDK for Windows 10.0.15063.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.InteropServices;
using System.Security;
using TerraFX.Utilities;

namespace TerraFX.Interop
{
    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
    [return: NativeTypeName("HRESULT")]
    public /* static */ unsafe delegate int PFNProgressNotification(
        [In, NativeTypeName("LPVOID")] void* pvData,
        [In, NativeTypeName("ULONG")] uint uFrameNum,
        [In] WICProgressOperation operation,
        [In] double dblProgress
    );
}
