using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CPosition : Component
    {
        public CPosition(GSVector2 mPosition) { Position = mPosition; }
        public GSVector2 Position { get; set; }
    }
}