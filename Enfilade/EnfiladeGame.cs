using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Enfilade.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Enfilade
{
    internal sealed class EnfiladeGame : Game
    {
        private static readonly AssemblyCatalog AssemblyCatalog = new AssemblyCatalog(typeof (EnfiladeGame).Assembly);

        private CompositionContainer _compositionContainer;
        private IModelService _modelService;
        private IUserInterfaceService _userInterfaceService;
        private Model _largeOakDarkModel;
        private Model _plateGrassModel;
        private Model _plateRiverModel;

        protected override void BeginRun()
        {
            _compositionContainer = new CompositionContainer(AssemblyCatalog);

            try
            {
                _compositionContainer.ComposeExportedValue(this);
                _compositionContainer.ComposeExportedValue(Content);
                _compositionContainer.ComposeExportedValue(GraphicsDevice);

                _modelService = _compositionContainer.GetExportedValue<IModelService>();
                _userInterfaceService = _compositionContainer.GetExportedValue<IUserInterfaceService>();

                _largeOakDarkModel = _modelService.Load("Assets/Models/Large_Oak_Dark_01.obj");
                _plateGrassModel = _modelService.Load("Assets/Models/Plate_Grass_01.obj");
                _plateRiverModel = _modelService.Load("Assets/Models/Plate_River_01.obj");
            }
            catch
            {
                _compositionContainer.Dispose();
                _compositionContainer = null;

                throw;
            }

            base.BeginRun();
        }

        protected override void EndRun()
        {
            base.EndRun();

            _compositionContainer?.Dispose();
            _compositionContainer = null;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(63, 124, 182));

            var aspectRatio = GraphicsDevice.Viewport.AspectRatio;

            var rotationY = Matrix.CreateRotationY((float) gameTime.TotalGameTime.TotalSeconds/2);
            var world = Matrix.Multiply(rotationY, Matrix.Identity);
            var view = Matrix.CreateLookAt(new Vector3(3, 3, 3), Vector3.Zero, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 100);

            GraphicsDevice.RasterizerState = new RasterizerState {CullMode = CullMode.None};

            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //GraphicsDevice.BlendState = BlendState.Opaque;

            //_largeOakDarkModel.Draw(Matrix.Identity, view, projection);

            _plateGrassModel.Draw(world, view, projection);

            world = Matrix.CreateTranslation(0, 0, -3);
            world = Matrix.Multiply(rotationY, world);

            _plateRiverModel.Draw(world, view, projection);

            world = Matrix.CreateTranslation(0, 0, -6);
            world = Matrix.Multiply(rotationY, world);

            _plateRiverModel.Draw(world, view, projection);

            


            /*
            // draw all the stuff
            var destinationRectangle = new Rectangle(8, 8, 83, 42);

            _userInterfaceService.DrawGlassPanel(GlassStyle.TopLeftCorner, "FPS", destinationRectangle,
                (spriteBatch, rectangle, spriteFont) =>
                {
                    var totalSeconds = gameTime.ElapsedGameTime.TotalSeconds;

                    if (totalSeconds > 0)
                    {
                        var position = rectangle.Location.ToVector2();

                        position.X += 3;
                        position.Y -= 2;

                        spriteBatch.DrawString(spriteFont, $"{1/totalSeconds:F0}", position, Color.Pink);
                    }
                });*/

//            base.Draw(gameTime);
        }
    }
}