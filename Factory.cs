using SFML.Graphics;
using SFMLStart.Utilities;
using SFMLStart.Vectors;
using TestGenericShooter.Components;
using TestGenericShooter.Resources;
using VeeCollision;
using VeeEntitySystem2012;

namespace TestGenericShooter
{
    public class Factory
    {
        private readonly GSGame _game;
        private readonly Manager _manager;
        private readonly World _world;

        public Factory(GSGame mGame, Manager mManager, World mWorld)
        {
            _game = mGame;
            _manager = mManager;
            _world = mWorld;
        }

        #region Components
        private CBody Body(SSVector2I mPosition, int mWidth, int mHeight, bool mIsStatic = false) { return new CBody(_world, mPosition, mIsStatic, mWidth, mHeight); }
        private CRender Render(CBody mCBody, string mTextureName, string mTilesetName = null, string mLabelName = null, float mRotation = 0) { return new CRender(_game, mCBody, mTextureName, mTilesetName, mLabelName, mRotation); }
        private CMovement Movement(CBody mCBody) { return new CMovement(mCBody); }
        private CTargeter Targeter(CBody mCBody, string mTargetTag) { return new CTargeter(mCBody, mTargetTag); }
        private CControl Control(CBody mCBody, CMovement mCMovement, CTargeter mCTargeter, CRender mCRender) { return new CControl(_game, mCBody, mCMovement, mCTargeter, mCRender); }
        private CChild Child(Entity mParent, CBody mCBody) { return new CChild(mParent, mCBody); }
        private CHealth Health(int mHealth) { return new CHealth(mHealth); }
        private CAI AI(CBody mCBody, CMovement mCMovement, CTargeter mCTargeter, CRender mCRender, bool mBig = false) { return new CAI(_game, mCBody, mCMovement, mCTargeter, mCRender, mBig); }
        private CPurification Purification(CRender mCRender) { return new CPurification(_game, mCRender); }
        private CPurifier Purifier(CBody mCBody, bool mEnemy) { return new CPurifier(mCBody, mEnemy); }
        #endregion

        #region Environment
        public Entity Wall(int mX, int mY, string mLabelName = "fill", float mRotation = 0)
        {
            var result = new Entity(_manager);

            var cBody = Body(new SSVector2I(mX, mY), 1600, 1600, true);
            var cRender = Render(cBody, Textures.WallBlack, Tilesets.Wall, mLabelName, mRotation);
            var cPurification = Purification(cRender);

            cBody.AddGroups(Groups.Obstacle);

            result.AddComponents(cBody, cRender, cPurification);
            result.AddTags(Tags.Wall, Tags.DestroysBullets, Tags.Purifiable);

            return result;
        }
        public Entity BreakableWall(int mX, int mY)
        {
            var result = new Entity(_manager);

            var cHealth = Health(85);
            var cBody = Body(new SSVector2I(mX, mY), 1600, 1600, true);
            var cRender = Render(cBody, Textures.WallBlack, Tilesets.Wall, "breakable");
            var cPurification = Purification(cRender);

            cBody.AddGroups(Groups.Obstacle);

            result.AddComponents(cHealth, cBody, cRender, cPurification);
            result.AddTags(Tags.Wall, Tags.DamagedByAny, Tags.Purifiable);

            cRender.Sprite.Rotation = Utils.RandomGenerator.GetNextInt(0, 4)*90;

            return result;
        }
        public Entity Decoration(int mX, int mY, string mTextureName, string mTilesetName, string mLabelName, float mRotation = 0)
        {
            var result = new Entity(_manager);

            var cBody = Body(new SSVector2I(mX, mY), 1600, 1600, true);
            var cRender = Render(cBody, mTextureName, mTilesetName, mLabelName, mRotation);
            var cPurification = Purification(cRender);

            cBody.AddGroups(Groups.Decoration);

            result.AddComponents(cBody, cRender, cPurification);
            result.AddTags(Tags.Decoration, Tags.Purifiable);

            return result;
        }
        #endregion

        #region Characters
        public Entity Player(int mX, int mY)
        {
            var result = new Entity(_manager);

            var cHealth = Health(10);
            var cBody = Body(new SSVector2I(mX, mY), 1000, 1000);
            var cMovement = Movement(cBody);
            var cTargeter = Targeter(cBody, Tags.Enemy);
            var cRender = Render(cBody, Textures.CharPlayer, Tilesets.Char, "normal");
            var cControl = Control(cBody, cMovement, cTargeter, cRender);
            //var cShadower = new CShadower(_game, cBody);

            cBody.AddGroups(Groups.Character, Groups.Friendly);
            cBody.AddGroupsToCheck(Groups.Obstacle);

            result.AddComponents(cHealth, cBody, cMovement, cTargeter, cControl, cRender);
            result.AddTags(Tags.Char, Tags.Friendly, Tags.DamagedByBlack);

            Aura(result, mX, mY, false);
            //Shield(result, mX, mY);

            return result;
        }
        public Entity Friendly(int mX, int mY)
        {
            var result = new Entity(_manager);

            var cHealth = Health(6);
            var cBody = Body(new SSVector2I(mX, mY), 1000, 1000);
            var cMovement = Movement(cBody);
            var cTargeter = Targeter(cBody, Tags.Enemy);
            var cRender = Render(cBody, Textures.CharFriendly, Tilesets.Char, "normal");
            var cAI = AI(cBody, cMovement, cTargeter, cRender);

            cBody.AddGroups(Groups.Character, Groups.Friendly);
            cBody.AddGroupsToCheck(Groups.Obstacle);

            cAI.Friendly = true;

            result.AddComponents(cHealth, cBody, cMovement, cTargeter, cRender, cAI);
            result.AddTags(Tags.Char, Tags.Friendly, Tags.DamagedByBlack);

            Aura(result, mX, mY, false);

            return result;
        }
        public Entity BigFriendly(int mX, int mY)
        {
            var result = new Entity(_manager);

            var cHealth = Health(12);
            var cBody = Body(new SSVector2I(mX, mY), 2000, 2000);
            var cMovement = Movement(cBody);
            var cTargeter = Targeter(cBody, Tags.Enemy);
            var cRender = Render(cBody, Textures.BigCharFriendly, Tilesets.BigChar, "normal");
            var cAI = AI(cBody, cMovement, cTargeter, cRender, true);

            cBody.AddGroups(Groups.Character, Groups.Friendly);
            cBody.AddGroupsToCheck(Groups.Obstacle);

            cAI.Friendly = true;

            result.AddComponents( cHealth, cBody, cMovement, cTargeter, cRender, cAI);
            result.AddTags(Tags.Char, Tags.Friendly, Tags.DamagedByBlack);

            Aura(result, mX, mY, false);

            return result;
        }
        public Entity Enemy(int mX, int mY)
        {
            var result = new Entity(_manager);

            var cHealth = Health(6);
            var cBody = Body(new SSVector2I(mX, mY), 1000, 1000);
            var cMovement = Movement(cBody);
            var cTargeter = Targeter(cBody, Tags.Friendly);
            var cRender = Render(cBody, Textures.CharBlack, Tilesets.Char, "normal");
            var cAI = AI(cBody, cMovement, cTargeter, cRender);
            var cShadower = new CShadower(_game, cBody);
            var cLineOfSight = new CLineOfSight(cBody, cShadower) {TargetTag = Tags.Friendly, Angle = 90, Amplitude = 30};

            cBody.AddGroups(Groups.Character, Groups.Enemy);
            cBody.AddGroupsToCheck(Groups.Obstacle);

            result.AddComponents(cHealth, cBody, cMovement, cTargeter, cRender, cAI, cShadower, cLineOfSight);
            result.AddTags(Tags.Char, Tags.Enemy, Tags.DamagedByWhite);

            Aura(result, mX, mY, true);

            return result;
        }
        public Entity BigEnemy(int mX, int mY)
        {
            var result = new Entity(_manager);

            var cHealth = Health(12);
            var cBody = Body(new SSVector2I(mX, mY), 2000, 2000);
            var cMovement = Movement(cBody);
            var cTargeter = Targeter(cBody, Tags.Friendly);
            var cRender = Render(cBody, Textures.BigCharBlack, Tilesets.BigChar, "normal");
            var cAI = AI(cBody, cMovement, cTargeter, cRender, true);

            cBody.AddGroups(Groups.Character, Groups.Enemy);
            cBody.AddGroupsToCheck(Groups.Obstacle);

            result.AddComponents(cHealth, cBody, cMovement, cTargeter, cRender, cAI);
            result.AddTags(Tags.Char, Tags.Enemy, Tags.DamagedByWhite);

            Aura(result, mX, mY, true);

            return result;
        }
        #endregion

        private Entity Aura(Entity mParent, int mX, int mY, bool mEnemy)
        {
            var result = new Entity(_manager);

            var cBody = Body(new SSVector2I(mX, mY), 4500, 4500);
            var cChild = Child(mParent, cBody);
            var cPurifier = Purifier(cBody, mEnemy);

            cBody.AddGroupsToCheck(Groups.Obstacle, Groups.Decoration);
            cBody.AddGroupsToIgnoreResolve(Groups.Obstacle, Groups.Decoration);

            result.AddComponents(cBody, cChild, cPurifier);

            return result;
        }
        private Entity Shield(Entity mParent, int mX, int mY)
        {
            var result = new Entity(_manager);

            var cBody = Body(new SSVector2I(mX, mY), 4500, 4500);
            var cChild = Child(mParent, cBody);
            var cShield = new CShield(_game, cBody);

            cBody.AddGroups(Groups.Character);

            result.AddComponents(cBody, cChild, cShield);
            result.AddTags(Tags.Shield);

            return result;
        }

        private Entity BulletBase(int mX, int mY, float mDegrees, int mSpeed, string mTextureName, bool mEnemy)
        {
            var result = new Entity(_manager);

            var cBody = Body(new SSVector2I(mX, mY), 250, 250);
            var cMovement = Movement(cBody);
            var cRender = Render(cBody, mTextureName);
            var cPurifier = Purifier(cBody, mEnemy);

            cBody.AddGroupsToCheck(Groups.Obstacle, Groups.Character);
            cBody.AddGroupsToIgnoreResolve(Groups.Obstacle, Groups.Character);
            cBody.OnCollision += (mFrameTime, mEntity, mBody) =>
                                 {
                                     var cHealth = mEntity.GetComponent<CHealth>();

                                     if (mEntity.HasTag(Tags.DamagedByAny))
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

                                     if (mEntity.HasTag(Tags.DestroysBullets)) result.Destroy();
                                 };

            cMovement.Angle = mDegrees;
            cMovement.Speed = mSpeed;

            cRender.Torque = 8;

            result.AddComponents(cBody, cMovement, cRender, cPurifier);
            result.AddTags(Tags.Bullet);

            return result;
        }
        public Entity Bullet(int mX, int mY, float mDegrees, int mSpeed, bool mEnemy)
        {
            var texture = !mEnemy ? Textures.BulletWhite : Textures.BulletBlack;
            var result = BulletBase(mX, mY, mDegrees, mSpeed, texture, mEnemy);
            result.AddTags(!mEnemy ? Tags.BulletWhite : Tags.BulletBlack);
            return result;
        }

        private Entity BigBulletBase(int mX, int mY, float mDegrees, int mSpeed, string mTextureName, bool mEnemy)
        {
            var result = new Entity(_manager);

            var cBody = Body(new SSVector2I(mX, mY), 500, 500);
            var cMovement = Movement(cBody);
            var cRender = Render(cBody, mTextureName);
            var cPurifier = Purifier(cBody, mEnemy);

            cBody.AddGroupsToCheck(Groups.Obstacle, Groups.Character);
            cBody.AddGroupsToIgnoreResolve(Groups.Obstacle, Groups.Character);
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

            cMovement.Angle = mDegrees;
            cMovement.Speed = mSpeed;

            cRender.Torque = 25;

            result.AddComponents(cBody, cMovement, cRender);
            result.AddTags(Tags.Bullet);

            return result;
        }
        public Entity BigBullet(int mX, int mY, float mDegrees, int mSpeed, bool mEnemy)
        {
            var texture = !mEnemy ? Textures.BigBulletWhite : Textures.BigBulletBlack;
            var result = BigBulletBase(mX, mY, mDegrees, mSpeed, texture, mEnemy);
            result.AddTags(!mEnemy ? Tags.BulletWhite : Tags.BulletBlack);
            return result;
        }

        private Entity SporeBase(int mX, int mY, float mDegrees, int mSpeed, string mTextureName, bool mEnemy)
        {
            var result = new Entity(_manager);

            var cBody = Body(new SSVector2I(mX, mY), 250, 250);
            var cMovement = Movement(cBody);
            var cRender = Render(cBody, mTextureName);

            cBody.AddGroupsToCheck(Groups.Obstacle, Groups.Character);
            cBody.AddGroupsToIgnoreResolve(Groups.Obstacle, Groups.Character);
            cBody.OnCollision += (mFrameTime, mEntity, mBody) =>
                                 {
                                     var cHealth = mEntity.GetComponent<CHealth>();

                                     if (result.HasTag(Tags.SporeBlack) && mEntity.HasTag(Tags.DamagedByWhite))
                                     {
                                         cHealth++;
                                         result.Destroy();
                                     }
                                     else if (result.HasTag(Tags.SporeWhite) && mEntity.HasTag(Tags.DamagedByBlack))
                                     {
                                         cHealth++;
                                         result.Destroy();
                                     }

                                     if (mEntity.HasTag(Tags.DestroysBullets)) result.Destroy();
                                 };

            cMovement.Angle = mDegrees;
            cMovement.Speed = mSpeed;
            cMovement.Acceleration = -0.1f;

            cRender.Torque = 8;

            result.AddComponents(cBody, cMovement, cRender);
            result.AddTags(Tags.Spore);

            cRender.Sprite.Color = new Color(255, 255, 255, 125);

            return result;
        }
        public Entity Spore(int mX, int mY, float mDegrees, int mSpeed, bool mEnemy)
        {
            var texture = !mEnemy ? Textures.BulletWhite : Textures.BulletBlack;
            var result = SporeBase(mX, mY, mDegrees, mSpeed, texture, mEnemy);
            result.AddTags(!mEnemy ? Tags.SporeWhite : Tags.SporeBlack);
            return result;
        }
    }
}