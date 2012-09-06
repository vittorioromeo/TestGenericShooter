using System;
using SFML.Window;
using SFMLStart.Data;
using SFMLStart.Utilities;
using SFMLStart.Utilities.Timelines;
using TestGenericShooter.Resources;
using VeeCollision;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CAI : Component
    {
        private readonly bool _big;
        private readonly CBody _cBody;
        private readonly CMovement _cMovement;
        private readonly CRender _cRender;
        private readonly CTargeter _cTargeter;
        private readonly GSGame _game;
        private readonly float _shootDelayMax = 90;
        private readonly float _speedMax = 100;
        private float _angle;
        private float _rotationSpeed;
        private float _shootDelay;
        private float _speed;
        private Timeline _timeline;

        public CAI(GSGame mGame, CBody mCBody, CMovement mCMovement, CTargeter mCTargeter, CRender mCRender, bool mBig)
        {
            _game = mGame;
            _cBody = mCBody;
            _cTargeter = mCTargeter;
            _cMovement = mCMovement;
            _cRender = mCRender;
            _shootDelay = Utils.Random.Next(0, 80);
            _big = mBig;

            if (_big)
            {
                _shootDelayMax = 150;
                _speedMax = 60;
            }

            mCBody.OnCollision += (mCollisionInfo) =>
                                  {
                                      var entity = (Entity)mCollisionInfo.UserData;

                                      if (entity.HasTag(Tags.Wall))
                                      {
                                          _angle += 180 + Utils.Random.Next(-45, 45);
                                          _speed = Utils.Random.Next(0, 100);
                                          _rotationSpeed = 0.002f;
                                      }
                                  };
        }

        public bool Friendly { get; set; }

        public override void Update(float mFrameTime)
        {          
            if (_speed < _speedMax) _speed += 3*mFrameTime;
            if (_rotationSpeed < 0.2f) _rotationSpeed += 0.002f * mFrameTime + (float)Utils.Random.NextDouble() / 1000f;

            if (_cTargeter.Target != null)
            {
                var targetAngle = _cTargeter.GetDegreesTowardsTarget();
                _shootDelay += mFrameTime + (float) Utils.Random.NextDouble()/100f;
                if (_shootDelay > _shootDelayMax)
                {
                    _timeline = new Timeline();

                    if (!_big)
                    {
                        _timeline.AddCommand(new Do(() => _game.Factory.Bullet(_cBody.Position.X, _cBody.Position.Y, targetAngle, 300, !Friendly)));
                        _timeline.AddCommand(new Wait(3));
                        _timeline.AddCommand(new Do(() => _game.Factory.Bullet(_cBody.Position.X, _cBody.Position.Y, targetAngle + Utils.Random.Next(-10, 10), 300, !Friendly)));
                        _timeline.AddCommand(new Wait(3));
                        _timeline.AddCommand(new Do(() => _game.Factory.Bullet(_cBody.Position.X, _cBody.Position.Y, targetAngle + Utils.Random.Next(-10, 10), 300, !Friendly)));
                    }
                    else
                    {                   
                        _timeline.AddCommand(new Do(() =>
                                                    {
                                                        _game.Factory.BigBullet(_cBody.Position.X, _cBody.Position.Y, targetAngle + Utils.Random.Next(-25, 25), 250, !Friendly);
                                                        _game.Factory.Bullet(_cBody.Position.X, _cBody.Position.Y, Utils.Random.Next(0, 360), 250, !Friendly);
                                                        _game.Factory.Bullet(_cBody.Position.X, _cBody.Position.Y, Utils.Random.Next(0, 360), 250, !Friendly);
                                                        _speed = 0;
                                                    }));
                        _timeline.AddCommand(new Wait(2));
                        _timeline.AddCommand(new Goto(0, 15));
                    }
                    _timeline.AddCommand(new Wait(3));
                    _timeline.AddCommand(new Do(() => _cRender.Animation = Assets.GetAnimation("charidle")));
                    _shootDelay = 0;
                }
                if (_shootDelay > _shootDelayMax - 30) _cRender.Animation = Assets.GetAnimation("charfiring");
                _angle = Utils.Math.Angles.RotateTowardsAngleDegrees(_angle, targetAngle, _rotationSpeed);
            }

            _cMovement.Angle = _angle;
            _cMovement.Speed = (int) _speed;
            if (_timeline != null) _timeline.Update(mFrameTime);          
        }
    }
}