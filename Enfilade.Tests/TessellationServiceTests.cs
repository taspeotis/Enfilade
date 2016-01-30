using System;
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
            Assert.AreEqual(concaveVertexIndex, 3);
        }

        [TestMethod]
        public void IsConcavePolygon_Returns_ConcaveVertexIndex()
        {
            var concavePolygon = ConcavePolygon.ToList();

            int concaveVertexIndex;
            Func<int, Vector3> indexFunc = i => ConcavePolygon[i];

            _tessellationService.IsConcavePolygon(concavePolygon.Count, indexFunc, out concaveVertexIndex);

            Assert.AreEqual(concaveVertexIndex, 4);
            concavePolygon.RemoveAt(concaveVertexIndex);

            _tessellationService.IsConcavePolygon(concavePolygon.Count, indexFunc, out concaveVertexIndex);

            Assert.AreEqual(concaveVertexIndex, 3);
            concavePolygon.RemoveAt(concaveVertexIndex);
        }

        [TestMethod]
        public void Triangulate_Triangulates_ConcavePolygon()
        {
            var x = _tessellationService.Triangulate(ConcavePolygon, Enumerable.Range(0, ConcavePolygon.Length), i => i);
        }
    }
}