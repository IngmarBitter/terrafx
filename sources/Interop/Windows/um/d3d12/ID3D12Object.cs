// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um\d3d12.h in the Windows SDK for Windows 10.0.15063.0
// Original source is Copyright © Microsoft. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Security;
using TerraFX.Utilities;
using static TerraFX.Utilities.InteropUtilities;

namespace TerraFX.Interop
{
    [Guid("C4FEC28F-7966-4E95-9F94-F431CB56C3B8")]
    [Unmanaged]
    public unsafe struct ID3D12Object
    {
        #region Fields
        public readonly Vtbl* lpVtbl;
        #endregion

        #region IUnknown Delegates
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _QueryInterface(
            [In] ID3D12Object* This,
            [In, NativeTypeName("REFIID")] Guid* riid,
            [Out] void** ppvObject
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("ULONG")]
        public /* static */ delegate uint _AddRef(
            [In] ID3D12Object* This
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("ULONG")]
        public /* static */ delegate uint _Release(
            [In] ID3D12Object* This
        );
        #endregion

        #region Delegates
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _GetPrivateData(
            [In] ID3D12Object* This,
            [In, NativeTypeName("REFGUID")] Guid* guid,
            [In, Out, NativeTypeName("UINT")] uint* pDataSize,
            [Out] void* pData = null
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _SetPrivateData(
            [In] ID3D12Object* This,
            [In, NativeTypeName("REFGUID")] Guid* guid,
            [In, NativeTypeName("UINT")] uint DataSize,
            [In] void* pData = null
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _SetPrivateDataInterface(
            [In] ID3D12Object* This,
            [In, NativeTypeName("REFGUID")] Guid* guid,
            [In] IUnknown* pData = null
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _SetName(
            [In] ID3D12Object* This,
            [In, NativeTypeName("LPCWSTR")] char* Name
        );
        #endregion

        #region IUnknown Methods
        [return: NativeTypeName("HRESULT")]
        public int QueryInterface(
            [In, NativeTypeName("REFIID")] Guid* riid,
            [Out] void** ppvObject
        )
        {
            fixed (ID3D12Object* This = &this)
            {
                return MarshalFunction<_QueryInterface>(lpVtbl->QueryInterface)(
                    This,
                    riid,
                    ppvObject
                );
            }
        }

        [return: NativeTypeName("ULONG")]
        public uint AddRef()
        {
            fixed (ID3D12Object* This = &this)
            {
                return MarshalFunction<_AddRef>(lpVtbl->AddRef)(
                    This
                );
            }
        }

        [return: NativeTypeName("ULONG")]
        public uint Release()
        {
            fixed (ID3D12Object* This = &this)
            {
                return MarshalFunction<_Release>(lpVtbl->Release)(
                    This
                );
            }
        }
        #endregion

        #region Methods
        [return: NativeTypeName("HRESULT")]
        public int GetPrivateData(
            [In, NativeTypeName("REFGUID")] Guid* guid,
            [In, Out, NativeTypeName("UINT")] uint* pDataSize,
            [Out] void* pData = null
        )
        {
            fixed (ID3D12Object* This = &this)
            {
                return MarshalFunction<_GetPrivateData>(lpVtbl->GetPrivateData)(
                    This,
                    guid,
                    pDataSize,
                    pData
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int SetPrivateData(
            [In, NativeTypeName("REFGUID")] Guid* guid,
            [In, NativeTypeName("UINT")] uint DataSize,
            [In] void* pData = null
        )
        {
            fixed (ID3D12Object* This = &this)
            {
                return MarshalFunction<_SetPrivateData>(lpVtbl->SetPrivateData)(
                    This,
                    guid,
                    DataSize,
                    pData
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int SetPrivateDataInterface(
            [In, NativeTypeName("REFGUID")] Guid* guid,
            [In] IUnknown* pData = null
        )
        {
            fixed (ID3D12Object* This = &this)
            {
                return MarshalFunction<_SetPrivateDataInterface>(lpVtbl->SetPrivateDataInterface)(
                    This,
                    guid,
                    pData
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int SetName(
            [In, NativeTypeName("LPCWSTR")] char* Name
        )
        {
            fixed (ID3D12Object* This = &this)
            {
                return MarshalFunction<_SetName>(lpVtbl->SetName)(
                    This,
                    Name
                );
            }
        }
        #endregion

        #region Structs
        [Unmanaged]
        public struct Vtbl
        {
            #region IUnknown Fields
            public IntPtr QueryInterface;

            public IntPtr AddRef;

            public IntPtr Release;
            #endregion

            #region Fields
            public IntPtr GetPrivateData;

            public IntPtr SetPrivateData;

            public IntPtr SetPrivateDataInterface;

            public IntPtr SetName;
            #endregion
        }
        #endregion
    }
}
