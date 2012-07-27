using System;
using System.Collections.Generic;
using System.Linq;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CBody : Component
    {
        private readonly PhysicsWorld _physicsWorld;
        private readonly CPosition _cPosition;
        private readonly bool _isStatic;
        private readonly HashSet<string> _groups;
        private readonly HashSet<string> _groupsToCheck;
        private readonly HashSet<string> _groupsToIgnoreResolve;

        public CBody(PhysicsWorld mPhysicsWorld, CPosition mCPosition, bool mIsStatic, int mWidth, int mHeight)
        {
            _physicsWorld = mPhysicsWorld;
            _cPosition = mCPosition;
            _isStatic = mIsStatic;
            HalfSize = new GSVector2(mWidth / 2, mHeight / 2);

            _groups = new HashSet<string>();
            _groupsToCheck = new HashSet<string>();
            _groupsToIgnoreResolve = new HashSet<string>();

            Cells = new HashSet<Cell>();           
        }

        public GSVector2 Velocity { get; set; }
        public HashSet<Cell> Cells { get; set; }
        public Action<Entity, CBody> OnCollision { get; set; }
        public GSVector2 HalfSize { get; set; }

        #region Shortcut Properties
        public GSVector2 Position { get { return _cPosition.Position; } set { _cPosition.Position = value; } }
        public int Left { get { return Position.X - HalfSize.X; } }
        public int Right { get { return Position.X + HalfSize.X; } }
        public int Top { get { return Position.Y - HalfSize.Y; } }
        public int Bottom { get { return Position.Y + HalfSize.Y; } }
        public int HalfWidth { get { return HalfSize.X; } }
        public int HalfHeight { get { return HalfSize.Y; } }
        public int Width { get { return HalfSize.X*2; } }
        public int Height { get { return HalfSize.Y*2; } }
        #endregion

        public void SetCells(HashSet<Cell> mCells) { Cells = mCells; }

        public void AddGroups(params string[] mGroups) { foreach (var group in mGroups) AddGroup(group); }
        public void AddGroupsToCheck(params string[] mGroups) { foreach (var group in mGroups)AddGroupToCheck(group); }
        public void AddGroupsToIgnoreResolve(params string[] mGroups) { foreach (var group in mGroups)AddGroupToIgnoreResolve(group); }     
        public IEnumerable<string> GetGroups() { return _groups; }
        public IEnumerable<string> GetGroupsToCheck() { return _groupsToCheck; }

        private void AddGroup(string mGroup) { _groups.Add(mGroup); }
        private void AddGroupToCheck(string mGroup) { _groupsToCheck.Add(mGroup); }
        private void AddGroupToIgnoreResolve(string mGroup) { _groupsToIgnoreResolve.Add(mGroup); }
        private bool HasGroup(string mGroup) { return _groups.Contains(mGroup); }
        private bool IsOverlapping(CBody mBody) { return Right > mBody.Left && Left < mBody.Right && (Bottom > mBody.Top && Top < mBody.Bottom); }
      
        public override void Added() { _physicsWorld.AddBody(this); }
        public override void Removed() { _physicsWorld.RemoveBody(this); }
        public override void Update(float mFrameTime)
        {
            if (_isStatic) return;

            Position += Velocity*mFrameTime;

            if (_physicsWorld.IsOutOfBounds(this))
            {
                Entity.Destroy();
                return;
            }

            var checkedBodies = new HashSet<CBody> {this};
            var bodiesToCheck = _physicsWorld.GetBodies(this);

            if (Velocity.X < 0) bodiesToCheck.Sort((b, a) => a.Position.X.CompareTo(b.Position.X));
            else bodiesToCheck.Sort((a, b) => a.Position.X.CompareTo(b.Position.X));

            foreach (var body in bodiesToCheck)
            {
                if (checkedBodies.Contains(body)) continue;
                checkedBodies.Add(body);

                if (!IsOverlapping(body)) continue;

                if (OnCollision != null) OnCollision(body.Entity, body);
                if (body.OnCollision != null) body.OnCollision(Entity, this);

                if (_groupsToIgnoreResolve.Any(groupToIgnoreResolve => body.HasGroup(groupToIgnoreResolve))) continue;

                int encrX = 0, encrY = 0, numPxOverlapX, numPxOverlapY;

                if (Bottom < body.Bottom && Bottom >= body.Top) encrY = body.Top - Bottom;
                else if (Top > body.Top && Top <= body.Bottom) encrY = body.Bottom - Top;

                if (Left < body.Left && Right >= body.Left) encrX = body.Left - Right;
                else if (Right > body.Right && Left <= body.Right) encrX = body.Right - Left;

                if (Left < body.Left) numPxOverlapX = Right - body.Left; else numPxOverlapX = body.Right - Left;
                if (Top < body.Top) numPxOverlapY = Bottom - body.Top; else numPxOverlapY = body.Bottom - Top;           

                if (numPxOverlapX > numPxOverlapY) Position += new GSVector2(0, encrY);
                else Position += new GSVector2(encrX, 0);
            }

            _physicsWorld.UpdateBody(this);
        }
    }
}