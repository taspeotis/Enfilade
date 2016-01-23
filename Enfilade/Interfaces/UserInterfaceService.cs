using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Enfilade.Interfaces
{
    public interface IUserInterfaceService
    {
        void DrawGlassPanel(GlassStyle glassStyle, string panelTitle,
            Rectangle destinationRectangle, Action<SpriteBatch, Rectangle> spriteBatchAction);
    }
}