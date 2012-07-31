using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CChild : Component
    {
        private readonly Entity _parent;
        private readonly CPosition _parentPosition;
        private readonly CBody _cBody;

        public CChild(Entity mParent, CBody mCBody)
        {
            _parent = mParent;
            _parentPosition = mParent.GetComponent<CPosition>();
            _cBody = mCBody;
        }

        public override void Update(float mFrameTime)
        {            
            _cBody.Position = _parentPosition.Position;
        }
    }
}