using TestGenericShooter.Resources;
using VeeCollision;
using VeeEntity;

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

            _body.OnCollision += (mCollisionInfo) =>
                                     {
                                         var entity = (Entity) mCollisionInfo.UserData;
                                     if (!entity.HasTag(Tags.BulletBlack) || entity.IsDead) return;

                                     var cMovement = entity.GetComponentUnSafe<CMovement>();
                                     var speed = cMovement.Speed;
                                     var angle = cMovement.Angle;

                                     entity.Destroy();

                                     _game.Factory.Bullet(_body.Position.X, _body.Position.Y, 180 + angle, (int)speed, false);
                                 };
        }
    }
}