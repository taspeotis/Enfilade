using Enfilade.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace Enfilade.Tests
{
    [TestClass]
    public class TessellationServiceTests
    {
        private TessellationService _tessellationService;

        [TestInitialize]
        public void TestInitialize()
        {
            _tessellationService = new TessellationService(new CollisionService());
        }

        [TestMethod]
        public void IsConvexPolygon_Returns_False()
        {
            var concavePolygon = new[]
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

            var isConvexPolygon = _tessellationService.IsConvexPolygon(concavePolygon.Length, i => concavePolygon[i]);

            Assert.IsFalse(isConvexPolygon);
        }

        [TestMethod]
        public void IsConvexPolygon_Returns_True()
        {
            var convexPolygon = new[]
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(3f, 0f, 0f),
                new Vector3(3f, 0.3f, 0f),
                new Vector3(0f, 0.3f, 0f)
            };

            var isConvexPolygon = _tessellationService.IsConvexPolygon(convexPolygon.Length, i => convexPolygon[i]);

            Assert.IsTrue(isConvexPolygon);
        }
    }
}