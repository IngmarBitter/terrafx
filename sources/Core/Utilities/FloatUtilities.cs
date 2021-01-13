// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace TerraFX.Utilities
{
    /// <summary>Provides a set of methods for manipulating floating-point values.</summary>
    public static class FloatUtilities
    {
        /// <summary>Clamps a <see cref="float" /> value to be between a minimum and maximum value.</summary>
        /// <param name="value">The value to restrict.</param>
        /// <param name="min">The minimum value (inclusive).</param>
        /// <param name="max">The maximum value (inclusive).</param>
        /// <returns><paramref name="value" /> clamped to be between <paramref name="min" /> and <paramref name="max" />.</returns>
        public static float Clamp(float value, float min, float max)
        {
            // The compare order here is important.
            // It ensures we match HLSL behavior for the scenario where min is larger than max.

            var result = value;

            result = (result > max) ? max : result;
            result = (result < min) ? min : result;

            return result;
        }

        /// <summary>Tests if two <see cref="float"/> instances have sufficiently similar values to see them as equivalent.
        /// Use this to compare values that might be affected by differences in rounding the least significant bits.</summary>
        /// <param name="left">The first insance to compare.</param>
        /// <param name="right">The other instance to compare.</param>
        /// <param name="epsilon">The threshold below which they are sufficiently similar.</param>
        /// <returns><c>true</c> if similar, <c>false</c> otherwise.</returns>
        public static bool EqualEstimate(this float left, float right, float epsilon) => MathF.Abs(right - left) < epsilon;
    }
}
