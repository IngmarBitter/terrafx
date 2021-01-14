// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static TerraFX.Utilities.UnsafeUtilities;

namespace TerraFX.Utilities
{
    /// <summary>Provides a set of methods to supplement or replace <see cref="Marshal" />.</summary>
    public static unsafe class MarshalUtilities
    {
        /// <summary>Gets a string for a given <see cref="ReadOnlySpan{SByte}" />.</summary>
        /// <param name="source">The <see cref="ReadOnlySpan{SByte}" /> for which to create the string.</param>
        /// <returns>A string created from <paramref name="source" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? AsString(this ReadOnlySpan<sbyte> source)
        {
            string? result = null;

            if (source.AsPointer() != null)
            {
                var bytes = MemoryMarshal.Cast<sbyte, byte>(source);
                result = Encoding.UTF8.GetString(bytes);
            }

            return result;
        }

        /// <summary>Gets a string for a given <see cref="ReadOnlySpan{UInt16}" />.</summary>
        /// <param name="source">The <see cref="ReadOnlySpan{UInt16}" /> for which to create the string.</param>
        /// <returns>A string created from <paramref name="source" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string? AsString(this ReadOnlySpan<ushort> source)
        {
            string? result = null;

            if (source.AsPointer() != null)
            {
                var chars = MemoryMarshal.Cast<ushort, char>(source);
                result = new string(chars);
            }

            return result;
        }

        /// <summary>Marshals a string to a null-terminated ASCII string.</summary>
        /// <param name="source">The string for which to marshal.</param>
        /// <returns>A null-terminated ASCII string that is equivalent to <paramref name="source" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<sbyte> MarshalStringToAscii(string? source)
        {
            ReadOnlySpan<sbyte> result;

            if (source is null)
            {
                result = null;
            }
            else
            {
                var maxLength = Encoding.ASCII.GetMaxByteCount(source.Length);
                var bytes = new sbyte[maxLength + 1];

                var length = Encoding.ASCII.GetBytes(source, MemoryMarshal.Cast<sbyte, byte>(bytes));
                result = bytes.AsSpan(0, length);
            }

            return result;
        }

        /// <summary>Marshals a string to a null-terminated UTF8 string.</summary>
        /// <param name="source">The string for which to marshal.</param>
        /// <returns>A null-terminated UTF8 string that is equivalent to <paramref name="source" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<sbyte> MarshalStringToUtf8(string? source)
        {
            ReadOnlySpan<sbyte> result;

            if (source is null)
            {
                result = null;
            }
            else
            {
                var maxLength = Encoding.UTF8.GetMaxByteCount(source.Length);
                var bytes = new sbyte[maxLength + 1];

                var length = Encoding.UTF8.GetBytes(source, MemoryMarshal.Cast<sbyte, byte>(bytes));
                result = bytes.AsSpan(0, length);
            }

            return result;
        }

        /// <summary>Marshals a string to a null-terminated UTF16 string.</summary>
        /// <param name="source">The string for which to marshal.</param>
        /// <returns>A null-terminated UTF16 string that is equivalent to <paramref name="source" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<ushort> MarshalStringToUtf16(string? source) => MemoryMarshal.Cast<char, ushort>(source.AsSpan());

        /// <summary>Marshals a null-terminated UTF8 string to a <see cref="ReadOnlySpan{SByte}" />.</summary>
        /// <param name="source">The pointer to the null-terminated UTF8 string.</param>
        /// <param name="maxLength">The maxmimum length of <paramref name="source" /> or <c>-1</c> if the maximum length is unknown.</param>
        /// <returns>A <see cref="ReadOnlySpan{SByte}" /> that starts at <paramref name="source" /> and extends to <paramref name="maxLength" /> or the first null character, whichever comes first.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<sbyte> MarshalUtf8ToReadOnlySpan(sbyte* source, nint maxLength = -1) => (source != null) ? MarshalUtf8ToReadOnlySpan(in *source, maxLength) : default;

        /// <summary>Marshals a null-terminated UTF8 string to a <see cref="ReadOnlySpan{SByte}" />.</summary>
        /// <param name="source">The pointer to the null-terminated UTF8 string.</param>
        /// <param name="maxLength">The maxmimum length of <paramref name="source" /> or <c>-1</c> if the maximum length is unknown.</param>
        /// <returns>A <see cref="ReadOnlySpan{SByte}" /> that starts at <paramref name="source" /> and extends to <paramref name="maxLength" /> or the first null character, whichever comes first.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<sbyte> MarshalUtf8ToReadOnlySpan(in sbyte source, nint maxLength = -1)
        {
            if (maxLength < 0)
            {
                maxLength = int.MaxValue;
            }

            var span = MemoryMarshal.CreateReadOnlySpan(ref AsRef(in source), (int)maxLength);
            var length = span.IndexOf((sbyte)'\0');

            if (length != -1)
            {
                span = span.Slice(0, length);
            }

            return span;
        }

        /// <summary>Marshals a null-terminated UTF16 string to a <see cref="ReadOnlySpan{UInt16}" />.</summary>
        /// <param name="source">The pointer to the null-terminated UTF16 string.</param>
        /// <param name="maxLength">The maxmimum length of <paramref name="source" /> or <c>-1</c> if the maximum length is unknown.</param>
        /// <returns>A <see cref="ReadOnlySpan{UInt16}" /> that starts at <paramref name="source" /> and extends to <paramref name="maxLength" /> or the first null character, whichever comes first.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<ushort> MarshalUtf16ToReadOnlySpan(ushort* source, nint maxLength = -1) => (source != null) ? MarshalUtf16ToReadOnlySpan(in *source, maxLength) : null;

        /// <summary>Marshals a null-terminated UTF16 string to a <see cref="ReadOnlySpan{UInt16}" />.</summary>
        /// <param name="source">The pointer to the null-terminated UTF16 string.</param>
        /// <param name="maxLength">The maxmimum length of <paramref name="source" /> or <c>-1</c> if the maximum length is unknown.</param>
        /// <returns>A <see cref="ReadOnlySpan{UInt16}" /> that starts at <paramref name="source" /> and extends to <paramref name="maxLength" /> or the first null character, whichever comes first.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlySpan<ushort> MarshalUtf16ToReadOnlySpan(in ushort source, nint maxLength = -1)
        {
            if (maxLength < 0)
            {
                maxLength = int.MaxValue;
            }

            var span = MemoryMarshal.CreateReadOnlySpan(ref AsRef(in source), (int)maxLength);
            var length = span.IndexOf('\0');

            if (length != -1)
            {
                span = span.Slice(0, length);
            }

            return span;
        }
    }
}
