using SFML.Graphics;
using SFML.Window;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CRenderShape : Component
    {
        private const float Divisor = 100;
        private readonly CBody _cBody;
        private readonly GSGame _game;
        private RectangleShape _shape;

        public CRenderShape(GSGame mGame, CBody mCBody)
        {
            _game = mGame;
            _cBody = mCBody;
        }

        public override void Added()
        {
            var halfWidth = _cBody.HalfWidth/Divisor;
            var halfHeight = _cBody.HalfHeight/Divisor;

            _shape = new RectangleShape(new Vector2f(halfWidth*2, halfHeight*2))
                     {
                         Origin = new Vector2f(halfWidth, halfHeight),
                         FillColor = Color.Black
                     };
            _game.OnDrawAfterCamera += Draw;
        }

        public override void Update(float mFrameTime)
        {
            var halfWidth = _cBody.HalfWidth/Divisor;
            var halfHeight = _cBody.HalfHeight/Divisor;

            _shape.Position = new Vector2f(_cBody.Position.X/Divisor, _cBody.Position.Y/Divisor);
            _shape.Origin = new Vector2f(halfWidth, halfHeight);
            _shape.Size = new Vector2f(halfWidth*2, halfHeight*2);
        }

        public override void Removed() { _game.OnDrawAfterCamera -= Draw; }

        private void Draw() { _game.GameWindow.RenderWindow.Draw(_shape); }
    }
}