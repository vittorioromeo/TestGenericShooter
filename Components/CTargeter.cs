using System.Linq;
using SFML.Window;
using SFMLStart.Utilities;
using SFMLStart.Vectors;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CTargeter : Component
    {
        private readonly CPosition _cPosition;

        public CTargeter(CPosition mCPosition, string mTargetTag)
        {
            _cPosition = mCPosition;
            TargetTag = mTargetTag;
        }

        public Entity Target { get; private set; }
        public CPosition TargetPosition { get; private set; }
        public string TargetTag { get; set; }

        private void FindTarget()
        {
            Target = Entity.Manager.GetEntitiesByTag(TargetTag).OrderBy(x =>
                                                                        {
                                                                            var cPosition = x.GetComponent<CPosition>();
                                                                            return Utils.Math.Distances.Euclidean(_cPosition.X, _cPosition.Y, cPosition.X, cPosition.Y);
                                                                        }).FirstOrDefault();
            if (Target != null) TargetPosition = Target.GetComponent<CPosition>();
        }

        public float GetDegreesTowardsTarget() { return Utils.Math.Angles.TowardsDegrees(_cPosition.Position, TargetPosition.Position); }
        public override void Update(float mFrameTime)
        {
            if (Target != null && !Target.IsDead) return;
            FindTarget();
        }
    }
}