using SFMLStart.Utilities;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CMovement : Component
    {
        private readonly CBody _cBody;

        public CMovement(CBody mCBody) { _cBody = mCBody; }

        public void Stop() { _cBody.Velocity = new GSVector2(0, 0); }
        public void MoveTowardsAngle(float mDegrees, int mSpeed)
        {
            var angleVector = Utils.Math.Angles.ToVectorDegrees(mDegrees);
            _cBody.Velocity = new GSVector2((int) (angleVector.X*mSpeed), (int) (angleVector.Y*mSpeed));
        }
    }
}