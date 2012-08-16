using TestGenericShooter.Resources;
using VeeCollision;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CPurifier : Component
    {
        private readonly CBody _body;

        public CPurifier(CBody mCBody, bool mEnemy)
        {
            _body = mCBody;

            _body.OnCollision += (mFrameTime, mEntity, mBody) =>
                                 {
                                     if (!mEntity.HasTag(Tags.Purifiable)) return;

                                     var cPurification = mEntity.GetComponentUnSafe<CPurification>();
                                     cPurification.Purifying = !mEnemy;
                                 };
        }
    }
}