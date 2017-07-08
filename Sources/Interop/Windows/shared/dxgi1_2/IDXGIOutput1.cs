// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared\dxgi1_2.h in the Windows SDK for Windows 10.0.15063.0
// Original source is Copyright © Microsoft. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Security;
using TerraFX.Interop.Desktop;

namespace TerraFX.Interop
{
    [Guid("00CDDEA8-939B-4B83-A340-A685226666CC")]
    unsafe public /* blittable */ struct IDXGIOutput1
    {
        #region Fields
        public readonly void* /* Vtbl* */ lpVtbl;
        #endregion

        #region Delegates
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        public /* static */ delegate HRESULT GetDisplayModeList1(
            [In] IDXGIOutput1* This,
            [In] DXGI_FORMAT EnumFormat,
            [In] UINT Flags,
            [In, Out] UINT* pNumModes,
            [Out, Optional] DXGI_MODE_DESC1* pDesc
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        public /* static */ delegate HRESULT FindClosestMatchingMode1(
            [In] IDXGIOutput1* This,
            [In] /* readonly */ DXGI_MODE_DESC1* pModeToMatch,
            [Out] DXGI_MODE_DESC1* pClosestMatch,
            [In, Optional] IUnknown* pConcernedDevice
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        public /* static */ delegate HRESULT GetDisplaySurfaceData1(
            [In] IDXGIOutput1* This,
            [In] IDXGIResource* pDestination
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        public /* static */ delegate HRESULT DuplicateOutput(
            [In] IDXGIOutput1* This,
            [In] IUnknown* pDevice,
            [Out] IDXGIOutputDuplication** ppOutputDuplication
        );
        #endregion

        #region Structs
        public /* blittable */ struct Vtbl
        {
            #region Fields
            public IDXGIOutput.Vtbl BaseVtbl;

            public GetDisplayModeList1 GetDisplayModeList1;

            public FindClosestMatchingMode1 FindClosestMatchingMode1;

            public GetDisplaySurfaceData1 GetDisplaySurfaceData1;

            public DuplicateOutput DuplicateOutput;
            #endregion
        }
        #endregion
    }
}