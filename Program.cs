using SFMLStart;
using SFMLStart.Data;

namespace TestGenericShooter
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Settings.Framerate.IsLimited = false;
            Settings.Framerate.Limit = 60;

            var game = new GSGame();
            var window = new GameWindow(320, 240, 3);

            window.SetGame(game);
            window.Run();
        }
    }
}