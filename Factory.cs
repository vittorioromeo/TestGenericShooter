using System;
using SFML.Graphics;
using SFML.Window;
using SFMLStart.Data;
using SFMLStart.Utilities;
using TestGenericShooter.Components;
using TestGenericShooter.Resources;
using TestGenericShooter.SpatialHash;
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

        #region Components
        private CPosition Position(int mX, int mY) { return new CPosition(new GSVector2(mX, mY)); }
        private CBody Body(CPosition mCPosition, int mWidth, int mHeight, bool mIsStatic = false) { return new CBody(_physicsWorld, mCPosition, mIsStatic, mWidth, mHeight); }
        private CRender Render(CPosition mCPosition, string mTextureName, string mTilesetName = null, string mLabelName = null, float mRotation = 0) { return new CRender(_game, mCPosition, mTextureName, mTilesetName, mLabelName, mRotation); }
        private CMovement Movement(CBody mCBody) { return new CMovement(mCBody); }
        private CTargeter Targeter(CPosition mCPosition) { return new CTargeter(mCPosition); }
        private CControl Control(CBody mCBody, CMovement mCMovement, CTargeter mCTargeter, CRender mCRender) { return new CControl(_game, mCBody, mCMovement, mCTargeter, mCRender); }
        private CChild Child(Entity mParent, CBody mCBody) { return new CChild(mParent, mCBody); }
        private CHealth Health(int mHealth) { return new CHealth(mHealth); }
        private CAI AI(CBody mCBody, CMovement mCMovement, CTargeter mCTargeter, CRender mCRender, bool mBig = false) { return new CAI(_game, mCBody, mCMovement, mCTargeter, mCRender, mBig); } 
        #endregion

        #region Environment
        public Entity Wall(int mX, int mY, string mLabelName = "fill", float mRotation = 0)
        {
            var result = new Entity(_manager);

            var cPosition = Position(mX, mY);
            var cBody = Body(cPosition, 1600, 1600, true);
            var cRender = Render(cPosition, Textures.WallBlack, Tilesets.Wall, mLabelName, mRotation);

            cBody.AddGroups(Groups.Obstacle);

            result.AddComponents(cPosition, cBody, cRender);
            result.AddTags(Tags.Wall, Tags.DestroysBullets);

            return result;
        }
        public Entity BreakableWall(int mX, int mY)
        {
            var result = new Entity(_manager);

            var cPosition = Position(mX, mY);
            var cHealth = Health(3);
            var cBody = Body(cPosition, 1600, 1600, true);
            var cRender = Render(cPosition, Textures.WallBlack, Tilesets.Wall, "breakable");

            cBody.AddGroups(Groups.Obstacle);            

            result.AddComponents(cPosition, cHealth, cBody, cRender);
            result.AddTags(Tags.Wall, Tags.DamagedByAny);

            cRender.Sprite.Rotation = Utils.RandomGenerator.GetNextInt(0, 4)*90;

            return result;
        }
        public Entity Decoration(int mX, int mY, string mTextureName, string mTilesetName, string mLabelName, float mRotation = 0)
        {
            var result = new Entity(_manager);

            var cPosition = Position(mX, mY);
            var cBody = Body(cPosition, 1600, 1600, true);
            var cRender = Render(cPosition, mTextureName, mTilesetName, mLabelName, mRotation);

            cBody.AddGroups(Groups.Decoration);

            result.AddComponents(cPosition, cBody, cRender);
            result.AddTags(Tags.Decoration);

            return result;
        }
        #endregion

        #region Characters
        public Entity Player(int mX, int mY)
        {
            var result = new Entity(_manager);

            var cPosition = Position(mX, mY);
            var cHealth = Health(10);
            var cBody = Body(cPosition, 1000, 1000);
            var cMovement = Movement(cBody);
            var cTargeter = Targeter(cPosition);
            var cRender = Render(cPosition, Textures.CharWhite, Tilesets.Char, "normal");
            var cControl = Control(cBody, cMovement, cTargeter, cRender);

            cBody.AddGroups(Groups.Character, Groups.Player);
            cBody.AddGroupsToCheck(Groups.Obstacle);

            result.AddComponents(cPosition, cHealth, cBody, cMovement, cTargeter, cControl, cRender);
            result.AddTags(Tags.Char, Tags.Player, Tags.DamagedByBlack);

            PurificationAura(result, mX, mY);

            return result;
        }
        public Entity Enemy(int mX, int mY)
        {
            var result = new Entity(_manager);

            var cPosition = Position(mX, mY);
            var cHealth = Health(6);
            var cBody = Body(cPosition, 1000, 1000);
            var cMovement = Movement(cBody);
            var cTargeter = Targeter(cPosition);
            var cRender = Render(cPosition, Textures.CharBlack, Tilesets.Char, "normal");
            var cAI = AI(cBody, cMovement, cTargeter, cRender);

            cBody.AddGroups(Groups.Character, Groups.Enemy);
            cBody.AddGroupsToCheck(Groups.Obstacle);

            result.AddComponents(cPosition, cHealth, cBody, cMovement, cTargeter, cRender, cAI);
            result.AddTags(Tags.Char, Tags.Enemy, Tags.DamagedByWhite);

            return result;
        }
        public Entity BigEnemy(int mX, int mY)
        {
            var result = new Entity(_manager);

            var cPosition = Position(mX, mY);
            var cHealth = Health(12);
            var cBody = Body(cPosition, 2000, 2000);
            var cMovement = Movement(cBody);
            var cTargeter = Targeter(cPosition);
            var cRender = Render(cPosition, Textures.BigCharBlack, Tilesets.BigChar, "normal");
            var cAI = AI(cBody, cMovement, cTargeter, cRender, true);

            cBody.AddGroups(Groups.Character, Groups.Enemy);
            cBody.AddGroupsToCheck(Groups.Obstacle);

            result.AddComponents(cPosition, cHealth, cBody, cMovement, cTargeter, cRender, cAI);
            result.AddTags(Tags.Char, Tags.Enemy, Tags.DamagedByWhite);

            return result;
        } 
        #endregion

        private Entity PurificationAura(Entity mParent, int mX, int mY)
        {
            var result = new Entity(_manager);

            var cPosition = Position(mX, mY);
            var cBody = Body(cPosition, 4500, 4500);
            var cChild = Child(mParent, cBody);

            cBody.AddGroupsToCheck(Groups.Obstacle, Groups.Decoration);
            cBody.AddGroupsToIgnoreResolve(Groups.Obstacle, Groups.Decoration);

            cBody.OnCollision += (mFrameTime, mEntity, mBody) =>
                                 {
                                    var sprite = mEntity.GetComponent<CRender>().Sprite;

                                    if (!mEntity.HasTag(Tags.Wall) && !mEntity.HasTag(Tags.Decoration)) return;

                                    if (sprite.Texture != Assets.GetTexture(Textures.WallWhite))
                                    {
                                        sprite.Texture = Assets.GetTexture(Textures.WallWhite);
                                        sprite.Color = Color.Black;
                                    }

                                    var next = sprite.Color.R + 25 * mFrameTime;
                                    var nextMin = (byte) Math.Min(next, 255);
                                    sprite.Color = new Color(nextMin, nextMin, nextMin, 255);
                                };

            result.AddComponents(cPosition, cBody, cChild);

            return result;
        }

        private Entity Bullet(int mX, int mY, float mDegrees, int mSpeed, string mTextureName)
        {
            var result = new Entity(_manager);

            var cPosition = Position(mX, mY);
            var cBody = Body(cPosition, 250, 250);
            var cMovement = Movement(cBody);
            var cRender = Render(cPosition, mTextureName);

            cBody.AddGroupsToCheck(Groups.Obstacle, Groups.Character);
            cBody.AddGroupsToIgnoreResolve(Groups.Character);
            cBody.OnCollision += (mFrameTime, mEntity, mBody) =>
                                 {
                                     var cHealth = mEntity.GetComponent<CHealth>();

                                     if(mEntity.HasTag(Tags.DamagedByAny))
                                     {
                                         cHealth--;
                                         result.Destroy();
                                     }
                                     else if (result.HasTag(Tags.BulletWhite) && mEntity.HasTag(Tags.DamagedByWhite))
                                     {
                                         cHealth--;
                                         result.Destroy();
                                     }
                                     else if (result.HasTag(Tags.BulletBlack) && mEntity.HasTag(Tags.DamagedByBlack))
                                     {
                                         cHealth--;
                                         result.Destroy();
                                     }

                                     if(mEntity.HasTag(Tags.DestroysBullets)) result.Destroy();
                                 };

            cMovement.MoveTowardsAngle(mDegrees, mSpeed);

            cRender.Torque = 8;

            result.AddComponents(cPosition, cBody, cMovement, cRender);
            result.AddTags(Tags.Bullet);

            return result;
        }
        public Entity BulletWhite(int mX, int mY, float mDegrees, int mSpeed)
        {
            var result = Bullet(mX, mY, mDegrees, mSpeed, Textures.BulletWhite);
            result.AddTags(Tags.BulletWhite);
            return result;
        }
        public Entity BulletBlack(int mX, int mY, float mDegrees, int mSpeed)
        {
            var result = Bullet(mX, mY, mDegrees, mSpeed, Textures.BulletBlack);
            result.AddTags(Tags.BulletBlack);
            return result;
        }

        private Entity BigBullet(int mX, int mY, float mDegrees, int mSpeed, string mTextureName)
        {
            var result = new Entity(_manager);

            var cPosition = Position(mX, mY);
            var cBody = Body(cPosition, 500, 500);
            var cMovement = Movement(cBody);
            var cRender = Render(cPosition, mTextureName);

            cBody.AddGroupsToCheck(Groups.Obstacle, Groups.Character);
            cBody.AddGroupsToIgnoreResolve(Groups.Character);
            cBody.OnCollision += (mFrameTime, mEntity, mBody) =>
            {
                var cHealth = mEntity.GetComponent<CHealth>();

                if (mEntity.HasTag(Tags.DamagedByAny))
                {
                    cHealth -= 2;
                    result.Destroy();
                }
                else if (result.HasTag(Tags.BulletWhite) && mEntity.HasTag(Tags.DamagedByWhite))
                {
                    cHealth -= 2;
                    result.Destroy();
                }
                else if (result.HasTag(Tags.BulletBlack) && mEntity.HasTag(Tags.DamagedByBlack))
                {
                    cHealth -= 2;
                    result.Destroy();
                }

                if (mEntity.HasTag(Tags.DestroysBullets)) result.Destroy();
            };

            cMovement.MoveTowardsAngle(mDegrees, mSpeed);

            cRender.Torque = 25;

            result.AddComponents(cPosition, cBody, cMovement, cRender);
            result.AddTags(Tags.Bullet);

            return result;
        }
        public Entity BigBulletWhite(int mX, int mY, float mDegrees, int mSpeed)
        {
            var result = BigBullet(mX, mY, mDegrees, mSpeed, Textures.BigBulletWhite);
            result.AddTags(Tags.BulletWhite);
            return result;
        }
        public Entity BigBulletBlack(int mX, int mY, float mDegrees, int mSpeed)
        {
            var result = BigBullet(mX, mY, mDegrees, mSpeed, Textures.BigBulletBlack);
            result.AddTags(Tags.BulletBlack);
            return result;
        }
    }
}