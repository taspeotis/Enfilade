using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Enfilade.Extensions;
using Enfilade.Infrastructure;
using Enfilade.Interfaces;
using Microsoft.Xna.Framework;

namespace Enfilade.Services
{
    [Export(typeof (ITessellationService))]
    internal sealed class TessellationService : ITessellationService
    {
        private readonly ICollisionService _collisionService;

        [ImportingConstructor]
        public TessellationService(ICollisionService collisionService)
        {
            _collisionService = collisionService;
        }

        public bool IsConvexPolygon(int vertexIndexCount, Func<int, Vector3> indexFunc)
        {
            if (vertexIndexCount < 3)
                throw new ArgumentOutOfRangeException(nameof(vertexIndexCount), vertexIndexCount, null);

            var positiveZ = false;
            var negativeZ = false;

            for (var vertexIndex = 0; vertexIndex < vertexIndexCount; ++vertexIndex)
            {
                var vectorA = indexFunc(vertexIndex);
                var vectorB = indexFunc((vertexIndex + 1)%vertexIndexCount);

                var crossProductLength = (float) ((double) vectorA.X*vectorB.Y -
                                                  (double) vectorB.X*vectorA.Y);

                if (crossProductLength > 0)
                    positiveZ = true;

                if (crossProductLength < 0)
                    negativeZ = true;

                if (positiveZ && negativeZ)
                    return false;
            }

            return true;
        }

        public IEnumerable<TVertexIndex> Triangulate<TVertexIndex>(IList<Vector3> vertexPositions,
            IEnumerable<TVertexIndex> vertexIndexes, Func<TVertexIndex, int> indexFunc)
        {
            var vertexIndexesList = vertexIndexes.ToList();

            var surfaceNormal = GetSurfaceNormal(vertexPositions, vertexIndexesList, indexFunc);
            var transformationMatrix = surfaceNormal.CreateRotationMatrixTo(Vector3.UnitZ);

            if (IsConvexPolygon(vertexIndexesList.Count, i => Vector3.Transform(vertexPositions[indexFunc(vertexIndexesList[i])], transformationMatrix)))
            {
                foreach (var vertexIndex in GetTriangleFanVertexIndexes(vertexIndexesList))
                    yield return vertexIndex;

                yield break;
            }

            while (vertexIndexesList.Count > 4)
            {
                for (var candidateVertexIndex = 0; candidateVertexIndex < vertexIndexesList.Count;)
                {
                    var cvi1 = vertexIndexesList[candidateVertexIndex];
                    var cvi2 = vertexIndexesList[(candidateVertexIndex + 2) % vertexIndexesList.Count];

                    var v1 = Vector3.Transform(vertexPositions[indexFunc(cvi1)], transformationMatrix);
                    var v2 = Vector3.Transform(vertexPositions[indexFunc(cvi2)], transformationMatrix);

                    var anyLinesIntersect = false;

                    for (var vertexIndex = 0; vertexIndex < vertexIndexesList.Count; ++vertexIndex)
                    {
                        var vi1 = vertexIndex;
                        var vi2 = (vertexIndex + 1) % vertexIndexesList.Count;

                        var p1 = vertexPositions[indexFunc(vertexIndexesList[vi1])];
                        var p2 = vertexPositions[indexFunc(vertexIndexesList[vi2])];

                        p1 = Vector3.Transform(p1, transformationMatrix);
                        p2 = Vector3.Transform(p2, transformationMatrix);

                        if (LinesIntersect(p1, p2, v1, v2))
                        {
                            anyLinesIntersect = true;
                            break;
                        }
                    }

                    if (anyLinesIntersect)
                    {
                        candidateVertexIndex++;

                        continue;
                    }

                    // Winding ??
                    yield return cvi1;
                    yield return vertexIndexesList[(candidateVertexIndex + 1)%vertexIndexesList.Count];
                    yield return cvi2;

                    vertexIndexesList.RemoveAt((candidateVertexIndex + 1) % vertexIndexesList.Count);
                    candidateVertexIndex = 0;
                }
            }

            foreach (var vertexIndex in GetTriangleFanVertexIndexes(vertexIndexesList))
                yield return vertexIndex;
        }

        private static IEnumerable<TVertexIndex>
            GetTriangleFanVertexIndexes<TVertexIndex>(IReadOnlyList<TVertexIndex> vertexIndexesList)
        {
            // Triangle fan
            for (var vertexIndex = 2; vertexIndex < vertexIndexesList.Count; ++vertexIndex)
            {
                yield return vertexIndexesList[0];
                yield return vertexIndexesList[vertexIndex - 1];
                yield return vertexIndexesList[vertexIndex];
            }
        }

        private bool LinesIntersect(
            Vector3 firstLineStart, Vector3 firstLineEnd, Vector3 secondLineStart, Vector3 secondLineEnd)
        {
            // Bit of a kludge to avoid dealing with indicative XY values but disparate Z values
            firstLineStart.Z = 0;
            firstLineEnd.Z = 0;
            secondLineStart.Z = 0;
            secondLineEnd.Z = 0;

            Vector2 intersectionPoint;

            if (!_collisionService.LinesIntersect(
                new Vector2(firstLineStart.X, firstLineStart.Y), new Vector2(firstLineEnd.X, firstLineEnd.Y),
                new Vector2(secondLineStart.X, secondLineStart.Y), new Vector2(secondLineEnd.X, secondLineEnd.Y),
                out intersectionPoint))
            {
                return false;
            }

            // Note the kludge: the 0 for Z is not a coincidence.
            var ignoredPoint = new Vector3(intersectionPoint.X, intersectionPoint.Y, 0);

            Func<Vector3, Vector3, bool> notEqualFunc =
                (v1, v2) => Math.Abs((v1 - v2).Length()) > Constants.FloatEpsilon;

            return notEqualFunc(firstLineStart, ignoredPoint) &&
                   notEqualFunc(firstLineEnd, ignoredPoint) &&
                   notEqualFunc(secondLineStart, ignoredPoint) &&
                   notEqualFunc(secondLineEnd, ignoredPoint);
        }

        private static Vector3 GetSurfaceNormal<TVertexIndex>(IList<Vector3> vertexPositions,
            IList<TVertexIndex> vertexIndexes, Func<TVertexIndex, int> indexFunc)
        {
            var surfaceNormal = Vector3.Zero;

            for (var vertexIndex = 0; vertexIndex < vertexIndexes.Count; ++vertexIndex)
            {
                var vertexIndex1 = indexFunc(vertexIndexes[vertexIndex]);
                var vertexIndex2 = indexFunc(vertexIndexes[(vertexIndex + 1)%vertexIndexes.Count]);

                surfaceNormal += Vector3.Cross(vertexPositions[vertexIndex1], vertexPositions[vertexIndex2]);
            }

            return Vector3.Normalize(surfaceNormal);
        }
    }
}