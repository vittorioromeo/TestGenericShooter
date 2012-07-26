using SFML.Window;
using SFMLStart.Utilities;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CControl : Component
    {
        private readonly CBody _cBody;
        private readonly CMovement _cMovement;
        private readonly CTargeter _cTargeter;
        private readonly GSGame _game;
        private float _fireDelay;

        public CControl(GSGame mGame, CBody mCBody, CMovement mCMovement, CTargeter mCTargeter)
        {
            _game = mGame;
            _cBody = mCBody;
            _cMovement = mCMovement;
            _cTargeter = mCTargeter;
        }

        public override void Update(float mFrameTime)
        {
            _fireDelay -= 1*mFrameTime;

            var nextAction = _game.NextAction;
            if (_fireDelay < 0 && nextAction == 1)
            {
                var fireAngle = _cTargeter.GetDegreesTowards((int) _game.GameWindow.Camera.MousePosition.X*100, (int) _game.GameWindow.Camera.MousePosition.Y*100);
                var bullet = _game.Factory.Bullet(_cBody.Position.X, _cBody.Position.Y, fireAngle);
                bullet.AddTags("playerbullet");
                _fireDelay = 4;
            }

            const int speed = 400;
            var nextX = _game.NextX;
            var nextY = _game.NextY;

            if (nextX == 0 && nextY == 0)
            {
                _cMovement.Stop();
                return;
            }

            var angle = Utils.Math.Vectors.ToAngleDegrees(new Vector2f(_game.NextX, _game.NextY));
            _cMovement.MoveTowardsAngle(angle, speed);
        }
    }
}