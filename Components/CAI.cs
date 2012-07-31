using SFMLStart.Data;
using SFMLStart.Utilities;
using SFMLStart.Utilities.Timelines;
using TestGenericShooter.Resources;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CAI : Component
    {
        private readonly CBody _cBody;
        private readonly CMovement _cMovement;
        private readonly CTargeter _cTargeter;
        private readonly GSGame _game;
        private float _angle;
        private float _replicationDelay;
        private float _rotationSpeed;
        private float _shootDelay;
        private float _speed;
        private Timeline _timeline;
        private readonly CRender _cRender;
        private bool _big;
        private float _shootDelayMax = 90;
        private float _speedMax = 100;


        public CAI(GSGame mGame, CBody mCBody, CMovement mCMovement, CTargeter mCTargeter, CRender mCRender, bool mBig)
        {
            _game = mGame;
            _cBody = mCBody;
            _cTargeter = mCTargeter;
            _cMovement = mCMovement;
            _cRender = mCRender;
            _shootDelay = Utils.RandomGenerator.GetNextInt(0, 80);
            _big = mBig;
            if (_big) _shootDelayMax = 150;
            if (_big) _speedMax = 60;

            mCBody.OnCollision += (mFrameTime, mEntity, mBody) =>
                                  {
                                      if (!mEntity.HasTag(Tags.Wall)) return;
                                      _angle += 180;
                                      _speed = Utils.RandomGenerator.GetNextInt(0, 100);
                                      _rotationSpeed = 0;
                                  };
        }

        public override void Update(float mFrameTime)
        {
            Utils.Math.Angles.WrapDegrees(_angle);

            _cTargeter.FindTarget(Tags.Player);
            var playerPosition = _cTargeter.TargetPosition.Position;
            var targetAngle = _cTargeter.GetDegreesTowards(playerPosition.X, playerPosition.Y);

            if (_speed < _speedMax) _speed += 3 * mFrameTime;
            if (_rotationSpeed < 0.2f) _rotationSpeed += 0.002f*mFrameTime + (float) Utils.RandomGenerator.GetNextDouble() / 1000f;

            _shootDelay += mFrameTime + (float)Utils.RandomGenerator.GetNextDouble() / 100f;
            //_replicationDelay += mFrameTime;

            if (_shootDelay > _shootDelayMax)
            {
                _timeline = new Timeline();

                if (!_big)
                {
                    _timeline.AddCommand(new Do(() =>
                                                {
                                                    _game.Factory.BulletBlack(_cBody.Position.X, _cBody.Position.Y, targetAngle, 300);
                                                    _speed = 25;
                                                }));
                    _timeline.AddCommand(new Wait(3));
                    _timeline.AddCommand(new Do(() => _game.Factory.BulletBlack(_cBody.Position.X, _cBody.Position.Y, targetAngle + Utils.RandomGenerator.GetNextInt(-10, 10), 250)));
                    _timeline.AddCommand(new Wait(3));
                    _timeline.AddCommand(new Do(() => _game.Factory.BulletBlack(_cBody.Position.X, _cBody.Position.Y, targetAngle + Utils.RandomGenerator.GetNextInt(-10, 10), 250)));
                }
                else
                {
                    _timeline.AddCommand(new Do(() =>
                                                {
                                                    _speed = 0;
                                                    _game.Factory.BigBulletBlack(_cBody.Position.X, _cBody.Position.Y, targetAngle + Utils.RandomGenerator.GetNextInt(-25, 25), 250);
                                                    _game.Factory.BulletBlack(_cBody.Position.X, _cBody.Position.Y, Utils.RandomGenerator.GetNextInt(0, 360), 250);
                                                    _game.Factory.BulletBlack(_cBody.Position.X, _cBody.Position.Y, Utils.RandomGenerator.GetNextInt(0, 360), 250);
                                                }));
                    _timeline.AddCommand(new Wait(2));
                    _timeline.AddCommand(new Goto(0, 15));
                }
                _timeline.AddCommand(new Wait(3));
                _timeline.AddCommand(new Do(() => _cRender.Animation = Assets.Animations["charidle"]));
                _shootDelay = 0;             
            }

            if (!_big && _shootDelay > 50) _cRender.Animation = Assets.Animations["charfiring"];
            if (_big && _shootDelay > 110) _cRender.Animation = Assets.Animations["charfiring"];

            if (_replicationDelay > 50)
            {
                _replicationDelay = 0;
                _game.Factory.Enemy(_cBody.Position.X, _cBody.Position.Y);
            }

            _angle = Utils.Math.Angles.RotateTowardsAngleDegrees(_angle, targetAngle, _rotationSpeed);

            _cMovement.MoveTowardsAngle(_angle, (int) _speed);
            if (_timeline != null) _timeline.Update(mFrameTime);
        }
    }
}