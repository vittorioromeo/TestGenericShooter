namespace TestGenericShooter
{
    public struct GSVector2
    {
        private int _x;
        private int _y;
        public GSVector2(int mX, int mY)
        {
            _x = mX;
            _y = mY;
        }

        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }

        public override string ToString() { return string.Format("X:{0} Y:{1}", _x, _y); }

        public static GSVector2 operator +(GSVector2 mVector1, GSVector2 mVector2) { return new GSVector2(mVector1.X + mVector2.X, mVector1.Y + mVector2.Y); }
        public static GSVector2 operator -(GSVector2 mVector1, GSVector2 mVector2) { return new GSVector2(mVector1.X - mVector2.X, mVector1.Y - mVector2.Y); }
        public static GSVector2 operator *(GSVector2 mVector, float mScalar) { return new GSVector2((int) (mVector.X*mScalar), (int) (mVector.Y*mScalar)); }
    }
}