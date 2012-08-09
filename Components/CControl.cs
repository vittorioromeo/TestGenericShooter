using SFML.Window;
using SFMLStart.Data;
using SFMLStart.Utilities;
using SFMLStart.Vectors;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CControl : Component
    {
        private readonly CBody _cBody;
        private readonly CMovement _cMovement;
        private readonly CRender _cRender;
        private readonly CTargeter _cTargeter;
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
                var fireAngle = Utils.Math.Angles.TowardsDegrees(new SSVector2F(_cBody.Position.X, _cBody.Position.Y), new SSVector2F(_game.GameWindow.Camera.MousePosition.X * 100, _game.GameWindow.Camera.MousePosition.Y * 100));
                _game.Factory.Bullet(_cBody.Position.X, _cBody.Position.Y, fireAngle, 500, false);
                _fireDelay = 4;

                _cRender.Animation = Assets.GetAnimation("charfiring");
            }

            if (nextAction == 0) _cRender.Animation = Assets.GetAnimation("charidle");

            if (nextX == 0 && nextY == 0)
            {
                _cMovement.Stop();
                return;
            }

            var angle = Utils.Math.Vectors.ToAngleDegrees(new SSVector2F(_game.NextX, _game.NextY));
            _cMovement.Angle = angle;
            _cMovement.Speed = speed;
        }
    }
}