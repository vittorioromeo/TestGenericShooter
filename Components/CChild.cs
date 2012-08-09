using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CChild : Component
    {
        private readonly CBody _cBody;
        private readonly Entity _parent;
        private readonly CPosition _parentPosition;

        public CChild(Entity mParent, CBody mCBody)
        {
            _parent = mParent;
            _parentPosition = mParent.GetComponent<CPosition>();
            _cBody = mCBody;

            _parent.OnDestroy += () => Entity.Destroy();
        }

        public override void Update(float mFrameTime) { _cBody.Position = _parentPosition.Position; }
    }
}