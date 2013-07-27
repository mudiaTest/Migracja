using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Migracja
{
    //na razie nie jest rozbudowany, ale bedzie potem
    class GeoPoint
    {
        private float X;
        private float Y;
        public GeoPoint() { }
        public GeoPoint(float x, float y)
        {
            X = x;
            Y = y;
        }
        public Point ToPoint()
        {
            return new Point(RasterToVector_Utils.Round(X), RasterToVector_Utils.Round(Y));
        }
        public float GetX()
        {
            return X;
        }
        public float GetY()
        {
            return Y;
        }
    }
}
