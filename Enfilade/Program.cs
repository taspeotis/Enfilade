using Microsoft.Xna.Framework;

namespace Enfilade
{
    internal static class Program
    {
        public static void Main()
        {
            using (var enfiladeGame = new EnfiladeGame())
            using (var graphicsDeviceManager = new GraphicsDeviceManager(enfiladeGame))
            {
                graphicsDeviceManager.PreferMultiSampling = true;

                enfiladeGame.Run();
            }
        }
    }
}