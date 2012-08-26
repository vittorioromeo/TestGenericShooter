using System.Linq;
using SFML.Window;
using SFMLStart.Utilities;
using SFMLStart.Vectors;
using VeeCollision;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CTargeter : Component
    {
        private readonly CBody _cBody;
        private int _retargetDelay; 

        public CTargeter(CBody mCBody, string mTargetTag)
        {
            _cBody = mCBody;
            TargetTag = mTargetTag;
        }

        public Entity Target { get; private set; }
        public CBody TargetBody { get; private set; }
        public string TargetTag { get; set; }

        private void FindTarget()
        {
            Target = Entity.Manager.GetEntitiesByTag(TargetTag).OrderBy(x =>
                                                                        {
                                                                            var cBody = x.GetComponentUnSafe<CBody>();
                                                                            return Utils.Math.Distances.Euclidean(_cBody.Position.X, _cBody.Position.Y, cBody.Position.X, cBody.Position.Y);
                                                                        }).FirstOrDefault();
            if (Target != null) TargetBody = Target.GetComponentUnSafe<CBody>();
        }

        public float GetDegreesTowardsTarget() { return Utils.Math.Angles.TowardsDegrees(_cBody.Position, TargetBody.Position); }
        public override void Update(float mFrameTime)
        {
            _retargetDelay--;

            if (_retargetDelay < 0 || Target == null || Target.IsDead)
            {
                FindTarget();
                _retargetDelay = 200;
            }
        }
    }
}