using TestGenericShooter.Resources;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CShield : Component
    {
        private readonly GSGame _game;
        private readonly CBody _body;

        public CShield(GSGame mGame, CBody mCBody)
        {
            _game = mGame;
            _body = mCBody;

            _body.OnCollision += (mFrameTime, mEntity, mBody) =>
                                 {
                                     if (!mEntity.HasTag(Tags.BulletBlack) || mEntity.IsDead) return;

                                     var cMovement = mEntity.GetComponent<CMovement>();
                                     var speed = cMovement.Speed;
                                     var angle = cMovement.Angle;

                                     mEntity.Destroy();

                                     _game.Factory.Bullet(_body.Position.X, _body.Position.Y, 180 + angle, (int)speed, false);
                                 };
        }
    }
}