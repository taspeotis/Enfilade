using System;
using System.Diagnostics;
using Enfilade.Infrastructure;
using Microsoft.Xna.Framework;

namespace Enfilade.Extensions
{
    public static class Vector3Extensions
    {
        /// <see>http://stackoverflow.com/a/11741520/242520</see>
        public static Matrix CreateRotationMatrixTo(this Vector3 fromVector, Vector3 toVector)
        {
            Debug.Assert(Math.Abs(1 - fromVector.Length()) < Constants.FloatEpsilon);
            Debug.Assert(Math.Abs(1 - toVector.Length()) < Constants.FloatEpsilon);

            var rotationQuaternion = fromVector.GetRotationQuaternionTo(toVector);
            
            return Matrix.CreateFromQuaternion(rotationQuaternion);
        }

        private static Quaternion GetRotationQuaternionTo(this Vector3 fromVector, Vector3 toVector)
        {
            if (fromVector == -toVector)
            {
                var orthogonalVector = fromVector.Orthogonal();
                var normalizedVector = Vector3.Normalize(orthogonalVector);

                return Quaternion.CreateFromAxisAngle(normalizedVector, 0);
            }

            var halfVector = Vector3.Normalize(fromVector + toVector);
            var vectorCross = Vector3.Cross(fromVector, halfVector);
            var vectorDot = Vector3.Dot(fromVector, halfVector);

            return new Quaternion(vectorCross, vectorDot);
        }

        private static Vector3 Orthogonal(this Vector3 vector)
        {
            var x = Math.Abs(vector.X);
            var y = Math.Abs(vector.Y);
            var z = Math.Abs(vector.Z);

            var otherVector = x < y
                ? (x < z ? Vector3.UnitX : Vector3.UnitZ)
                : (y < z ? Vector3.UnitY : Vector3.UnitZ);

            return Vector3.Cross(vector, otherVector);
        }
    }
}