using System.Linq;
using Enfilade.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace Enfilade.Tests
{
    [TestClass]
    public class TessellationServiceTests
    {
        private static readonly Vector3[] ConcavePolygon =
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

        private static readonly Vector3[] ConvexPolygon =
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(3f, 0.3f, 0f),
            new Vector3(0f, 0.3f, 0f)
        };

        private TessellationService _tessellationService;

        [TestInitialize]
        public void TestInitialize()
        {
            _tessellationService = new TessellationService(new CollisionService());
        }

        [TestMethod]
        public void IsConvexPolygon_Returns_False()
        {
            var isConvexPolygon = _tessellationService.IsConvexPolygon(ConcavePolygon.Length, i => ConcavePolygon[i]);

            Assert.IsFalse(isConvexPolygon);
        }

        [TestMethod]
        public void IsConvexPolygon_Returns_True()
        {
            var isConvexPolygon = _tessellationService.IsConvexPolygon(ConvexPolygon.Length, i => ConvexPolygon[i]);

            Assert.IsTrue(isConvexPolygon);
        }

        [TestMethod]
        public void Triangulate_Triangulates_ConcavePolygon()
        {
            var x = _tessellationService.Triangulate(ConcavePolygon, Enumerable.Range(0, ConcavePolygon.Length), i => i);
        }
    }
}