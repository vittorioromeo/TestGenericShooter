using System.Collections.Generic;
using SFML.Graphics;
using SFMLStart.Vectors;
using TestGenericShooter.Resources;
using VeeCollision;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CShadower: Component
    {
        private readonly GSGame _game;
        private readonly CBody _cBody;
        private readonly List<Vertex[]> _vertexArrays;
        private readonly List<Polygon> _polygons; 

        public CShadower(GSGame mGame, CBody mCBody)
        {
            _game = mGame;
            _cBody = mCBody;
            _vertexArrays = new List<Vertex[]>();
            _polygons = new List<Polygon>();

            _game.AddDrawAction(Draw, -10);
        }

        private SSVector2I ProjectPoint(SSVector2I mPoint)
        {
            var light = new SSVector2I(_cBody.X, _cBody.Y);
            var lightToPoint = mPoint - light;
            return mPoint + lightToPoint * 1500;
        }
        private void ShadowLine(SSVector2I mPoint1, SSVector2I mPoint2)
        {
            var polygon = new Polygon(mPoint1, ProjectPoint(mPoint1),  ProjectPoint(mPoint2), mPoint2);
            _polygons.Add(polygon);
            _vertexArrays.Add(polygon.GetVertexArray());
        }

        public IEnumerable<Polygon> GetPolygons() { return _polygons; }

        public override void Update(float mFrameTime)
        {
            _polygons.Clear();
            _vertexArrays.Clear();
            
            foreach (var wall in Manager.GetEntitiesByTag(Tags.Wall))
            {
                var body = wall.GetComponentUnSafe<CBody>();
                var point1 = new SSVector2I();
                var point2 = new SSVector2I();

                if(_cBody.X >= body.Right)
                {
                    point1 = new SSVector2I(body.Right, body.Top);

                    if(_cBody.Y <= body.Bottom && _cBody.Y >= body.Top)
                    {
                        point1 = new SSVector2I(body.Right, body.Top);
                        point2 = new SSVector2I(body.Right, body.Bottom);
                    }
                    else if(_cBody.Y >= body.Bottom)
                    {
                        point1 = new SSVector2I(body.Right, body.Top);
                        point2 = new SSVector2I(body.Left, body.Bottom);
                    }
                    else if(_cBody.Y <= body.Top)
                    {
                        point1 = new SSVector2I(body.Right, body.Bottom);
                        point2 = new SSVector2I(body.Left, body.Top);
                    }
                }
                else if (_cBody.X <= body.Left)
                {
                    if (_cBody.Y <= body.Bottom && _cBody.Y >= body.Top)
                    {
                        point1 = new SSVector2I(body.Left, body.Top);
                        point2 = new SSVector2I(body.Left, body.Bottom);
                    }
                    else if (_cBody.Y >= body.Bottom)
                    {
                        point1 = new SSVector2I(body.Right, body.Bottom);
                        point2 = new SSVector2I(body.Left, body.Top);
                    }
                    else if (_cBody.Y <= body.Top)
                    {
                        point1 = new SSVector2I(body.Right, body.Top);
                        point2 = new SSVector2I(body.Left, body.Bottom);
                    }
                }
                else if (_cBody.X >= body.Left && _cBody.X <= body.Right)
                {
                    if (_cBody.Y >= body.Bottom)
                    {
                        point1 = new SSVector2I(body.Right, body.Bottom);
                        point2 = new SSVector2I(body.Left, body.Bottom);
                    }
                    else if (_cBody.Y <= body.Top)
                    {
                        point1 = new SSVector2I(body.Right, body.Top);
                        point2 = new SSVector2I(body.Left, body.Top);
                    }
                }

                ShadowLine(point1, point2);
            }
        }
        public void Draw()
        {
            foreach (var array in _vertexArrays)
                _game.GameWindow.RenderWindow.Draw(array, PrimitiveType.Quads);
        }
    }
}