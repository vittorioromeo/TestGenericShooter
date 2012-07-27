using TestGenericShooter.Components;
using VeeEntitySystem2012;

namespace TestGenericShooter
{
    public class Factory
    {
        private readonly GSGame _game;
        private readonly Manager _manager;
        private readonly PhysicsWorld _physicsWorld;

        public Factory(GSGame mGame, Manager mManager, PhysicsWorld mPhysicsWorld)
        {
            _game = mGame;
            _manager = mManager;
            _physicsWorld = mPhysicsWorld;
        }

        public Entity Player(int mX, int mY)
        {
            var result = new Entity(_manager);

            var cPosition = new CPosition(new GSVector2(mX, mY));
            var cBody = new CBody(_physicsWorld, cPosition, false, 2000, 2000);
            var cMovement = new CMovement(cBody);
            var cTargeter = new CTargeter(cPosition);
            var cControl = new CControl(_game, cBody, cMovement, cTargeter);
            var cRenderShape = new CRenderShape(_game, cBody);

            cBody.AddGroups(Groups.Character, Groups.Player);
            cBody.AddGroupsToCheck(Groups.Obstacle);

            result.AddComponents(cPosition, cBody, cMovement, cTargeter, cControl, cRenderShape);
            result.AddTags("player");

            return result;
        }
        public Entity Wall(int mX, int mY, int mWidth, int mHeight)
        {
            var result = new Entity(_manager);

            var cPosition = new CPosition(new GSVector2(mX, mY));
            var cBody = new CBody(_physicsWorld, cPosition, true, mWidth, mHeight);
            var cRenderShape = new CRenderShape(_game, cBody);

            cBody.AddGroups(Groups.Obstacle);

            result.AddComponents(cPosition, cBody, cRenderShape);
            result.AddTags("wall");

            return result;
        }
        public Entity BreakableWall(int mX, int mY, int mWidth, int mHeight)
        {
            var result = new Entity(_manager);

            var cPosition = new CPosition(new GSVector2(mX, mY));
            var cBody = new CBody(_physicsWorld, cPosition, false, mWidth, mHeight);
            var cRenderShape = new CRenderShape(_game, cBody);

            cBody.AddGroups(Groups.Obstacle); 
            cBody.AddGroupsToCheck(Groups.Obstacle);

            cBody.OnCollision += (mEntity, mBody) =>
            {
                if (!mEntity.HasTag("bullet")) return;
                cBody.HalfSize -= new GSVector2(75, 75);
                if(cBody.HalfWidth < 200) result.Destroy();
            };

            result.AddComponents(cPosition, cBody, cRenderShape);
            result.AddTags("wall");

            return result;
        }
        public Entity Bullet(int mX, int mY, float mDegrees, int mSpeed = 1000)
        {
            var result = new Entity(_manager);

            var cPosition = new CPosition(new GSVector2(mX, mY));
            var cBody = new CBody(_physicsWorld, cPosition, false, 500, 500);
            var cMovement = new CMovement(cBody);
            var cRenderShape = new CRenderShape(_game, cBody);

            cBody.AddGroupsToCheck(Groups.Obstacle, Groups.Character);
            cBody.AddGroupsToIgnoreResolve(Groups.Character);

            cMovement.MoveTowardsAngle(mDegrees, mSpeed);
            cBody.OnCollision += (mEntity, mBody) =>
                                 {
                                     if (!mEntity.HasTag("wall")) return;
                                     result.Destroy();
                                 };

            result.AddComponents(cPosition, cBody, cMovement, cRenderShape);
            result.AddTags("bullet");

            return result;
        }

        public Entity Enemy(int mX, int mY)
        {
            var result = new Entity(_manager);

            var cPosition = new CPosition(new GSVector2(mX, mY));
            var cHealth = new CHealth(1);
            var cBody = new CBody(_physicsWorld, cPosition, false, 2000, 2000);
            var cMovement = new CMovement(cBody);
            var cTargeter = new CTargeter(cPosition);
            var cRenderShape = new CRenderShape(_game, cBody);
            var cAI = new CAI(_game, cBody, cMovement, cTargeter);

            cBody.AddGroups(Groups.Character, Groups.Enemy);
            cBody.AddGroupsToCheck(Groups.Obstacle);

            cBody.OnCollision += (mEntity, mBody) =>
                                 {
                                     if (!mEntity.HasTag("playerbullet")) return;
                                     mEntity.Destroy();
                                     cHealth.Health--;
                                 };

            result.AddComponents(cPosition, cHealth, cBody, cMovement, cTargeter, cRenderShape, cAI);
            result.AddTags("enemy");

            return result;
        }
    }
}