using SFMLStart.Vectors;
using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CPosition : Component
    {
        public CPosition(SSVector2I mPosition) { Position = mPosition; }

        public SSVector2I Position { get; set; }
        public int X { get { return Position.X; } }
        public int Y { get { return Position.Y; } }
    }
}