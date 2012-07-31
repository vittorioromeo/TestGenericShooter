using VeeEntitySystem2012;

namespace TestGenericShooter.Components
{
    public class CHealth : Component
    {
        private int _health;

        public CHealth(int mHealth) { Health = mHealth; }

        public int Health
        {
            get { return _health; }

            set
            {
                _health = value;
                if (_health <= 0) Entity.Destroy();
            }
        }

        public static CHealth operator ++(CHealth mCHealth)
        {
            mCHealth.Health++;
            return mCHealth;
        }
        public static CHealth operator --(CHealth mCHealth)
        { 
            mCHealth.Health--;
            return mCHealth;
        }
        public static CHealth operator +(CHealth mCHealth, int mValue)
        {
            mCHealth.Health += mValue;
            return mCHealth;
        }
        public static CHealth operator -(CHealth mCHealth, int mValue)
        {
            mCHealth.Health -= mValue;
            return mCHealth;
        }
    }
}