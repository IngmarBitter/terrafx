// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.	

using System.Runtime.CompilerServices;
using static TerraFX.Interop.Windows;
using static TerraFX.Utilities.ExceptionUtilities;

namespace TerraFX.Utilities
{
    internal static unsafe partial class Win32Utilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowExternalExceptionIfFalse(int value, string methodName)
        {
            if (value == FALSE)
            {
                ThrowForLastError(methodName);
            }
        }
    }
}
