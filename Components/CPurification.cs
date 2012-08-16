using SFML.Graphics;
using SFML.Window;
using SFMLStart.Data;
using SFMLStart.Utilities;
using SFMLStart.Vectors;
using TestGenericShooter.Resources;
using VeeCollision;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CPurification : Component
    {
        private readonly CRender _cRender;
        private readonly GSGame _game;
        private float _purificationValue;
        private int _shootDelay;
        private Sprite _sprite;

        public CPurification(GSGame mGame, CRender mCRender)
        {
            _game = mGame;
            _cRender = mCRender;
            _shootDelay = Utils.RandomGenerator.GetNextInt(0, 100);
        }

        public bool Purifying { get; set; }
        public bool Purified { get { return _purificationValue > 125; } }

        public override void Added()
        {
            _sprite = _cRender.Sprite;
            _sprite.Texture = Assets.GetTexture(Textures.WallWhite);
            _sprite.Color = Color.Black;
        }
        public override void Update(float mFrameTime)
        {
            if (Purifying) _purificationValue += 2*mFrameTime;
            else _purificationValue -= 2*mFrameTime;

            _purificationValue = Utils.Math.Clamp(_purificationValue, 255, 0);
            var next = (byte) _purificationValue;
            _sprite.Color = new Color(next, next, next, 255);

            _shootDelay++;
            if (_shootDelay <= 100) return;

            var position = Entity.GetComponentUnSafe<CBody>().Position;

            if (Utils.RandomGenerator.GetNextInt(0, 100) > 98)
            {
                var angle = Utils.RandomGenerator.GetNextInt(0, 360);
                var orbit = Utils.Math.Vectors.OrbitDegrees(new SSVector2F(position.X, position.Y), angle, 1600);
                _game.Factory.Spore((int) orbit.X, (int) orbit.Y, angle, 100, !Purified);
            }

            _shootDelay = 0;
        }
    }
}