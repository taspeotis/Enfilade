using System;
using System.ComponentModel.Composition;
using Enfilade.Infrastructure;
using Enfilade.Interfaces;
using Microsoft.Xna.Framework;

namespace Enfilade.Services
{
    [Export(typeof (ICollisionService))]
    internal sealed class CollisionService : ICollisionService
    {
        // From Microsoft's CollisionMath.cs
        public bool LinesIntersect(Vector2 firstLineStart, Vector2 firstLineEnd,
            Vector2 secondLineStart, Vector2 secondLineEnd, out Vector2 intersectionPoint)
        {
            intersectionPoint = Vector2.Zero;

            double denominator = (firstLineEnd.X - firstLineStart.X)*(secondLineEnd.Y - secondLineStart.Y) -
                                 (firstLineEnd.Y - firstLineStart.Y)*(secondLineEnd.X - secondLineStart.X);

            // If the denominator in above is zero, AB & CD are colinear
            if (Math.Abs(denominator) < Constants.DoubleEpsilon)
                return false;

            double numeratorR = (firstLineStart.Y - secondLineStart.Y)*(secondLineEnd.X - secondLineStart.X) -
                                (firstLineStart.X - secondLineStart.X)*(secondLineEnd.Y - secondLineStart.Y);

            double numeratorS = (firstLineStart.Y - secondLineStart.Y)*(firstLineEnd.X - firstLineStart.X) -
                                (firstLineStart.X - secondLineStart.X)*(firstLineEnd.Y - firstLineStart.Y);

            var r = numeratorR/denominator;
            var s = numeratorS/denominator;

            // non-intersecting
            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            // find intersection point
            intersectionPoint.X = (float) (firstLineStart.X + r*(firstLineEnd.X - firstLineStart.X));
            intersectionPoint.Y = (float) (firstLineStart.Y + r*(firstLineEnd.Y - firstLineStart.Y));

            return true;
        }
    }
}