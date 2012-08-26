using System;
using System.Collections.Generic;
using System.Linq;
using SFMLStart.Utilities;
using SFMLStart.Vectors;
using VeeCollision;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CLineOfSight : Component
    {
        private readonly CBody _cBody;
        private readonly CShadower _cShadower;        

        public CLineOfSight(CBody mCBody, CShadower mCShadower)
        {
            _cBody = mCBody;
            _cShadower = mCShadower;
            Targets = new List<Entity>();
        }

        public int Angle { get; set; }
        public int Amplitude { get; set; }
        public string TargetTag { get; set; }
        public List<Entity> Targets { get; set; }

        public override void Update(float mFrameTime)
        {
            Targets.Clear();
            
            foreach(var target in Manager.GetEntitiesByTag(TargetTag))
            {
                var body = target.GetComponentUnSafe<CBody>();

                var thisPoint = new SSVector2F(_cBody.X, _cBody.Y);
                var targetPoint = new SSVector2F(body.X, body.Y);
                var angleVector = Utils.Math.Angles.ToVectorDegrees(180 + Angle);
                var spanVector = thisPoint - targetPoint;

                var checkAngle = Math.Abs(angleVector.GetAngleBetween(spanVector));

                if (checkAngle > Amplitude) continue;

                var polygons = _cShadower.GetPolygons();
                if(polygons.Any(x => x.IsIntersecting(targetPoint))) continue;

                Targets.Add(target);
            }

            foreach(var target in Targets)
            {
                target.Destroy();
            }
        }
    }
}