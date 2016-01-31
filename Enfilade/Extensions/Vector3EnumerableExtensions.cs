using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Enfilade.Extensions
{
    public static class Vector3EnumerableExtensions
    {
        public static Vector3 GetSurfaceNormal(this IEnumerable<Vector3> vertexPositions)
        {
            var surfaceNormal = Vector3.Zero;

            using (var vertexPositionEnumerator = vertexPositions.GetEnumerator())
            {
                if (!vertexPositionEnumerator.MoveNext())
                    throw new ArgumentOutOfRangeException(nameof(vertexPositions));

                var firstVector = vertexPositionEnumerator.Current;
                var previousVector = firstVector;

                while (vertexPositionEnumerator.MoveNext())
                {
                    var currentVector = vertexPositionEnumerator.Current;

                    surfaceNormal.X += (previousVector.Y - currentVector.Y)*(previousVector.Z + currentVector.Z);
                    surfaceNormal.Y += (previousVector.Z - currentVector.Z)*(previousVector.X + currentVector.X);
                    surfaceNormal.Z += (previousVector.X - currentVector.X)*(previousVector.Y + currentVector.Y);

                    previousVector = currentVector;
                }

                surfaceNormal.X += (previousVector.Y - firstVector.Y) * (previousVector.Z + firstVector.Z);
                surfaceNormal.Y += (previousVector.Z - firstVector.Z) * (previousVector.X + firstVector.X);
                surfaceNormal.Z += (previousVector.X - firstVector.X) * (previousVector.Y + firstVector.Y);
            }

            return Vector3.Normalize(surfaceNormal);
        }
    }
}