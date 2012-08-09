using System;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using SFMLStart;
using SFMLStart.Data;
using SFMLStart.Utilities;
using TestGenericShooter.Components;
using TestGenericShooter.Resources;
using TestGenericShooter.SpatialHash;
using VeeEntitySystem2012;

namespace TestGenericShooter
{
    public class GSGame : Game
    {
        private readonly Manager _manager;
        private readonly PhysicsWorld _physicsWorld;

        public GSGame()
        {
            _manager = new Manager();
            _physicsWorld = new PhysicsWorld(Groups.GroupArray, 21, 16, 1600);
            Factory = new Factory(this, _manager, _physicsWorld);

            OnUpdate += _manager.Update;
            OnUpdate += UpdateInputs;
            OnUpdate += UpdatePurification;
            OnUpdate += mFrameTime => GameWindow.RenderWindow.SetTitle(((int) GameWindow.FPS).ToString());

            OnDrawBeforeCamera += () => GameWindow.RenderWindow.Clear(new Color(125, 125, 185));

            InitializeInputs();
            NewGame();
        }

        public Factory Factory { get; private set; }
        public int NextX { get; private set; }
        public int NextY { get; private set; }
        public int NextAction { get; private set; }

        private void InitializeInputs()
        {
            Bind("quit", 0, () => Environment.Exit(0), null, new KeyCombination(Keyboard.Key.Escape));

            Bind("w", 0, () => NextY = -1, null, new KeyCombination(Keyboard.Key.W));
            Bind("s", 0, () => NextY = 1, null, new KeyCombination(Keyboard.Key.S));
            Bind("a", 0, () => NextX = -1, null, new KeyCombination(Keyboard.Key.A));
            Bind("d", 0, () => NextX = 1, null, new KeyCombination(Keyboard.Key.D));

            Bind("fire", 0, () => NextAction = 1, null, new KeyCombination(Mouse.Button.Left));
        }
        private void NewGame()
        {
            _manager.Clear();
            DebugLevel();
        }

        private void DebugLevel()
        {
            const int sizeX = 20;
            const int sizeY = 15;
            
            var map = new[]
                          {
                              "11111111111111111111",
                              "16633333777744444551",
                              "16633331777714444551",
                              "16633331777714444551",
                              "16633333777744444551",
                              "16633333777744444551",
                              "16633333777744444551",
                              "16633331777714444551",
                              "16633331777714444551",
                              "16633333777744444551",
                              "16633333777744444551",
                              "16633331777714444551",
                              "16633331777714444551",
                              "16633333777744444551",
                              "11111111111111111111"
                          };

            var map3 = new[]
                       {
                           "11111111111111111111",
                           "10000100000000000601",
                           "10200100000040000041",
                           "10030100400001000001",
                           "10111100000000000001",
                           "10000000001000011001",
                           "10000000111100000001",
                           "11771110001000001001",
                           "10003000111100011001",
                           "10401000010000000001",
                           "10000000000000113001",
                           "10500111110000110001",
                           "10400007000600770501",
                           "10004007004000770001",
                           "11111111111111111111"
                       };


            var maphdhdf = new[]
                         {
                             "11111111111111111111",
                             "16633000000000044551",
                             "16633000011000044551",
                             "16633000011000044551",
                             "16633000000000044551",
                             "16633111000011144551",
                             "16633111000011144551",
                             "16633111000011144551",
                             "16633111000011144551",
                             "16633111000011144551",
                             "16633000000000044551",
                             "16633000011000044551",
                             "16633000011000044551",
                             "16633000000000044551",
                             "11111111111111111111"
                         };

            var maper = new[]
                      {
                          "11111111111111111111",
                          "10000000000000000001",
                          "10200000000000000001",
                          "10000000000000000001",
                          "10001111000011110001",
                          "10001111000011110001",
                          "10001111000011110001",
                          "10000000000000000001",
                          "10000000000000000001",
                          "10001111000011110001",
                          "10001111000011110001",
                          "10001111000011110001",
                          "10044444444444444401",
                          "10044444444444444401",
                          "11111111111111111111"
                      };

            for (var iY = 0; iY < sizeY; iY++)
                for (var iX = 0; iX < sizeX; iX++)
                {
                    if (map.IsValue(iX, iY, 2))
                        Factory.Player(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY);

                    if (map.IsValue(iX, iY, 1))
                        Factory.Wall(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, map.CalculateWall(iX, iY));

                    if (map.IsValue(iX, iY, 3))
                        Factory.Friendly(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY);

                    if (map.IsValue(iX, iY, 4))
                        Factory.Enemy(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY);

                    if (map.IsValue(iX, iY, 5))
                        Factory.BigEnemy(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY);

                    if (map.IsValue(iX, iY, 6))
                        Factory.BigFriendly(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY);

                    if (map.IsValue(iX, iY, 7))
                        Factory.BreakableWall(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY);

                    if (map.IsValue(iX, iY, 9))
                    {
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.RandomGenerator.GetNextInt(0, 360), Utils.RandomGenerator.GetNextInt(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.RandomGenerator.GetNextInt(0, 360), Utils.RandomGenerator.GetNextInt(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.RandomGenerator.GetNextInt(0, 360), Utils.RandomGenerator.GetNextInt(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.RandomGenerator.GetNextInt(0, 360), Utils.RandomGenerator.GetNextInt(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.RandomGenerator.GetNextInt(0, 360), Utils.RandomGenerator.GetNextInt(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.RandomGenerator.GetNextInt(0, 360), Utils.RandomGenerator.GetNextInt(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.RandomGenerator.GetNextInt(0, 360), Utils.RandomGenerator.GetNextInt(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.RandomGenerator.GetNextInt(0, 360), Utils.RandomGenerator.GetNextInt(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.RandomGenerator.GetNextInt(0, 360), Utils.RandomGenerator.GetNextInt(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.RandomGenerator.GetNextInt(0, 360), Utils.RandomGenerator.GetNextInt(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.RandomGenerator.GetNextInt(0, 360), Utils.RandomGenerator.GetNextInt(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.RandomGenerator.GetNextInt(0, 360), Utils.RandomGenerator.GetNextInt(0, 25), false);
                    }

                    if (iX <= 0 || iY <= 0 || iX >= sizeX - 1 || iY >= sizeY - 1) continue;
                    if (!map.IsValue(iX, iY, 0)) continue;

                    if (map.IsValue(iX - 1, iY, 1))
                        if (map.IsValue(iX, iY - 1, 1))
                            Factory.Decoration(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Textures.WallBlack, Tilesets.Wall, "icurve");

                    if (map.IsValue(iX + 1, iY, 1))
                        if (map.IsValue(iX, iY - 1, 1))
                            Factory.Decoration(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Textures.WallBlack, Tilesets.Wall, "icurve", 90);

                    if (map.IsValue(iX + 1, iY, 1))
                        if (map.IsValue(iX, iY + 1, 1))
                            Factory.Decoration(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Textures.WallBlack, Tilesets.Wall, "icurve", 180);

                    if (map.IsValue(iX - 1, iY, 1))
                        if (map.IsValue(iX, iY + 1, 1))
                            Factory.Decoration(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Textures.WallBlack, Tilesets.Wall, "icurve", 270);
                }
        }

        private void UpdateInputs(float mFrameTime) { NextX = NextY = NextAction = 0; }
        private void UpdatePurification(float mFrameTime)
        {
            //var map = _physicsWorld.GetObstacleMap(Groups.Obstacle);

            if (!_manager.HasEntityByTag(Tags.Friendly))
                foreach (var cPurification in _manager.GetEntitiesByTag(Tags.Purifiable).Select(x => x.GetComponent<CPurification>())) cPurification.Purifying = false;
            if (!_manager.HasEntityByTag(Tags.Enemy))
                foreach (var cPurification in _manager.GetEntitiesByTag(Tags.Purifiable).Select(x => x.GetComponent<CPurification>())) cPurification.Purifying = true;
        }
    }
}