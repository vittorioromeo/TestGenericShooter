using SFMLStart;
using SFMLStart.Data;

namespace TestGenericShooter
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Settings.Framerate.Limit = 60;
            Settings.Framerate.IsLimited = false;
            Settings.Frametime.StaticValue = 1.5f;
            Settings.Frametime.IsStatic = false;

            var game = new GSGame();
            var window = new GameWindow(320, 240, 3);

            window.SetGame(game);
            window.Run();
        }
    }
}