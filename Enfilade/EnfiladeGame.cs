using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Enfilade.Interfaces;
using Microsoft.Xna.Framework;

namespace Enfilade
{
    internal sealed class EnfiladeGame : Game
    {
        private static readonly AssemblyCatalog AssemblyCatalog = new AssemblyCatalog(typeof (EnfiladeGame).Assembly);

        private CompositionContainer _compositionContainer;
        private IModelService _modelService;
        private IUserInterfaceService _userInterfaceService;

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

        private float _y = -5 ;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(63, 124, 182));

            var model = _modelService.Load("Assets/Models/Large_Oak_Dark_01.obj");

            var aspectRatio = GraphicsDevice.Viewport.AspectRatio;

            var world = Matrix.Identity;
            var view = Matrix.CreateLookAt(new Vector3(10, 10, 10), Vector3.Zero, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 100);

            var originalState = GraphicsDevice.RasterizerState;

            try
            {
                //GraphicsDevice.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame };

                model.Draw(world, view, projection);
            }
            finally
            {
                GraphicsDevice.RasterizerState = originalState;
            }

            /*
            var vertexBuffer = new VertexBuffer(GraphicsDevice, typeof (VertexPositionColor), 3, BufferUsage.None);

            var random = new Random();

            vertexBuffer.SetData(new[]
            {
                new VertexPositionColor(new Vector3(random.Next(-10, 10), random.Next(-10, 10), random.Next(-10, 10)), Color.Pink),
                new VertexPositionColor(new Vector3(random.Next(-10, 10), random.Next(-10, 10), random.Next(-10, 10)), Color.White),
                new VertexPositionColor(new Vector3(random.Next(-10, 10), random.Next(-10, 10), random.Next(-10, 10)), Color.Green)
            });

            var indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, 3, BufferUsage.None);

            indexBuffer.SetData(new[] {0, 1, 2});

            var modelBone = new ModelBone {Transform = Matrix.Identity};
            var basicEffect = new BasicEffect(GraphicsDevice) {VertexColorEnabled = true};
            var modelMeshPart = new ModelMeshPart {IndexBuffer = indexBuffer, NumVertices = 3, PrimitiveCount = 1, VertexBuffer = vertexBuffer};
            var modelMesh = new ModelMesh(GraphicsDevice, new List<ModelMeshPart> {modelMeshPart})
            {
                ParentBone = modelBone,
                Effects = new ModelEffectCollection(new List<Effect> {basicEffect})
            };

            modelMeshPart.Effect = basicEffect;


            var model = new Model(GraphicsDevice, new List<ModelBone> {modelBone}, new List<ModelMesh> {modelMesh});

            

            var originalState = GraphicsDevice.RasterizerState;

            try
            {
                GraphicsDevice.RasterizerState = new RasterizerState {FillMode = FillMode.WireFrame};

                model.Draw(world, view, projection);
            }
            finally
            {
                GraphicsDevice.RasterizerState = originalState;
            }

    */
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
                });

            base.Draw(gameTime);
        }
    }
}