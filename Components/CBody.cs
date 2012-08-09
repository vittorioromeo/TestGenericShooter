using System;
using System.Collections.Generic;
using System.Linq;
using SFMLStart.Vectors;
using TestGenericShooter.SpatialHash;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CBody : Component
    {
        private readonly CPosition _cPosition;
        private  SSVector2I _previousPosition;
        private readonly bool _isStatic;

        public CBody(PhysicsWorld mPhysicsWorld, CPosition mCPosition, bool mIsStatic, int mWidth, int mHeight)
        {
            PhysicsWorld = mPhysicsWorld;
            _cPosition = mCPosition;
            _isStatic = mIsStatic;
            HalfSize = new SSVector2I(mWidth/2, mHeight/2);

            Groups = new HashSet<int>();
            GroupsToCheck = new HashSet<int>();
            GroupsToIgnoreResolve = new HashSet<int>();

            Cells = new HashSet<Cell>();
        }

        public PhysicsWorld PhysicsWorld { get; set; }
        public HashSet<Cell> Cells { get; set; }
        public SSVector2I Velocity { get; set; }
        public SSVector2I HalfSize { get; set; }
        public HashSet<int> Groups { get; private set; }
        public HashSet<int> GroupsToCheck { get; private set; }
        public HashSet<int> GroupsToIgnoreResolve { get; private set; }
        public Action<float, Entity, CBody> OnCollision { get; set; }

        #region Shortcut Properties
        public SSVector2I Position { get { return _cPosition.Position; } set { _cPosition.Position = value; } }
        public int Left { get { return _cPosition.X - HalfSize.X; } }
        public int Right { get { return _cPosition.X + HalfSize.X; } }
        public int Top { get { return _cPosition.Y - HalfSize.Y; } }
        public int Bottom { get { return _cPosition.Y + HalfSize.Y; } }
        public int HalfWidth { get { return HalfSize.X; } }
        public int HalfHeight { get { return HalfSize.Y; } }
        public int Width { get { return HalfSize.X*2; } }
        public int Height { get { return HalfSize.Y*2; } }
        #endregion

        public void AddGroups(params int[] mGroups) { foreach (var group in mGroups) Groups.Add(group); }
        public void AddGroupsToCheck(params int[] mGroups)
        {
            foreach (var group in mGroups) GroupsToCheck.Add(group);
            ;
        }
        public void AddGroupsToIgnoreResolve(params int[] mGroups) { foreach (var group in mGroups) GroupsToIgnoreResolve.Add(group); }

        private bool IsOverlapping(CBody mBody) { return Right > mBody.Left && Left < mBody.Right && (Bottom > mBody.Top && Top < mBody.Bottom); }

        public override void Added() { PhysicsWorld.AddBody(this); }
        public override void Removed() { PhysicsWorld.RemoveBody(this); }
        public override void Update(float mFrameTime)
        {
            if (_isStatic) return;

            _previousPosition = Position;
            Position += Velocity*mFrameTime;

            var checkedBodies = new HashSet<CBody> {this};
            var bodiesToCheck = PhysicsWorld.GetBodies(this);
            bodiesToCheck.Sort((a, b) => Velocity.X > 0 ? a.Position.X.CompareTo(b.Position.X) : b.Position.X.CompareTo(a.Position.X));

            foreach (var body in bodiesToCheck)
            {
                if (checkedBodies.Contains(body)) continue;
                checkedBodies.Add(body);

                if (!IsOverlapping(body)) continue;

                if (OnCollision != null) OnCollision(mFrameTime, body.Entity, body);
                if (body.OnCollision != null) body.OnCollision(mFrameTime, Entity, this);

                if (GroupsToIgnoreResolve.Any(x => body.Groups.Contains(x))) continue;

                int encrX = 0, encrY = 0;

                if (Bottom < body.Bottom && Bottom >= body.Top) encrY = body.Top - Bottom;
                else if (Top > body.Top && Top <= body.Bottom) encrY = body.Bottom - Top;
                if (Left < body.Left && Right >= body.Left) encrX = body.Left - Right;
                else if (Right > body.Right && Left <= body.Right) encrX = body.Right - Left;

                var numPxOverlapX = Left < body.Left ? Right - body.Left : body.Right - Left;
                var numPxOverlapY = Top < body.Top ? Bottom - body.Top : body.Bottom - Top;

                Position += numPxOverlapX > numPxOverlapY ? new SSVector2I(0, encrY) : new SSVector2I(encrX, 0);
            }

            PhysicsWorld.UpdateBody(this);
        }
    }
}