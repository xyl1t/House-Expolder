using System;
using System.Drawing;

namespace OLC_CodeJam
{
    struct Vector
    {
        public double X;
        public double Y;

        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;

            PointF p = new Point(3, 2);
            p = p + this;
        }

        public Vector getNormalized()
        {
            if (this.X == 0 && this.Y == 0)
                return new Vector(0, 0);

            return this / this.getLength();
        }
        public double getLength()
        {
            return Math.Sqrt(X * X + Y * Y);
        }


        // Vector + Vector
        public static Vector operator +(Vector one, Vector two)
        {
            return new Vector(one.X + two.X, one.Y + two.Y);
        }
        // Point + Vector
        public static PointF operator +(PointF one, Vector two)
        {
            return new PointF(one.X + (float)two.X, one.Y + (float)two.Y);
        }
        // Vector + double
        public static Vector operator +(Vector one, double two)
        {
            Vector v;

            v.X = one.X + two;
            v.Y = one.Y + two;

            return v;
        }

        // Vector - Vector
        public static Vector operator -(Vector one, Vector two)
        {
            return new Vector(one.X - two.X, one.Y - two.Y);
        }
        // Point - Vector
        public static PointF operator -(PointF one, Vector two)
        {
            return new PointF(one.X - (float)two.X, one.Y - (float)two.Y);
        }
        // Vector - double
        public static Vector operator -(Vector one, double two)
        {
            return new Vector(one.X - two, one.Y - two);
        }

        // Vector * Vector
        public static Vector operator *(Vector one, Vector two)
        {
            return new Vector(one.X * two.X, one.Y * two.Y);
        }
        // Point * Vector
        public static PointF operator *(PointF one, Vector two)
        {
            return new PointF(one.X * (float)two.X, one.Y * (float)two.Y);
        }
        // Vector * double
        public static Vector operator *(Vector v1, double amount)
        {
            Vector v;

            v.X = v1.X * amount;
            v.Y = v1.Y * amount;

            return v;
        }

        // Vector / Vector
        public static Vector operator /(Vector one, Vector two)
        {
            return new Vector(one.X / two.X, one.Y / two.Y);
        }
        // Point / Vector
        public static PointF operator /(PointF one, Vector two)
        {
            return new PointF(one.X / (float)two.X, one.Y / (float)two.Y);
        }
        // Vector / double
        public static Vector operator /(Vector one, double two)
        {
            return new Vector(one.X / two, one.Y / two);
        }


        // Vector == Vector
        public static bool operator ==(Vector one, Vector two)
        {
            return (one.X == two.X && one.Y == two.Y);
        }
        // Vector != Vector
        public static bool operator !=(Vector one, Vector two)
        {
            return !(one == two);
        }

        // Vector == double
        public static bool operator ==(Vector one, double two)
        {
            return (one.X == two && one.Y == two);
        }

        // Vector != double
        public static bool operator !=(Vector one, double two)
        {
            return !(one == two);
        }


        public static double DotProduct(Vector v1, Vector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }
    }
}
