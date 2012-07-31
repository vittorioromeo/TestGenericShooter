using SFML.Graphics;
using SFML.Window;
using SFMLStart.Data;
using SFMLStart.Utilities;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CRender : Component
    {
        private readonly GSGame _game;
        private readonly CPosition _cPosition;
        private readonly string _textureName;
        private readonly string _tilesetName;
        private readonly string _labelName;
        private readonly float _rotation;

        public CRender(GSGame mGame, CPosition mCPosition, string mTextureName, string mTilesetName = null, string mLabelName = null, float mRotation = 0)
        {
            _game = mGame;
            _cPosition = mCPosition;
            _textureName = mTextureName;
            _tilesetName = mTilesetName;
            _labelName = mLabelName;
            _rotation = mRotation;
        }

        public Sprite Sprite { get; set; }
        public Animation Animation { get; set; }
        public float Torque { get; set; }

        private void Draw() { _game.GameWindow.RenderWindow.Draw(Sprite); }
        
        public override void Added()
        {
            var x = _cPosition.X.ToPixels();
            var y = _cPosition.Y.ToPixels();

            Sprite = new Sprite(Assets.GetTexture(_textureName));
            if (_tilesetName != null && _labelName != null) Sprite.TextureRect = Assets.Tilesets[_tilesetName].GetTextureRect(_labelName);
            Sprite.Rotation = _rotation;
            Sprite.Position = new Vector2f(x, y);
            Sprite.Origin = new Vector2f(Sprite.GetGlobalBounds().Width / 2, Sprite.GetGlobalBounds().Height / 2); 

            _game.OnDrawAfterCamera += Draw;
        }
        public override void Update(float mFrameTime)
        {
            var x = _cPosition.X.ToPixels();
            var y = _cPosition.Y.ToPixels();                 

            Sprite.Position = new Vector2f(x, y);
            Sprite.Origin = new Vector2f(Sprite.GetGlobalBounds().Width / 2, Sprite.GetGlobalBounds().Height / 2);
            Sprite.Rotation += Torque*mFrameTime;

            if (Animation == null) return;
            Animation.Update(mFrameTime);
            Sprite.TextureRect = Assets.Tilesets[_tilesetName].GetTextureRect(Animation.GetCurrentLabel());
        }
        public override void Removed() { _game.OnDrawAfterCamera -= Draw; }
    }
}