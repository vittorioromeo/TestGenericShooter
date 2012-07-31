using SFML.Window;
using SFMLStart.Data;
using SFMLStart.Utilities;
using TestGenericShooter.Resources;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CControl : Component
    {
        private readonly CBody _cBody;
        private readonly CMovement _cMovement;
        private readonly CTargeter _cTargeter;
        private readonly CRender _cRender;
        private readonly GSGame _game;
        private float _fireDelay;

        public CControl(GSGame mGame, CBody mCBody, CMovement mCMovement, CTargeter mCTargeter, CRender mCRender)
        {
            _game = mGame;
            _cBody = mCBody;
            _cMovement = mCMovement;
            _cTargeter = mCTargeter;
            _cRender = mCRender;
        }

        public override void Update(float mFrameTime)
        {
            const int speed = 170;
            var nextX = _game.NextX;
            var nextY = _game.NextY;
            var nextAction = _game.NextAction;

            _fireDelay -= 1*mFrameTime;

            if (_fireDelay < 0 && nextAction == 1)
            {
                var fireAngle = _cTargeter.GetDegreesTowards((int) _game.GameWindow.Camera.MousePosition.X*100, (int) _game.GameWindow.Camera.MousePosition.Y*100);
                var bullet = _game.Factory.BulletWhite(_cBody.Position.X, _cBody.Position.Y, fireAngle, 500);
                bullet.AddTags(Tags.BulletWhite);
                _fireDelay = 4;

                _cRender.Animation = Assets.Animations["charfiring"];
            }
            
            if(nextAction == 0) _cRender.Animation = Assets.Animations["charidle"];

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