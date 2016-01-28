using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using Enfilade.Extensions;
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

            //yield break;


            var bailout = 0;

            while (vertexIndexesList.Count > 3)
            {
                if (bailout++ > 1000)
                    break;

                for (var candidateVertexIndex = 2;
                    candidateVertexIndex < vertexIndexesList.Count;
                    ++candidateVertexIndex)
                {
                    var v1 = vertexPositions[indexFunc(vertexIndexesList[candidateVertexIndex - 2])];
                    var v2 = vertexPositions[indexFunc(vertexIndexesList[candidateVertexIndex - 1])];
                    var v3 = vertexPositions[indexFunc(vertexIndexesList[candidateVertexIndex])];

                    var anyLinesIntersect = false;

                    // does this intersect with any of the other lines?
                    for (var vertexIndex = 2; vertexIndex < vertexIndexesList.Count; ++vertexIndex)
                    {
                        if (vertexIndex == candidateVertexIndex) continue;

                        var p1 = vertexPositions[indexFunc(vertexIndexesList[vertexIndex])];
                        var p2 =
                            vertexPositions[indexFunc(vertexIndexesList[(vertexIndex + 1)%vertexIndexesList.Count])];

                        var linesIntersect = LinesIntersect(p1, p2, v1, v2) ||
                                             LinesIntersect(p1, p2, v2, v3) ||
                                             LinesIntersect(p1, p2, v3, v1);

                        if (linesIntersect)
                        {
                            anyLinesIntersect = true;
                            break;
                        }
                    }

                    if (anyLinesIntersect)
                        continue;

                    yield return vertexIndexesList[candidateVertexIndex - 2];
                    yield return vertexIndexesList[candidateVertexIndex - 1];
                    yield return vertexIndexesList[candidateVertexIndex];

                    vertexIndexesList.RemoveAt(candidateVertexIndex - 1);
                }
            }

            foreach (var vertexIndex in vertexIndexesList)
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
            Vector2 intersectionPoint;

            if (!_collisionService.LinesIntersect(
                new Vector2(firstLineStart.X, firstLineStart.Y), new Vector2(firstLineEnd.X, firstLineEnd.Y),
                new Vector2(secondLineStart.X, secondLineStart.Y), new Vector2(secondLineEnd.X, secondLineEnd.Y),
                out intersectionPoint))
            {
                return false;
            }

            var ignoredPoint = new Vector3(intersectionPoint.X, intersectionPoint.Y, 0);

            return firstLineStart != ignoredPoint &&
                   firstLineEnd != ignoredPoint &&
                   secondLineStart != ignoredPoint &&
                   secondLineEnd != ignoredPoint;
        }

        private static Vector3 GetSurfaceNormal<TVertexIndex>(IList<Vector3> vertexPositions,
            IList<TVertexIndex> vertexIndexes, Func<TVertexIndex, int> indexFunc)
        {
            var surfaceNormal = Vector3.Zero;

            for (var vertexIndex = 1; vertexIndex < vertexIndexes.Count; ++vertexIndex)
            {
                var vertexIndex1 = indexFunc(vertexIndexes[vertexIndex - 1]);
                var vertexIndex2 = indexFunc(vertexIndexes[vertexIndex]);

                surfaceNormal += Vector3.Cross(vertexPositions[vertexIndex1], vertexPositions[vertexIndex2]);
            }

            return Vector3.Normalize(surfaceNormal);
        }
    }
}