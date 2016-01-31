using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Enfilade.Extensions;
using Enfilade.Interfaces;
using Microsoft.Xna.Framework;

namespace Enfilade.Services
{
    [Export(typeof (ITessellationService))]
    internal sealed class TessellationService : ITessellationService
    {
        public bool IsConcavePolygon(int vertexIndexCount, Func<int, Vector3> indexFunc, out int concaveVertexIndex)
        {
            if (vertexIndexCount < 3)
                throw new ArgumentOutOfRangeException(nameof(vertexIndexCount), vertexIndexCount, null);

            var positiveZ = false;
            var negativeZ = false;

            for (concaveVertexIndex = 0; concaveVertexIndex < vertexIndexCount; ++concaveVertexIndex)
            {
                var pointA = indexFunc(concaveVertexIndex);
                var pointB = indexFunc((concaveVertexIndex + 1)%vertexIndexCount);
                var pointC = indexFunc((concaveVertexIndex + 2)%vertexIndexCount);

                var vectorA = pointA - pointB;
                var vectorB = pointC - pointB;

                var crossProductLength = (float) ((double) vectorA.X*vectorB.Y -
                                                  (double) vectorB.X*vectorA.Y);

                if (crossProductLength > 0)
                    positiveZ = true;

                if (crossProductLength < 0)
                    negativeZ = true;

                if (positiveZ && negativeZ)
                    return true;
            }

            return false;
        }

        public IEnumerable<TVertexIndex> Triangulate<TVertexIndex>(IList<Vector3> vertexPositions,
            IEnumerable<TVertexIndex> vertexIndexes, Func<TVertexIndex, int> indexFunc)
        {
            var vertexIndexesList = vertexIndexes.ToList();
            var surfaceNormal = vertexIndexesList.Select(vi => vertexPositions[indexFunc(vi)]).GetSurfaceNormal();
            var transformationMatrix = surfaceNormal.CreateRotationMatrixTo(Vector3.UnitZ);

            for (int concaveVertexIndex;
                IsConcavePolygon(vertexIndexesList.Count,
                    i => Vector3.Transform(vertexPositions[indexFunc(vertexIndexesList[i])], transformationMatrix),
                    out concaveVertexIndex);)
            {
                yield return vertexIndexesList[concaveVertexIndex - 1];
                yield return vertexIndexesList[concaveVertexIndex];
                yield return vertexIndexesList[(concaveVertexIndex + 1)%vertexIndexesList.Count];

                vertexIndexesList.RemoveAt(concaveVertexIndex);
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
    }
}