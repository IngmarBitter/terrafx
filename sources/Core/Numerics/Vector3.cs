// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Globalization;
using System.Text;
using TerraFX.Utilities;

namespace TerraFX.Numerics
{
    /// <summary>Defines a three-dimensional Euclidean vector.</summary>
    public readonly struct Vector3 : IEquatable<Vector3>, IFormattable
    {
        /// <summary>Defines a <see cref="Vector3" /> where all components are zero.</summary>
        public static readonly Vector3 Zero = new Vector3(0.0f, 0.0f, 0.0f);

        /// <summary>Defines a <see cref="Vector3" /> whose x-component is one and whose remaining components are zero.</summary>
        public static readonly Vector3 UnitX = new Vector3(1.0f, 0.0f, 0.0f);

        /// <summary>Defines a <see cref="Vector3" /> whose y-component is one and whose remaining components are zero.</summary>
        public static readonly Vector3 UnitY = new Vector3(0.0f, 1.0f, 0.0f);

        /// <summary>Defines a <see cref="Vector3" /> whose z-component is one and whose remaining components are zero.</summary>
        public static readonly Vector3 UnitZ = new Vector3(0.0f, 0.0f, 1.0f);

        /// <summary>Defines a <see cref="Vector3" /> where all components are one.</summary>
        public static readonly Vector3 One = new Vector3(1.0f, 1.0f, 1.0f);

        private readonly float _x;
        private readonly float _y;
        private readonly float _z;

        /// <summary>Initializes a new instance of the <see cref="Vector3" /> struct.</summary>
        /// <param name="x">The value of the x-dimension.</param>
        /// <param name="y">The value of the y-dimension.</param>
        /// <param name="z">The value of the z-dimension.</param>
        public Vector3(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        /// <summary>Initializes a new instance of the <see cref="Vector3" /> struct with each component set to <paramref name="value" />.</summary>
        /// <param name="value">The value to set each component to.</param>
        public Vector3(float value)
        {
            _x = value;
            _y = value;
            _z = value;
        }

        /// <summary>Gets the value of the x-dimension.</summary>
        public float X => _x;

        /// <summary>Gets the value of the y-dimension.</summary>
        public float Y => _y;

        /// <summary>Gets the value of the z-dimension.</summary>
        public float Z => _z;

        /// <summary>Gets the square-rooted length of the vector.</summary>
        public float Length => MathF.Sqrt(LengthSquared);

        /// <summary>Gets the squared length of the vector.</summary>
        public float LengthSquared => Dot(this, this);

        /// <summary>Compares two <see cref="Vector3" /> instances to determine equality.</summary>
        /// <param name="left">The <see cref="Vector3" /> to compare with <paramref name="right" />.</param>
        /// <param name="right">The <see cref="Vector3" /> to compare with <paramref name="left" />.</param>
        /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return (left.X == right.X)
                && (left.Y == right.Y)
                && (left.Z == right.Z);
        }

        /// <summary>Compares two <see cref="Vector3" /> instances to determine inequality.</summary>
        /// <param name="left">The <see cref="Vector3" /> to compare with <paramref name="right" />.</param>
        /// <param name="right">The <see cref="Vector3" /> to compare with <paramref name="left" />.</param>
        /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return (left.X != right.X)
                || (left.Y != right.Y)
                || (left.Z != right.Z);
        }

        /// <summary>Returns the value of the <see cref="Vector3" /> operand (the sign of the operand is unchanged).</summary>
        /// <param name="value">The operand to return</param>
        /// <returns>The value of the operand, <paramref name="value" />.</returns>
        public static Vector3 operator +(Vector3 value) => value;

        /// <summary>Negates the value of the specified <see cref="Vector3" /> operand.</summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The result of <paramref name="value" /> multiplied by negative one (-1).</returns>
        public static Vector3 operator -(Vector3 value) => value * -1;

        /// <summary>Adds two specified <see cref="Vector3" /> values.</summary>
        /// <param name="left">The first value to add.</param>
        /// <param name="right">The second value to add.</param>
        /// <returns>The result of adding <paramref name="left" /> and <paramref name="right" />.</returns>
        public static Vector3 operator +(Vector3 left, Vector3 right) => new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

        /// <summary>Subtracts two specified <see cref="Vector3" /> values.</summary>
        /// <param name="left">The minuend.</param>
        /// <param name="right">The subtrahend.</param>
        /// <returns>The result of subtracting <paramref name="right" /> from <paramref name="left" />.</returns>
        public static Vector3 operator -(Vector3 left, Vector3 right) => new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        /// <summary>Multiplies two specified <see cref="Vector3" /> values.</summary>
        /// <param name="left">The first value to multiply.</param>
        /// <param name="right">The second value to multiply.</param>
        /// <returns>The result of multiplying <paramref name="left" /> by <paramref name="right" />.</returns>
        public static Vector3 operator *(Vector3 left, Vector3 right) => new Vector3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

        /// <summary>Divides two specified <see cref="Vector3" /> values.</summary>
        /// <param name="left">The dividend.</param>
        /// <param name="right">The divisor.</param>
        /// <returns>The result of dividing <paramref name="left" /> by <paramref name="right" />.</returns>
        public static Vector3 operator /(Vector3 left, Vector3 right) => new Vector3(left.X / right.X, left.Y / right.Y, left.Z / right.Z);

        /// <summary>Multiplies each component of a <see cref="Vector3" /> value by a given <see cref="float" /> value.</summary>
        /// <param name="left">The vector to multiply.</param>
        /// <param name="right">The value to multiply each component by.</param>
        /// <returns>The result of multiplying each component of <paramref name="left" /> by <paramref name="right" />.</returns>
        public static Vector3 operator *(Vector3 left, float right) => new Vector3(left.X * right, left.Y * right, left.Z * right);

        /// <summary>Matrix-Vector multiplication between the left <see cref="Matrix3x3" /> and right <see cref="Vector3" />.</summary>
        /// <param name="left">The vector to multiply.</param>
        /// <param name="right">The matrix for multiplcation.</param>
        /// <returns>The resulting transformed <see cref="Vector3" />.</returns>
        public static Vector3 operator *(Vector3 left, Matrix3x3 right) => new Vector3(
            (left.X * right.X.X) + (left.Y * right.Y.X) + (left.Z * right.Z.X),
            (left.X * right.X.Y) + (left.Y * right.Y.Y) + (left.Z * right.Z.Y),
            (left.X * right.X.Z) + (left.Y * right.Y.Z) + (left.Z * right.Z.Z)
        );

        /// <summary>Divides each component of a <see cref="Vector3" /> value by a given <see cref="float" /> value.</summary>
        /// <param name="left">The dividend.</param>
        /// <param name="right">The divisor to divide each component by.</param>
        /// <returns>The result of multiplying each component of <paramref name="left" /> by <paramref name="right" />.</returns>
        public static Vector3 operator /(Vector3 left, float right) => new Vector3(left.X / right, left.Y / right, left.Z / right);

        /// <summary>Computes the cross product of two <see cref="Vector3" /> values.</summary>
        /// <remarks>This method assumes both vectors <paramref name="left" /> and <paramref name="right" /> start at the origin.</remarks>
        /// <param name="left">The first value to cross.</param>
        /// <param name="right">The second value to cross.</param>
        /// <returns>The cross product of <paramref name="left" /> and <paramref name="right" /></returns>
        public static Vector3 Cross(Vector3 left, Vector3 right) => new Vector3(
            (left.Y * right.Z) - (left.Z * right.Y),
            (left.Z * right.X) - (left.X * right.Z),
            (left.X * right.Y) - (left.Y * right.X)
        );

        /// <summary>Calculates the dot product of two <see cref="Vector3" /> values.</summary>
        /// <param name="left">The first value to dot.</param>
        /// <param name="right">The second value to dot.</param>
        /// <returns>The result of adding the multiplication of each component of <paramref name="left" /> by each component of <paramref name="right" />.</returns>
        public static float Dot(Vector3 left, Vector3 right) => (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);

        /// <inheritdoc />
        public override bool Equals(object? obj) => (obj is Vector3 other) && Equals(other);

        /// <inheritdoc />
        public bool Equals(Vector3 other) => this == other;

        /// <summary>Tests if two <see cref="Vector3" /> instances have sufficiently similar values to see them as equivalent.
        /// Use this to compare values that might be affected by differences in rounding the least significant bits.</summary>
        /// <param name="left">The left instance to compare.</param>
        /// <param name="right">The right instance to compare.</param>
        /// <param name="epsilon">The threshold below which they are sufficiently similar.</param>
        /// <returns><c>True</c> if similar, <c>False</c> otherwise.</returns>
        public static bool EqualEstimate(Vector3 left, Vector3 right, Vector3 epsilon)
        {
            return MathUtilities.EqualEstimate(left.X, right.X, epsilon.X)
                && MathUtilities.EqualEstimate(left.Y, right.Y, epsilon.Y)
                && MathUtilities.EqualEstimate(left.Z, right.Z, epsilon.Z);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            {
                hashCode.Add(X);
                hashCode.Add(Y);
                hashCode.Add(Z);
            }
            return hashCode.ToHashCode();
        }

        /// <summary>Computes the normalized value of the given <see cref="Vector3" /> value.</summary>
        /// <param name="value">The value to normalize.</param>
        /// <returns>The unit vector of <paramref name="value" />.</returns>
        public static Vector3 Normalize(Vector3 value) => value / value.Length;

        /// <summary>Computes the <see cref="Vector3" /> that for each component has the maximum value out of this and v.</summary>
        /// <param name="left">The <see cref="Vector3" /> for this operation.</param>
        /// <param name="right">The other <see cref="Vector3" /> to compute the max with.</param>
        /// <returns>The resulting new instance.</returns>
        public static Vector3 Max(Vector3 left, Vector3 right) => new Vector3(MathF.Max(left.X, right.X), MathF.Max(left.Y, right.Y), MathF.Max(left.Z, right.Z));

        /// <summary>Computes the <see cref="Vector3" /> that for each component has the minimum value out of this and v.</summary>
        /// <param name="left">The <see cref="Vector3" /> for this operation.</param>
        /// <param name="right">The other <see cref="Vector3" /> to compute the min with.</param>
        /// <returns>The resulting new instance.</returns>
        public static Vector3 Min(Vector3 left, Vector3 right) => new Vector3(MathF.Min(left.X, right.X), MathF.Min(left.Y, right.Y), MathF.Min(left.Z, right.Z));

        /// <summary>The given <see cref="Vector3" /> rotated by this <see cref="Quaternion" />.</summary>
         /// <param name="value">The <see cref="Vector3" /> to rotate.</param>
       /// <param name="rotation">The <see cref="Quaternion" /> for this operation.</param>
        /// <returns>The resulting rotated <see cref="Vector3" />.</returns>
        public static Vector3 Rotate(Vector3 value, Quaternion rotation) => value * Quaternion.ToMatrix3x3(rotation);

        /// <summary>The given <see cref="Vector3" /> rotated by the inverse of this <see cref="Quaternion" />.</summary>
        /// <param name="value">The <see cref="Vector3" /> to rotate.</param>
        /// <param name="rotation">The <see cref="Quaternion" /> for this operation.</param>
        /// <returns>The resulting rotated <see cref="Vector3" />.</returns>
        public static Vector3 RotateInverse(Vector3 value, Quaternion rotation) => value * Quaternion.ToMatrix3x3(Quaternion.Invert(rotation));

        /// <inheritdoc />
        public override string ToString() => ToString(format: null, formatProvider: null);

        /// <inheritdoc />
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

            return new StringBuilder(7 + (separator.Length * 2))
                .Append('<')
                .Append(X.ToString(format, formatProvider))
                .Append(separator)
                .Append(' ')
                .Append(Y.ToString(format, formatProvider))
                .Append(separator)
                .Append(' ')
                .Append(Z.ToString(format, formatProvider))
                .Append('>')
                .ToString();
        }

        /// <summary>Matrix-Vector multiplication between the left <see cref="Matrix4x4" /> and right <see cref="Vector3" />.</summary>
        /// <param name="value">The <see cref="Vector3" /> for this operation.</param>
        /// <param name="matrix">The <see cref="Matrix4x4" /> to multiply.</param>
        /// <returns>The resulting transformed <see cref="Vector3" />.</returns>
        public static Vector3 Transform(Vector3 value, Matrix4x4 matrix) => new Vector3(
            (value.X * matrix.X.X) + (value.Y * matrix.Y.X) + (value.Z * matrix.Z.X) + matrix.W.X,
            (value.X * matrix.X.Y) + (value.Y * matrix.Y.Y) + (value.Z * matrix.Z.Y) + matrix.W.Y,
            (value.X * matrix.X.Z) + (value.Y * matrix.Y.Z) + (value.Z * matrix.Z.Z) + matrix.W.Z
        );

        /// <summary>Matrix-Vector multiplication between the left <see cref="Matrix4x4" /> and right <see cref="Vector3" />.</summary>
        /// <param name="value">The <see cref="Vector3" /> for this operation.</param>
        /// <param name="rotation">The <see cref="Matrix4x4" /> to multiply.</param>
        /// <returns>The resulting transformed <see cref="Vector3" />.</returns>
        public static Vector3 TransformNormal(Vector3 value, Matrix4x4 rotation) => new Vector3(
            (value.X * rotation.X.X) + (value.Y * rotation.Y.X) + (value.Z * rotation.Z.X),
            (value.X * rotation.X.Y) + (value.Y * rotation.Y.Y) + (value.Z * rotation.Z.Y),
            (value.X * rotation.X.Z) + (value.Y * rotation.Y.Z) + (value.Z * rotation.Z.Z)
        );

        /// <summary>Creates a new <see cref="Vector3" /> instance with <see cref="X" /> set to the specified value.</summary>
        /// <param name="value">The new value of the x-dimension.</param>
        /// <returns>A new <see cref="Vector3" /> instance with <see cref="X" /> set to <paramref name="value" />.</returns>
        public Vector3 WithX(float value) => new Vector3(value, Y, Z);

        /// <summary>Creates a new <see cref="Vector3" /> instance with <see cref="Y" /> set to the specified value.</summary>
        /// <param name="value">The new value of the y-dimension.</param>
        /// <returns>A new <see cref="Vector3" /> instance with <see cref="Y" /> set to <paramref name="value" />.</returns>
        public Vector3 WithY(float value) => new Vector3(X, value, Z);

        /// <summary>Creates a new <see cref="Vector3" /> instance with <see cref="Z" /> set to the specified value.</summary>
        /// <param name="value">The new value of the z-dimension.</param>
        /// <returns>A new <see cref="Vector3" /> instance with <see cref="Z" /> set to <paramref name="value" />.</returns>
        public Vector3 WithZ(float value) => new Vector3(X, Y, value);
    }
}
