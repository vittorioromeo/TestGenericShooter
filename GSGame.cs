using System;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using SFMLStart;
using SFMLStart.Data;
using SFMLStart.Utilities;
using TestGenericShooter.Components;
using TestGenericShooter.Resources;
using VeeCollision;
using VeeEntity;

namespace TestGenericShooter
{
    public class GSGame : Game
    {
        private readonly Manager _manager;
        private readonly World _world;

        public GSGame()
        {
            _manager = new Manager();
            _world = new World(Groups.GroupArray, 21, 16, 1600);
            Factory = new Factory(this, _manager, _world);

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

            Bind("zoomin", 0, () => GameWindow.Camera.Zoom(1.001f), null, new KeyCombination(Keyboard.Key.N));
            Bind("zoomout", 0, () => GameWindow.Camera.Zoom(0.999f), null, new KeyCombination(Keyboard.Key.M));

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

            var mapxd = new[]
                          {
                              "11111111111111111111",
                              "10000000000000000001",
                              "10011000000000000001",
                              "10011000004000000001",
                              "10011000111111000001",
                              "10011001111111101001",
                              "10011001000000101001",
                              "10011001000200101001",
                              "10000000000000000001",
                              "10000000000000000001",
                              "10000011111111110001",
                              "10000011111111110001",
                              "10000000000000000001",
                              "10000000000000000001",
                              "11111111111111111111"
                          };

            var map = new[]
                          {
                              "11111111111111111111",
                              "14400000000000000001",
                              "14401000100001000001",
                              "14401000000000000001",
                              "14401000000011100001",
                              "10001002000011100001",
                              "10001000000011100001",
                              "10000000000011100001",
                              "10000000000011100001",
                              "10010010000011100001",
                              "10000000000000000131",
                              "10001111110000000131",
                              "10001111110000100131",
                              "10000000000000000031",
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
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.Random.Next(0, 360), Utils.Random.Next(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.Random.Next(0, 360), Utils.Random.Next(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.Random.Next(0, 360), Utils.Random.Next(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.Random.Next(0, 360), Utils.Random.Next(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.Random.Next(0, 360), Utils.Random.Next(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.Random.Next(0, 360), Utils.Random.Next(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.Random.Next(0, 360), Utils.Random.Next(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.Random.Next(0, 360), Utils.Random.Next(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.Random.Next(0, 360), Utils.Random.Next(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.Random.Next(0, 360), Utils.Random.Next(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.Random.Next(0, 360), Utils.Random.Next(0, 25), false);
                        Factory.Bullet(8.ToUnits() + 16.ToUnits()*iX, 8.ToUnits() + 16.ToUnits()*iY, Utils.Random.Next(0, 360), Utils.Random.Next(0, 25), false);
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
            //var map = _world.GetObstacleMap(Groups.Obstacle);

            if (!_manager.HasEntityByTag(Tags.Friendly))
                foreach (var cPurification in _manager.GetEntitiesByTag(Tags.Purifiable).Select(x => x.GetComponentUnSafe<CPurification>())) cPurification.Purifying = false;
            if (!_manager.HasEntityByTag(Tags.Enemy))
                foreach (var cPurification in _manager.GetEntitiesByTag(Tags.Purifiable).Select(x => x.GetComponentUnSafe<CPurification>())) cPurification.Purifying = true;
        }

  
    }
}