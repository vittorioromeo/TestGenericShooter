using System.Linq;
using SFML.Window;
using SFMLStart.Utilities;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CTargeter : Component
    {
        private readonly CPosition _cPosition;

        public CTargeter(CPosition mCPosition) { _cPosition = mCPosition; }

        public Entity Target { get; private set; }
        public CPosition TargetPosition { get; private set; }

        public void FindTarget(string mTag)
        {
            if (Target != null) return;

            Target = Manager.GetEntitiesByTag(mTag).FirstOrDefault();
            if (Target == null) return;
            TargetPosition = Target.GetComponent<CPosition>();
        }

        public float GetDegreesTowards(int mX, int mY) { return Utils.Math.Angles.TowardsDegrees(new Vector2f(_cPosition.X, _cPosition.Y), new Vector2f(mX, mY)); }
    }
}