using SFML.Graphics;
using SFML.Window;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CRenderShape : Component
    {
        private const float Divisor = 100;
        private static readonly Texture BlackTexture;
        private readonly CBody _cBody;
        private readonly GSGame _game;
        public Sprite _sprite;

        static CRenderShape() { BlackTexture = new Texture(new Image(16, 16, Color.Black)); }

        public CRenderShape(GSGame mGame, CBody mCBody)
        {
            _game = mGame;
            _cBody = mCBody;
        }

        public override void Added()
        {
            _sprite = new Sprite(BlackTexture);
            _game.OnDrawAfterCamera += Draw;
        }

        public override void Update(float mFrameTime)
        {
            var halfWidth = _cBody.HalfWidth/Divisor;
            var halfHeight = _cBody.HalfHeight/Divisor;

            _sprite.Position = new Vector2f(_cBody.Position.X/Divisor, _cBody.Position.Y/Divisor);
            _sprite.Scale = new Vector2f(halfWidth*2/16, halfHeight*2/16);
            _sprite.Origin = new Vector2f(_sprite.GetGlobalBounds().Width/2, _sprite.GetGlobalBounds().Height/2);
        }

        public override void Removed() { _game.OnDrawAfterCamera -= Draw; }

        private void Draw() { _game.GameWindow.RenderWindow.Draw(_sprite); }
    }
}