using System;
using Enfilade.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace Enfilade.Tests.ExtensionsTests
{
    [TestClass]
    public class Vector3EnumerableExtensionsTests
    {
        private static readonly Vector3[] Polygon =
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(3f, 0.3f, 0f),
            new Vector3(2.2413f, 0.3f, 0f),
            new Vector3(2.2413f, 0.15f, 0f),
            new Vector3(0.758698f, 0.15f, 0f),
            new Vector3(0.758698f, 0.3f, 0f),
            new Vector3(0f, 0.3f, 0f)
        };

        [TestMethod]
        public void GetSurfaceNormal_Returns_SurfaceNormal()
        {
            var surfaceNormal = Polygon.GetSurfaceNormal();

            Assert.AreEqual(Vector3.UnitZ, surfaceNormal);
        }
    }
}
