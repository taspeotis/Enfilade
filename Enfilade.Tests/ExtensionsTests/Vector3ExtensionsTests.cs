using Enfilade.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace Enfilade.Tests.ExtensionsTests
{
    [TestClass]
    public class Vector3ExtensionsTests
    {
        [TestMethod]
        public void CreateRotationMatrixTo_Returns_Identity()
        {
            var transformationMatrix = Vector3.UnitZ.CreateRotationMatrixTo(Vector3.UnitZ);

            Assert.AreEqual(Matrix.Identity, transformationMatrix);
        }
    }
}