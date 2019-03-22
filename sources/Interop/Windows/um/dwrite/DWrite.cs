// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um\dwrite.h in the Windows SDK for Windows 10.0.15063.0
// Original source is Copyright © Microsoft. All rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Security;
using TerraFX.Utilities;

namespace TerraFX.Interop
{
    public static unsafe partial class DWrite
    {
        private const string DllName = nameof(DWrite);

        #region Constants
        /// <summary>Maximum alpha value in a texture returned by IDWriteGlyphRunAnalysis::CreateAlphaTexture.</summary>
        public const int DWRITE_ALPHA_MAX = 255;
        #endregion

        #region FACILITY_* Constants
        public const int FACILITY_DWRITE = 0x898;
        #endregion

        #region DWRITE_ERR_* Constants
        public const int DWRITE_ERR_BASE = 0x5000;
        #endregion

        #region IID_* Constants
        public static readonly Guid IID_IDWriteFontFileLoader = new Guid(0x727CAD4E, 0xD6AF, 0x4C9E, 0x8A, 0x08, 0xD6, 0x95, 0xB1, 0x1C, 0xAA, 0x49);

        public static readonly Guid IID_IDWriteLocalFontFileLoader = new Guid(0xB2D9F3EC, 0xC9FE, 0x4A11, 0xA2, 0xEC, 0xD8, 0x62, 0x08, 0xF7, 0xC0, 0xA2);

        public static readonly Guid IID_IDWriteFontFileStream = new Guid(0x6D4865FE, 0x0AB8, 0x4D91, 0x8F, 0x62, 0x5D, 0xD6, 0xBE, 0x34, 0xA3, 0xE0);

        public static readonly Guid IID_IDWriteFontFile = new Guid(0x739D886A, 0xCEF5, 0x47DC, 0x87, 0x69, 0x1A, 0x8B, 0x41, 0xBE, 0xBB, 0xB0);

        public static readonly Guid IID_IDWriteRenderingParams = new Guid(0x2F0DA53A, 0x2ADD, 0x47CD, 0x82, 0xEE, 0xD9, 0xEC, 0x34, 0x68, 0x8E, 0x75);

        public static readonly Guid IID_IDWriteFontFace = new Guid(0x5F49804D, 0x7024, 0x4D43, 0xBF, 0xA9, 0xD2, 0x59, 0x84, 0xF5, 0x38, 0x49);

        public static readonly Guid IID_IDWriteFontCollectionLoader = new Guid(0xCCA920E4, 0x52F0, 0x492B, 0xBF, 0xA8, 0x29, 0xC7, 0x2E, 0xE0, 0xA4, 0x68);

        public static readonly Guid IID_IDWriteFontFileEnumerator = new Guid(0x72755049, 0x5FF7, 0x435D, 0x83, 0x48, 0x4B, 0xE9, 0x7C, 0xFA, 0x6C, 0x7C);

        public static readonly Guid IID_IDWriteLocalizedStrings = new Guid(0x08256209, 0x099A, 0x4B34, 0xB8, 0x6D, 0xC2, 0x2B, 0x11, 0x0E, 0x77, 0x71);

        public static readonly Guid IID_IDWriteFontCollection = new Guid(0xA84CEE02, 0x3EEA, 0x4EEE, 0xA8, 0x27, 0x87, 0xC1, 0xA0, 0x2A, 0x0F, 0xCC);

        public static readonly Guid IID_IDWriteFontList = new Guid(0x1A0D8438, 0x1D97, 0x4EC1, 0xAE, 0xF9, 0xA2, 0xFB, 0x86, 0xED, 0x6A, 0xCB);

        public static readonly Guid IID_IDWriteFontFamily = new Guid(0xDA20D8EF, 0x812A, 0x4C43, 0x98, 0x02, 0x62, 0xEC, 0x4A, 0xBD, 0x7A, 0xDD);

        public static readonly Guid IID_IDWriteFont = new Guid(0xACD16696, 0x8C14, 0x4F5D, 0x87, 0x7E, 0xFE, 0x3F, 0xC1, 0xD3, 0x27, 0x37);

        public static readonly Guid IID_IDWriteTextFormat = new Guid(0x9C906818, 0x31D7, 0x4FD3, 0xA1, 0x51, 0x7C, 0x5E, 0x22, 0x5D, 0xB5, 0x5A);

        public static readonly Guid IID_IDWriteTypography = new Guid(0x55F1112B, 0x1DC2, 0x4B3C, 0x95, 0x41, 0xF4, 0x68, 0x94, 0xED, 0x85, 0xB6);

        public static readonly Guid IID_IDWriteNumberSubstitution = new Guid(0x14885CC9, 0xBAB0, 0x4F90, 0xB6, 0xED, 0x5C, 0x36, 0x6A, 0x2C, 0xD0, 0x3D);

        public static readonly Guid IID_IDWriteTextAnalysisSource = new Guid(0x688E1A58, 0x5094, 0x47C8, 0xAD, 0xC8, 0xFB, 0xCE, 0xA6, 0x0A, 0xE9, 0x2B);

        public static readonly Guid IID_IDWriteTextAnalysisSink = new Guid(0x5810CD44, 0x0CA0, 0x4701, 0xB3, 0xFA, 0xBE, 0xC5, 0x18, 0x2A, 0xE4, 0xF6);

        public static readonly Guid IID_IDWriteTextAnalyzer = new Guid(0xB7E6163E, 0x7F46, 0x43B4, 0x84, 0xB3, 0xE4, 0xE6, 0x24, 0x9C, 0x36, 0x5D);

        public static readonly Guid IID_IDWriteInlineObject = new Guid(0x8339FDE3, 0x106F, 0x47AB, 0x83, 0x73, 0x1C, 0x62, 0x95, 0xEB, 0x10, 0xB3);

        public static readonly Guid IID_IDWritePixelSnapping = new Guid(0xEAF3A2DA, 0xECF4, 0x4D24, 0xB6, 0x44, 0xB3, 0x4F, 0x68, 0x42, 0x02, 0x4B);

        public static readonly Guid IID_IDWriteTextRenderer = new Guid(0xEF8A8135, 0x5CC6, 0x45FE, 0x88, 0x25, 0xC5, 0xA0, 0x72, 0x4E, 0xB8, 0x19);

        public static readonly Guid IID_IDWriteTextLayout = new Guid(0x53737037, 0x6D14, 0x410B, 0x9B, 0xFE, 0x0B, 0x18, 0x2B, 0xB7, 0x09, 0x61);

        public static readonly Guid IID_IDWriteBitmapRenderTarget = new Guid(0x5E5A32A3, 0x8DFF, 0x4773, 0x9F, 0xF6, 0x06, 0x96, 0xEA, 0xB7, 0x72, 0x67);

        public static readonly Guid IID_IDWriteGdiInterop = new Guid(0x1EDD9491, 0x9853, 0x4299, 0x89, 0x8F, 0x64, 0x32, 0x98, 0x3B, 0x6F, 0x3A);

        public static readonly Guid IID_IDWriteGlyphRunAnalysis = new Guid(0x7D97DBF7, 0xE085, 0x42D4, 0x81, 0xE3, 0x6A, 0x88, 0x3B, 0xDE, 0xD1, 0x18);

        public static readonly Guid IID_IDWriteFactory = new Guid(0xB859EE5A, 0xD838, 0x4B5B, 0xA2, 0xE8, 0x1A, 0xDC, 0x7D, 0x93, 0xDB, 0x48);
        #endregion

        #region External Methods
        /// <summary>Creates a DirectWrite factory object that is used for subsequent creation of individual DirectWrite objects.</summary>
        /// <param name="factoryType">Identifies whether the factory object will be shared or isolated.</param>
        /// <param name="iid">Identifies the DirectWrite factory interface, such as __uuidof(IDWriteFactory).</param>
        /// <param name="factory">Receives the DirectWrite factory object.</param>
        /// <returns>Standard HRESULT error code.</returns>
        /// <remarks>Obtains DirectWrite factory object that is used for subsequent creation of individual DirectWrite classes. DirectWrite factory contains internal state such as font loader registration and cached font data. In most cases it is recommended to use the shared factory object, because it allows multiple components that use DirectWrite to share internal DirectWrite state and reduce memory usage. However, there are cases when it is desirable to reduce the impact of a component, such as a plug-in from an untrusted source, on the rest of the process by sandboxing and isolating it from the rest of the process components. In such cases, it is recommended to use an isolated factory for the sandboxed component.</remarks>
        [DllImport(DllName, BestFitMapping = false, CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode, EntryPoint = "DWriteCreateFactory", ExactSpelling = true, PreserveSig = true, SetLastError = false, ThrowOnUnmappableChar = false)]
        [SuppressUnmanagedCodeSecurity]
        [return: NativeTypeName("HRESULT")]
        public static extern int DWriteCreateFactory(
            [In] DWRITE_FACTORY_TYPE factoryType,
            [In, NativeTypeName("REFIID")] Guid* iid,
            [Out] IUnknown** factory
        );
        #endregion
    }
}
