// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um\d2d1effectauthor.h in the Windows SDK for Windows 10.0.15063.0
// Original source is Copyright © Microsoft. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Security;
using TerraFX.Utilities;
using static TerraFX.Utilities.InteropUtilities;

namespace TerraFX.Interop
{
    /// <summary>The internal context handed to effect authors to create transforms from effects and any other operation tied to context which is not useful to the application facing API.</summary>
    [Guid("3D9F916B-27DC-4AD7-B4F1-64945340F563")]
    [Unmanaged]
    public unsafe struct ID2D1EffectContext
    {
        #region Fields
        public readonly Vtbl* lpVtbl;
        #endregion

        #region IUnknown Delegates
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _QueryInterface(
            [In] ID2D1EffectContext* This,
            [In, NativeTypeName("REFIID")] Guid* riid,
            [Out] void** ppvObject
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("ULONG")]
        public /* static */ delegate uint _AddRef(
            [In] ID2D1EffectContext* This
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("ULONG")]
        public /* static */ delegate uint _Release(
            [In] ID2D1EffectContext* This
        );
        #endregion

        #region Delegates
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        public /* static */ delegate void _GetDpi(
            [In] ID2D1EffectContext* This,
            [Out, NativeTypeName("FLOAT")] float* dpiX,
            [Out, NativeTypeName("FLOAT")] float* dpiY
        );

        /// <summary>Create a new effect, the effect must either be built in or previously registered through ID2D1Factory1::RegisterEffect.</summary>
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _CreateEffect(
            [In] ID2D1EffectContext* This,
            [In, NativeTypeName("REFCLSID")] Guid* effectId,
            [Out] ID2D1Effect** effect
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _GetMaximumSupportedFeatureLevel(
            [In] ID2D1EffectContext* This,
            [In, NativeTypeName("D3D_FEATURE_LEVEL[]")] D3D_FEATURE_LEVEL* featureLevels,
            [In, NativeTypeName("UINT32")] uint featureLevelsCount,
            [Out] D3D_FEATURE_LEVEL* maximumSupportedFeatureLevel
        );

        /// <summary>Create a transform node from the passed in effect.</summary>
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _CreateTransformNodeFromEffect(
            [In] ID2D1EffectContext* This,
            [In] ID2D1Effect* effect,
            [Out] ID2D1TransformNode** transformNode
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _CreateBlendTransform(
            [In] ID2D1EffectContext* This,
            [In, NativeTypeName("UINT32")] uint numInputs,
            [In] D2D1_BLEND_DESCRIPTION* blendDescription,
            [Out] ID2D1BlendTransform** transform
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _CreateBorderTransform(
            [In] ID2D1EffectContext* This,
            [In] D2D1_EXTEND_MODE extendModeX,
            [In] D2D1_EXTEND_MODE extendModeY,
            [Out] ID2D1BorderTransform** transform
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _CreateOffsetTransform(
            [In] ID2D1EffectContext* This,
            [In, NativeTypeName("D2D1_POINT_2L")] POINT offset,
            [Out] ID2D1OffsetTransform** transform
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _CreateBoundsAdjustmentTransform(
            [In] ID2D1EffectContext* This,
            [In, NativeTypeName("D2D1_RECT_L")] RECT* outputRectangle,
            [Out] ID2D1BoundsAdjustmentTransform** transform
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _LoadPixelShader(
            [In] ID2D1EffectContext* This,
            [In, NativeTypeName("REFGUID")] Guid* shaderId,
            [In, NativeTypeName("BYTE[]")] byte* shaderBuffer,
            [In, NativeTypeName("UINT32")] uint shaderBufferCount
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _LoadVertexShader(
            [In] ID2D1EffectContext* This,
            [In, NativeTypeName("REFGUID")] Guid* resourceId,
            [In, NativeTypeName("BYTE[]")] byte* shaderBuffer,
            [In, NativeTypeName("UINT32")] uint shaderBufferCount
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _LoadComputeShader(
            [In] ID2D1EffectContext* This,
            [In, NativeTypeName("REFGUID")] Guid* resourceId,
            [In, NativeTypeName("BYTE[]")]  byte* shaderBuffer,
            [In, NativeTypeName("UINT32")] uint shaderBufferCount
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("BOOL")]
        public /* static */ delegate int _IsShaderLoaded(
            [In] ID2D1EffectContext* This,
            [In, NativeTypeName("REFGUID")] Guid* shaderId
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _CreateResourceTexture(
            [In] ID2D1EffectContext* This,
            [In, Optional, NativeTypeName("GUID")] Guid* resourceId,
            [In] D2D1_RESOURCE_TEXTURE_PROPERTIES* resourceTextureProperties,
            [In, Optional, NativeTypeName("BYTE[]")] byte* data,
            [In, Optional, NativeTypeName("UINT32[]")] uint* strides,
            [In, NativeTypeName("UINT32")] uint dataSize,
            [Out] ID2D1ResourceTexture** resourceTexture
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _FindResourceTexture(
            [In] ID2D1EffectContext* This,
            [In, NativeTypeName("GUID")] Guid* resourceId,
            [Out] ID2D1ResourceTexture** resourceTexture
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _CreateVertexBuffer(
            [In] ID2D1EffectContext* This,
            [In] D2D1_VERTEX_BUFFER_PROPERTIES* vertexBufferProperties,
            [In, Optional, NativeTypeName("GUID")] Guid* resourceId,
            [In, Optional] D2D1_CUSTOM_VERTEX_BUFFER_PROPERTIES* customVertexBufferProperties,
            [Out] ID2D1VertexBuffer** buffer
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _FindVertexBuffer(
            [In] ID2D1EffectContext* This,
            [In, NativeTypeName("GUID")] Guid* resourceId,
            [Out] ID2D1VertexBuffer** buffer
        );

        /// <summary>Creates a color context from a color space.  If the space is Custom, the context is initialized from the profile/profileSize arguments.  Otherwise the context is initialized with the profile bytes associated with the space and profile/profileSize are ignored.</summary>
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _CreateColorContext(
            [In] ID2D1EffectContext* This,
            [In] D2D1_COLOR_SPACE space,
            [In, Optional, NativeTypeName("BYTE[]")] byte* profile,
            [In, NativeTypeName("UINT32")] uint profileSize,
            [Out] ID2D1ColorContext** colorContext
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _CreateColorContextFromFilename(
            [In] ID2D1EffectContext* This,
            [In, NativeTypeName("PCWSTR")] char* filename,
            [Out] ID2D1ColorContext** colorContext
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _CreateColorContextFromWicColorContext(
            [In] ID2D1EffectContext* This,
            [In] IWICColorContext* wicColorContext,
            [Out] ID2D1ColorContext** colorContext
        );

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("HRESULT")]
        public /* static */ delegate int _CheckFeatureSupport(
            [In] ID2D1EffectContext* This,
            [In] D2D1_FEATURE feature,
            [Out] void* featureSupportData,
            [In, NativeTypeName("UINT32")] uint featureSupportDataSize
        );

        /// <summary>Indicates whether the buffer precision is supported by D2D.</summary>
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.ThisCall, BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = false, ThrowOnUnmappableChar = false)]
        [return: NativeTypeName("BOOL")]
        public /* static */ delegate int _IsBufferPrecisionSupported(
            [In] ID2D1EffectContext* This,
            [In] D2D1_BUFFER_PRECISION bufferPrecision
        );
        #endregion

        #region IUnknown Methods
        [return: NativeTypeName("HRESULT")]
        public int QueryInterface(
            [In, NativeTypeName("REFIID")] Guid* riid,
            [Out] void** ppvObject
        )
        {
            fixed (ID2D1EffectContext* This = &this)
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
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_AddRef>(lpVtbl->AddRef)(
                    This
                );
            }
        }

        [return: NativeTypeName("ULONG")]
        public uint Release()
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_Release>(lpVtbl->Release)(
                    This
                );
            }
        }
        #endregion

        #region Methods
        public void GetDpi(
            [Out, NativeTypeName("FLOAT")] float* dpiX,
            [Out, NativeTypeName("FLOAT")] float* dpiY
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                MarshalFunction<_GetDpi>(lpVtbl->GetDpi)(
                    This,
                    dpiX,
                    dpiY
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int CreateEffect(
            [In, NativeTypeName("REFCLSID")] Guid* effectId,
            [Out] ID2D1Effect** effect
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_CreateEffect>(lpVtbl->CreateEffect)(
                    This,
                    effectId,
                    effect
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int GetMaximumSupportedFeatureLevel(
            [In, NativeTypeName("D3D_FEATURE_LEVEL[]")] D3D_FEATURE_LEVEL* featureLevels,
            [In, NativeTypeName("UINT32")] uint featureLevelsCount,
            [Out] D3D_FEATURE_LEVEL* maximumSupportedFeatureLevel
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_GetMaximumSupportedFeatureLevel>(lpVtbl->GetMaximumSupportedFeatureLevel)(
                    This,
                    featureLevels,
                    featureLevelsCount,
                    maximumSupportedFeatureLevel
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int CreateTransformNodeFromEffect(
            [In] ID2D1Effect* effect,
            [Out] ID2D1TransformNode** transformNode
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_CreateTransformNodeFromEffect>(lpVtbl->CreateTransformNodeFromEffect)(
                    This,
                    effect,
                    transformNode
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int CreateBlendTransform(
            [In, NativeTypeName("UINT32")] uint numInputs,
            [In] D2D1_BLEND_DESCRIPTION* blendDescription,
            [Out] ID2D1BlendTransform** transform
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_CreateBlendTransform>(lpVtbl->CreateBlendTransform)(
                    This,
                    numInputs,
                    blendDescription,
                    transform
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int CreateBorderTransform(
            [In] D2D1_EXTEND_MODE extendModeX,
            [In] D2D1_EXTEND_MODE extendModeY,
            [Out] ID2D1BorderTransform** transform
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_CreateBorderTransform>(lpVtbl->CreateBorderTransform)(
                    This,
                    extendModeX,
                    extendModeY,
                    transform
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int CreateOffsetTransform(
            [In, NativeTypeName("D2D1_POINT_2L")] POINT offset,
            [Out] ID2D1OffsetTransform** transform
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_CreateOffsetTransform>(lpVtbl->CreateOffsetTransform)(
                    This,
                    offset,
                    transform
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int CreateBoundsAdjustmentTransform(
            [In, NativeTypeName("D2D1_RECT_L")] RECT* outputRectangle,
            [Out] ID2D1BoundsAdjustmentTransform** transform
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_CreateBoundsAdjustmentTransform>(lpVtbl->CreateBoundsAdjustmentTransform)(
                    This,
                    outputRectangle,
                    transform
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int LoadPixelShader(
            [In, NativeTypeName("REFGUID")] Guid* shaderId,
            [In, NativeTypeName("BYTE[]")] byte* shaderBuffer,
            [In, NativeTypeName("UINT32")] uint shaderBufferCount
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_LoadPixelShader>(lpVtbl->LoadPixelShader)(
                    This,
                    shaderId,
                    shaderBuffer,
                    shaderBufferCount
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int LoadVertexShader(
            [In, NativeTypeName("REFGUID")] Guid* resourceId,
            [In, NativeTypeName("BYTE[]")] byte* shaderBuffer,
            [In, NativeTypeName("UINT32")] uint shaderBufferCount
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_LoadVertexShader>(lpVtbl->LoadVertexShader)(
                    This,
                    resourceId,
                    shaderBuffer,
                    shaderBufferCount
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int LoadComputeShader(
            [In, NativeTypeName("REFGUID")] Guid* resourceId,
            [In, NativeTypeName("BYTE[]")]  byte* shaderBuffer,
            [In, NativeTypeName("UINT32")] uint shaderBufferCount
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_LoadComputeShader>(lpVtbl->LoadComputeShader)(
                    This,
                    resourceId,
                    shaderBuffer,
                    shaderBufferCount
                );
            }
        }

        [return: NativeTypeName("BOOL")]
        public int IsShaderLoaded(
            [In, NativeTypeName("REFGUID")] Guid* shaderId
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_IsShaderLoaded>(lpVtbl->IsShaderLoaded)(
                    This,
                    shaderId
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int CreateResourceTexture(
            [In, Optional, NativeTypeName("GUID")] Guid* resourceId,
            [In] D2D1_RESOURCE_TEXTURE_PROPERTIES* resourceTextureProperties,
            [In, Optional, NativeTypeName("BYTE[]")] byte* data,
            [In, Optional, NativeTypeName("UINT32[]")] uint* strides,
            [In, NativeTypeName("UINT32")] uint dataSize,
            [Out] ID2D1ResourceTexture** resourceTexture
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_CreateResourceTexture>(lpVtbl->CreateResourceTexture)(
                    This,
                    resourceId,
                    resourceTextureProperties,
                    data,
                    strides,
                    dataSize,
                    resourceTexture
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int FindResourceTexture(
            [In, NativeTypeName("GUID")] Guid* resourceId,
            [Out] ID2D1ResourceTexture** resourceTexture
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_FindResourceTexture>(lpVtbl->FindResourceTexture)(
                    This,
                    resourceId,
                    resourceTexture
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int CreateVertexBuffer(
            [In] D2D1_VERTEX_BUFFER_PROPERTIES* vertexBufferProperties,
            [In, Optional, NativeTypeName("GUID")] Guid* resourceId,
            [In, Optional] D2D1_CUSTOM_VERTEX_BUFFER_PROPERTIES* customVertexBufferProperties,
            [Out] ID2D1VertexBuffer** buffer
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_CreateVertexBuffer>(lpVtbl->CreateVertexBuffer)(
                    This,
                    vertexBufferProperties,
                    resourceId,
                    customVertexBufferProperties,
                    buffer
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int FindVertexBuffer(
            [In, NativeTypeName("GUID")] Guid* resourceId,
            [Out] ID2D1VertexBuffer** buffer
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_FindVertexBuffer>(lpVtbl->FindVertexBuffer)(
                    This,
                    resourceId,
                    buffer
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int CreateColorContext(
            [In] D2D1_COLOR_SPACE space,
            [In, Optional, NativeTypeName("BYTE[]")] byte* profile,
            [In, NativeTypeName("UINT32")] uint profileSize,
            [Out] ID2D1ColorContext** colorContext
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_CreateColorContext>(lpVtbl->CreateColorContext)(
                    This,
                    space,
                    profile,
                    profileSize,
                    colorContext
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int CreateColorContextFromFilename(
            [In, NativeTypeName("PCWSTR")] char* filename,
            [Out] ID2D1ColorContext** colorContext
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_CreateColorContextFromFilename>(lpVtbl->CreateColorContextFromFilename)(
                    This,
                    filename,
                    colorContext
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int CreateColorContextFromWicColorContext(
            [In] IWICColorContext* wicColorContext,
            [Out] ID2D1ColorContext** colorContext
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_CreateColorContextFromWicColorContext>(lpVtbl->CreateColorContextFromWicColorContext)(
                    This,
                    wicColorContext,
                    colorContext
                );
            }
        }

        [return: NativeTypeName("HRESULT")]
        public int CheckFeatureSupport(
            [In] D2D1_FEATURE feature,
            [Out] void* featureSupportData,
            [In, NativeTypeName("UINT32")] uint featureSupportDataSize
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_CheckFeatureSupport>(lpVtbl->CheckFeatureSupport)(
                    This,
                    feature,
                    featureSupportData,
                    featureSupportDataSize
                );
            }
        }

        [return: NativeTypeName("BOOL")]
        public int IsBufferPrecisionSupported(
            [In] D2D1_BUFFER_PRECISION bufferPrecision
        )
        {
            fixed (ID2D1EffectContext* This = &this)
            {
                return MarshalFunction<_IsBufferPrecisionSupported>(lpVtbl->IsBufferPrecisionSupported)(
                    This,
                    bufferPrecision
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
            public IntPtr GetDpi;

            public IntPtr CreateEffect;

            public IntPtr GetMaximumSupportedFeatureLevel;

            public IntPtr CreateTransformNodeFromEffect;

            public IntPtr CreateBlendTransform;

            public IntPtr CreateBorderTransform;

            public IntPtr CreateOffsetTransform;

            public IntPtr CreateBoundsAdjustmentTransform;

            public IntPtr LoadPixelShader;

            public IntPtr LoadVertexShader;

            public IntPtr LoadComputeShader;

            public IntPtr IsShaderLoaded;

            public IntPtr CreateResourceTexture;

            public IntPtr FindResourceTexture;

            public IntPtr CreateVertexBuffer;

            public IntPtr FindVertexBuffer;

            public IntPtr CreateColorContext;

            public IntPtr CreateColorContextFromFilename;

            public IntPtr CreateColorContextFromWicColorContext;

            public IntPtr CheckFeatureSupport;

            public IntPtr IsBufferPrecisionSupported;
            #endregion
        }
        #endregion
    }
}
