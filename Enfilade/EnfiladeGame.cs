using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using Enfilade.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Enfilade
{
    internal sealed class EnfiladeGame : Game
    {
        private static readonly AssemblyCatalog AssemblyCatalog = new AssemblyCatalog(typeof (EnfiladeGame).Assembly);

        private CompositionContainer _compositionContainer;
        private IUserInterfaceService _userInterfaceService;

        protected override void BeginRun()
        {
            base.BeginRun();

            _compositionContainer = new CompositionContainer(AssemblyCatalog);

            try
            {
                _compositionContainer.ComposeExportedValue(this);
                _compositionContainer.ComposeExportedValue(Content);
                _compositionContainer.ComposeExportedValue(GraphicsDevice);

                _userInterfaceService = _compositionContainer.GetExportedValue<IUserInterfaceService>();
            }
            catch
            {
                _compositionContainer.Dispose();
                _compositionContainer = null;

                throw;
            }
        }

        protected override void EndRun()
        {
            base.EndRun();

            _compositionContainer?.Dispose();
            _compositionContainer = null;
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var spriteFont = Content.Load<SpriteFont>(Assets.Fonts.KenVectorFutureThin12);

            GraphicsDevice.Clear(new Color(63, 124, 182));

            DrawShields();

            using (var y = new SpriteBatch(GraphicsDevice))
            {
                y.Begin();

                var totalSeconds = gameTime.ElapsedGameTime.TotalSeconds;

                if (totalSeconds > 0)
                    y.DrawString(spriteFont, $"Frames Per Second: {1/totalSeconds:F0}", new Vector2(50, 50), Color.Pink);

                y.End();
            }
        }

        private void DrawShields()
        {
            _userInterfaceService.DrawGlassPanel(GlassStyle.TopLeftCorner, "FPS", new Rectangle(8, 8, 250, 42), null);
            return;
            // 50 high
            // 14x14 corners
            var destination = new Rectangle(0, 0, 25, 25);
            var source = new Rectangle(0, 0, 25, 25);

            // TODO: One of these for the user interface
            using (var spriteBatch = new SpriteBatch(GraphicsDevice))
            {
                spriteBatch.Begin();

                const string path = "Assets/Sprites/UserInterface/glassPanel_cornerTL.png";

                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                using (var texture = Texture2D.FromStream(GraphicsDevice, stream))
                {
                    spriteBatch.Draw(texture, destination, source, Color.White);

                    spriteBatch.End();
                }
            }
        }
    }
}