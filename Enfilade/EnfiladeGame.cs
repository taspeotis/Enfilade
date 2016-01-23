using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Enfilade
{
    internal sealed class EnfiladeGame : Game
    {
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var spriteFont = Content.Load<SpriteFont>(Assets.Fonts.KenVectorFutureThin18);

            GraphicsDevice.Clear(Color.Blue);

            using (var y = new SpriteBatch(GraphicsDevice))
            {
                y.Begin();

                var totalSeconds = gameTime.ElapsedGameTime.TotalSeconds;

                if (totalSeconds > 0)
                    y.DrawString(spriteFont, $"Frames Per Second: {1/totalSeconds:F0}", new Vector2(50, 50), Color.Pink);

                y.End();
            }
        }
    }
}