using Microsoft.Xna.Framework;

namespace Enfilade.Interfaces
{
    internal interface ICollisionService
    {
        bool LinesIntersect(Vector2 firstLineStart, Vector2 firstLineEnd, Vector2 secondLineStart, Vector2 secondLineEnd, out Vector2 intersectionPoint);
    }
}