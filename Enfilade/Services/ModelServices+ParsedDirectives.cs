using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Enfilade.Services
{
    internal sealed partial class ModelService
    {
        private sealed class ParsedDirectives
        {
            public ParsedDirectives(GraphicsDevice graphicsDevice)
            {
                //newmtl Wood
                //Ka 0.000000 0.000000 0.000000
                //Kd 0.666667 0.545098 0.356863
                //Ks 0.330000 0.330000 0.330000
                Materials["Wood"] = new BasicEffect(graphicsDevice)
                {
                    AmbientLightColor = Vector3.Zero,
                    DiffuseColor = new Vector3(0.666667f, 0.545098f, 0.356863f),
                    SpecularColor = new Vector3(0.330000f, 0.330000f, 0.330000f)
                };

                //newmtl Leafs
                //Ka 0.000000 0.000000 0.000000
                //Kd 0.270588 0.407843 0.400000
                //Ks 0.330000 0.330000 0.330000
                Materials["Leafs"] = new BasicEffect(graphicsDevice)
                {
                    AmbientLightColor = Vector3.Zero,
                    DiffuseColor = new Vector3(0.270588f, 0.407843f, 0.400000f),
                    SpecularColor = new Vector3(0.330000f, 0.330000f, 0.330000f)
                };

                foreach (var basicEffect in Materials.Values)
                    basicEffect.EnableDefaultLighting();
            }

            // Hardcoded for now
            public IDictionary<string, BasicEffect> Materials { get; } = new Dictionary<string, BasicEffect>();

            // PositionIndex, TextureCoordinateIndex, NormalIndex (all one based)
            public readonly IDictionary<string, IList<Tuple<int, int?, int?>[]>> MaterialFaces =
                new Dictionary<string, IList<Tuple<int, int?, int?>[]>>(StringComparer.OrdinalIgnoreCase);

            public IList<Vector3> NormalList { get; } = new List<Vector3>();

            public IList<Vector3> PositionList { get; } = new List<Vector3>();
        }
    }
}