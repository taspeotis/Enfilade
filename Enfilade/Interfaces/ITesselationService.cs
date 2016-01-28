using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Enfilade.Interfaces
{
    public interface ITessellationService
    {
        bool IsConvexPolygon(int vertexIndexCount, Func<int, Vector3> indexFunc);

        IEnumerable<TVertexIndex> Triangulate<TVertexIndex>(IList<Vector3> vertexPositions,
            IEnumerable<TVertexIndex> vertexIndexes, Func<TVertexIndex, int> indexFunc);
    }
}