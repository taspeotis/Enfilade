using Microsoft.Xna.Framework;

namespace Enfilade
{
    internal static class Program
    {
        public static void Main()
        {
            using (var enfiladeGame = new EnfiladeGame())
            using (new GraphicsDeviceManager(enfiladeGame))
                enfiladeGame.Run();
        }
    }
}