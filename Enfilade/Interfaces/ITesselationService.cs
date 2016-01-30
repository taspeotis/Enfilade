using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Enfilade.Interfaces
{
    public interface ITessellationService
    {
        bool IsConcavePolygon(int vertexIndexCount, Func<int, Vector3> indexFunc, out int concaveVertexIndex);

        IEnumerable<TVertexIndex> Triangulate<TVertexIndex>(IList<Vector3> vertexPositions,
            IEnumerable<TVertexIndex> vertexIndexes, Func<TVertexIndex, int> indexFunc);
    }
}