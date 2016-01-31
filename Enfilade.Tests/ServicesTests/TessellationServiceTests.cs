using System;
using System.Linq;
using Enfilade.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace Enfilade.Tests.ServicesTests
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

        private static readonly Vector3[] Polygon =
        {
            new Vector3(2.60396f, 0.15f, -1.27492f),
            new Vector3(2.2413f, 0.15f, -1.92432f),
            new Vector3(2.38747f, 0.15f, -2.38719f),
            new Vector3(2.2413f, 0.15f, -3f),
            new Vector3(0.758698f, 0.15f, -3f),
            new Vector3(0.904869f, 0.15f, -2.38719f),
            new Vector3(0.758698f, 0.15f, -1.92432f),
            new Vector3(1.12136f, 0.15f, -1.27492f),
            new Vector3(0.758698f, 0.15f, -0.701031f),
            new Vector3(0.877648f, 0.15f, -0.371864f),
            new Vector3(0.758698f, 0.15f, 0f),
            new Vector3(2.2413f, 0.15f, 0f),
            new Vector3(2.36025f, 0.15f, -0.371864f),
            new Vector3(2.2413f, 0.15f, -0.701031f)
        };

        private TessellationService _tessellationService;

        [TestInitialize]
        public void TestInitialize()
        {
            _tessellationService = new TessellationService();
        }

        [TestMethod]
        public void IsConcavePolygon_Returns_False()
        {
            int concaveVertexIndex;

            var isConcavePolygon = _tessellationService.IsConcavePolygon(
                ConvexPolygon.Length, i => ConvexPolygon[i], out concaveVertexIndex);

            Assert.IsFalse(isConcavePolygon);
        }

        [TestMethod]
        public void IsConcavePolygon_Returns_True()
        {
            int concaveVertexIndex;

            var isConcavePolygon = _tessellationService.IsConcavePolygon(
                ConcavePolygon.Length, i => ConcavePolygon[i], out concaveVertexIndex);

            Assert.IsTrue(isConcavePolygon);
            Assert.AreEqual(3, concaveVertexIndex);
        }

        [TestMethod]
        public void IsConcavePolygon_Returns_ConcaveVertexIndex()
        {
            var concavePolygon = ConcavePolygon.ToList();

            int concaveVertexIndex;
            Func<int, Vector3> indexFunc = i => ConcavePolygon[i];

            _tessellationService.IsConcavePolygon(concavePolygon.Count, indexFunc, out concaveVertexIndex);

            Assert.AreEqual(3, concaveVertexIndex);
            concavePolygon.RemoveAt(concaveVertexIndex);

            _tessellationService.IsConcavePolygon(concavePolygon.Count, indexFunc, out concaveVertexIndex);

            Assert.AreEqual(3, concaveVertexIndex);
            concavePolygon.RemoveAt(concaveVertexIndex);

            _tessellationService.IsConcavePolygon(concavePolygon.Count, indexFunc, out concaveVertexIndex);

            Assert.AreEqual(3, concaveVertexIndex);
            
        }

        [TestMethod]
        public void Triangulate_Triangulates_ConcavePolygon()
        {
            var x = _tessellationService.Triangulate(Polygon, Enumerable.Range(0, Polygon.Length), i => i).ToList();
        }
    }
}