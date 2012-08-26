using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using SFMLStart.Vectors;

namespace TestGenericShooter.Components
{
    public class Polygon : IEnumerable<SSVector2I>
    {
        private readonly List<SSVector2I> _points;

        public Polygon(params SSVector2I[] mPoints) {_points = mPoints.ToList();  }

        public Vertex[] GetVertexArray()
        {
            var result = new Vertex[_points.Count];

            for (var i = 0; i < _points.Count; i++)
            {
                var point = _points[i];
                result[i] = new Vertex(new Vector2f(point.X.ToPixels(), point.Y.ToPixels())){Color = new Color(255,100,100,255)};
            }

            return result;
        }
        public bool IsIntersecting(SSVector2I mPoint)
        {
            int i;
            double angle = 0;

            for (i = 0; i < _points.Count; i++)
            {
                var p1 = new SSVector2I(_points[i].X - mPoint.X, _points[i].Y - mPoint.Y);
                var p2 = new SSVector2I(_points[(i + 1) % _points.Count].X - mPoint.X, _points[(i + 1) % _points.Count].Y - mPoint.Y);
                angle += Angle2D(p1.X, p1.Y, p2.X, p2.Y);
            }

            return !(Math.Abs(angle) < Math.PI);
        }

        public IEnumerator<SSVector2I> GetEnumerator() { return _points.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator()  { return GetEnumerator(); }

        private static double Angle2D(double x1, double y1, double x2, double y2)
        {
            var theta1 = Math.Atan2(y1, x1);
            var theta2 = Math.Atan2(y2, x2);
            var dtheta = theta2 - theta1;
            while (dtheta > Math.PI) dtheta -= Math.PI * 2;
            while (dtheta < -Math.PI) dtheta += Math.PI * 2;

            return (dtheta);
        }
    }
}