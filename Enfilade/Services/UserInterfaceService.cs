using System;
using System.ComponentModel.Composition;
using System.IO;
using Enfilade.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Enfilade.Services
{
    [Export(typeof (IUserInterfaceService))]
    public sealed class UserInterfaceService : IUserInterfaceService, IDisposable
    {
        private const int GlassPanelCornerHeight = 14;
        private const int GlassPanelCornerWidth = 14;

        private static readonly Point GlassPanelCornerSize =
            new Point(GlassPanelCornerWidth, GlassPanelCornerHeight);

        private readonly GraphicsDevice _graphicsDevice;
        private readonly Lazy<SpriteFont> _lazySpriteFont; 
        private readonly Lazy<Texture2D> _lazyGlassPanel;
        private readonly Lazy<Texture2D> _lazyGlassPanelCorners;

        [ImportingConstructor]
        public UserInterfaceService(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            _lazyGlassPanel = GetLazyTexture(Assets.Sprites.UserInterface.Panel.GlassPanel);
            _lazyGlassPanelCorners = GetLazyTexture(Assets.Sprites.UserInterface.Panel.GlassPanelCorners);

            _lazySpriteFont = new Lazy<SpriteFont>(isThreadSafe: false,
                valueFactory: () => contentManager.Load<SpriteFont>(Assets.Fonts.KenVectorFutureThin12));
        }

        public void Dispose()
        {
            if (_lazyGlassPanel.IsValueCreated)
                _lazyGlassPanel.Value.Dispose();

            if (_lazyGlassPanelCorners.IsValueCreated)
                _lazyGlassPanelCorners.Value.Dispose();
        }

        public void DrawGlassPanel(GlassStyle glassStyle, string panelTitle,
            Rectangle destinationRectangle, Action<SpriteBatch, Rectangle> spriteBatchAction)
        {
            using (var spriteBatch = new SpriteBatch(_graphicsDevice))
            {
                spriteBatch.Begin();

                // 50 px high minimum
                // 14 px corners
                DrawTopLeft(glassStyle.HasFlag(GlassStyle.TopLeftCorner), destinationRectangle, spriteBatch);
                DrawLeft(destinationRectangle, spriteBatch);
                DrawBottomLeft(glassStyle.HasFlag(GlassStyle.BottomLeftCorner), destinationRectangle, spriteBatch);

                DrawTop(destinationRectangle, spriteBatch);
                DrawMiddle(panelTitle, destinationRectangle, spriteBatch);
                DrawBottom(destinationRectangle, spriteBatch);

                DrawTopRight(glassStyle.HasFlag(GlassStyle.TopRightCorner), destinationRectangle, spriteBatch);
                DrawRight(destinationRectangle, spriteBatch);
                DrawBottomRight(glassStyle.HasFlag(GlassStyle.BottomRightCorner), destinationRectangle, spriteBatch);

                spriteBatch.End();
            }
        }

        private void DrawMiddle(string panelTitle, Rectangle destinationRectangle, SpriteBatch spriteBatch)
        {
            // draw the bg
            var rectangle = new Rectangle(
                destinationRectangle.Left + GlassPanelCornerWidth, 
                destinationRectangle.Top + GlassPanelCornerHeight,
                destinationRectangle.Width - 2*GlassPanelCornerWidth,
                destinationRectangle.Height - 2*GlassPanelCornerHeight);

            var sourceRectangle = new Rectangle(GlassPanelCornerWidth, GlassPanelCornerHeight, 1, 1);

            spriteBatch.Draw(_lazyGlassPanel.Value, rectangle, sourceRectangle, Color.White);

            if (!String.IsNullOrEmpty(panelTitle))
            {
                var spriteFont = _lazySpriteFont.Value;

                // draw the text
                var q = spriteFont.LineSpacing;
                var dims = spriteFont.MeasureString(panelTitle);

                var v2 = rectangle.Location.ToVector2();
                v2.Y -= 2;

                spriteBatch.DrawString(spriteFont, panelTitle, v2, new Color(46, 74, 95));
            }
        }

        private void DrawBottomLeft(bool cornerStyle, Rectangle destinationRectangle, SpriteBatch spriteBatch)
        {
            var rectangle = new Rectangle(
                destinationRectangle.Left, destinationRectangle.Bottom - GlassPanelCornerHeight,
                GlassPanelCornerWidth, GlassPanelCornerHeight);

            var texture = (cornerStyle ? _lazyGlassPanelCorners : _lazyGlassPanel).Value;

            var sourceRectangle = new Rectangle(
                0, texture.Height - GlassPanelCornerHeight,
                GlassPanelCornerWidth, GlassPanelCornerHeight);

            spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.White);
        }

        private Lazy<Texture2D> GetLazyTexture(string texturePath)
        {
            return new Lazy<Texture2D>(isThreadSafe: false, valueFactory: () =>
            {
                using (var fileStream = File.Open(texturePath, FileMode.Open, FileAccess.Read))
                    return Texture2D.FromStream(_graphicsDevice, fileStream);
            });
        }

        private void DrawLeft(Rectangle destinationRectangle, SpriteBatch spriteBatch)
        {
            var rectangle = new Rectangle(
                destinationRectangle.Left, destinationRectangle.Top + GlassPanelCornerHeight,
                GlassPanelCornerWidth, destinationRectangle.Height - 2*GlassPanelCornerHeight);

            var sourceRectangle = new Rectangle(0, GlassPanelCornerHeight, GlassPanelCornerWidth, 1);
            var texture = _lazyGlassPanel.Value;

            spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.White);
        }

        private void DrawTopRight(bool cornerStyle, Rectangle destinationRectangle, SpriteBatch spriteBatch)
        {
            var rectangle = new Rectangle(
                destinationRectangle.Right - GlassPanelCornerWidth, destinationRectangle.Top,
                GlassPanelCornerWidth, GlassPanelCornerHeight);

            var texture = (cornerStyle ? _lazyGlassPanelCorners : _lazyGlassPanel).Value;

            var sourceRectangle = new Rectangle(
                texture.Width - GlassPanelCornerWidth, 0,
                GlassPanelCornerWidth, GlassPanelCornerHeight);

            spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.White);
        }

        private void DrawBottomRight(bool cornerStyle, Rectangle destinationRectangle, SpriteBatch spriteBatch)
        {
            var rectangle = new Rectangle(
                destinationRectangle.Right - GlassPanelCornerWidth, destinationRectangle.Bottom - GlassPanelCornerHeight,
                GlassPanelCornerWidth, GlassPanelCornerHeight);

            var texture = (cornerStyle ? _lazyGlassPanelCorners : _lazyGlassPanel).Value;

            var sourceRectangle = new Rectangle(
                texture.Width - GlassPanelCornerWidth, texture.Height - GlassPanelCornerHeight,
                GlassPanelCornerWidth, GlassPanelCornerHeight);

            spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.White);
        }

        private void DrawTopLeft(bool cornerStyle, Rectangle destinationRectangle, SpriteBatch spriteBatch)
        {
            var rectangle = new Rectangle(destinationRectangle.Location, GlassPanelCornerSize);
            var sourceRectangle = new Rectangle(Point.Zero, GlassPanelCornerSize);
            var texture = (cornerStyle ? _lazyGlassPanelCorners : _lazyGlassPanel).Value;

            spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.White);
        }

        private void DrawTop(Rectangle destinationRectangle, SpriteBatch spriteBatch)
        {
            var rectangle = new Rectangle(
                destinationRectangle.Left + GlassPanelCornerWidth, destinationRectangle.Top,
                destinationRectangle.Width - 2*GlassPanelCornerWidth, GlassPanelCornerHeight);

            var sourceRectangle = new Rectangle(GlassPanelCornerWidth, 0, 1, GlassPanelCornerHeight);
            var texture = _lazyGlassPanel.Value;

            spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.White);
        }

        private void DrawBottom(Rectangle destinationRectangle, SpriteBatch spriteBatch)
        {
            var rectangle = new Rectangle(
                destinationRectangle.Left + GlassPanelCornerWidth, destinationRectangle.Bottom - GlassPanelCornerHeight,
                destinationRectangle.Width - 2*GlassPanelCornerWidth, GlassPanelCornerHeight);

            var texture = _lazyGlassPanel.Value;

            var sourceRectangle = new Rectangle(
                GlassPanelCornerWidth, texture.Height - GlassPanelCornerHeight,
                1, GlassPanelCornerHeight);

            spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.White);
        }

        private void DrawRight(Rectangle destinationRectangle, SpriteBatch spriteBatch)
        {
            var rectangle = new Rectangle(
                destinationRectangle.Right - GlassPanelCornerWidth, destinationRectangle.Top + GlassPanelCornerHeight,
                GlassPanelCornerWidth, destinationRectangle.Height - 2 * GlassPanelCornerHeight);

            var texture = _lazyGlassPanel.Value;

            var sourceRectangle = new Rectangle(
                texture.Width - GlassPanelCornerWidth, GlassPanelCornerHeight,
                GlassPanelCornerWidth, 1);

            spriteBatch.Draw(texture, rectangle, sourceRectangle, Color.White);
        }
    }
}