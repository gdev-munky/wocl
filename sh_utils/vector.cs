using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WOCL.Shared.Utils
{
    public struct Vector3D
    {
        public double x, y, z;

        public double Length()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }
        public double ZLength()
        {
            return Math.Sqrt(x * x + y * y );
        }
        public double YLength()
        {
            return Math.Sqrt(x * x + z * z);
        }
        public double XLength()
        {
            return Math.Sqrt(y * y + z * z);
        }
        public double LengthSq()
        {
            return x * x + y * y + z * z;
        }

        public void Normalize()
        {
            var l = Length();
            x /= l;
            y /= l;
            z /= l;
        }
        public void Add(Vector3D a)
        {
            x += a.x;
            y += a.y;
            z += a.z;
        }
        public void Sub(Vector3D a)
        {
            x -= a.x;
            y -= a.y;
            z -= a.z;
        }
        public void Mul(Vector3D a)
        {
            x *= a.x;
            y *= a.y;
            z *= a.z;
        }
        public void Div(Vector3D a)
        {
            x /= a.x;
            y /= a.y;
            z /= a.z;
        }
        public void Mul(double a)
        {
            x *= a;
            y *= a;
            z *= a;
        }
        public void Div(double a)
        {
            x /= a;
            y /= a;
            z /= a;
        }

        public Vector3D(double X, double Y, double Z)
        {
            x = X; y = Y; z = Z;
        }

        public static Vector3D operator --(Vector3D a)
        {
            var l = a.Length();
            return new Vector3D(a.x / l, a.y / l, a.z / l);
        }
        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static Vector3D operator *(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static Vector3D operator /(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.x / b.x, a.y / b.y, a.z / b.z);
        }
        public static Vector3D operator *(Vector3D a, double b)
        {
            return new Vector3D(a.x * b, a.y * b, a.z * b);
        }
        public static Vector3D operator /(Vector3D a, double b)
        {
            return new Vector3D(a.x / b, a.y / b, a.z / b);
        }

        public static bool operator ==(Vector3D a, Vector3D b)
        {
            return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
        }
        public static bool operator !=(Vector3D a, Vector3D b)
        {
            return (a.x != b.x) || (a.y != b.y) || (a.z != b.z);
        }

        public static explicit operator double(Vector3D a)
        {
            return a.Length();
        }
        public static explicit operator int(Vector3D a)
        {
            return (int)a.Length();
        }

        public static readonly Vector3D NULL = new Vector3D(0, 0, 0);
        public static readonly Vector3D UP = new Vector3D(0, 0, 1);
        public static readonly Vector3D DOWN = new Vector3D(0, 0, -1);
        public static readonly Vector3D RIGHT = new Vector3D(1, 0, 0);
        public static readonly Vector3D LEFT = new Vector3D(-1, 0, 0);
        public static readonly Vector3D FORW = new Vector3D(0, 1, 0);
        public static readonly Vector3D BACK = new Vector3D(0, -1, 0);

        public override string ToString()
        {
            return x + "; " + y + "; " + z;
        }
        public void RotateX(double add)
        {
            var l = XLength();
            if (l <= 0) return;
            var acos = Math.Acos(y / l);
            var asin = Math.Asin(z / l);
            double a = add;
            if (y > 0 && z > 0)
                a += acos;
            else if (y < 0 && z > 0)
                a += acos;
            else if (y < 0 && z < 0)
                a += 2 * Math.PI - acos;
            else
                a += asin;
            y = Math.Cos(a) * l;
            z = Math.Sin(a) * l;
        }
        public void RotateY(double add)
        {
            var l = YLength();
            if (l <= 0) return;
            var acos = Math.Acos(x / l);
            var asin = Math.Asin(z / l);
            double a = add;
            if (x > 0 && z > 0)
                a += acos;
            else if (x < 0 && z > 0)
                a += acos;
            else if (x < 0 && z < 0)
                a += 2 * Math.PI - acos;
            else
                a += asin;
            x = Math.Cos(a) * l;
            z = Math.Sin(a) * l;
        }
        public void RotateZ(double add)
        {
            var l = ZLength();
            if (l <= 0) return;
            var acos = Math.Acos(x / l);
            var asin = Math.Asin(y / l);
            double a = add;
            if (x > 0 && y > 0)
                a += acos;
            else if (x < 0 && y > 0)
                a += acos;
            else if (x < 0 && y < 0)
                a += 2 * Math.PI - acos;
            else 
                a += asin;
            x = Math.Cos(a) * l;
            y = Math.Sin(a) * l;
        }

        public byte[] GetBytes()
        {
            var bts = new List<byte>();
            bts.AddRange(BitConverter.GetBytes(x));
            bts.AddRange(BitConverter.GetBytes(y));
            bts.AddRange(BitConverter.GetBytes(z));
            return bts.ToArray();
        }
        public void WriteBytesTo(BinaryWriter str)
        {
            str.Write(x);
            str.Write(y);
            str.Write(z);
        }
        public static Vector3D ReadFrom(BinaryReader str)
        {
            var vec = new Vector3D();
            vec.x = str.ReadDouble();
            vec.y = str.ReadDouble();
            vec.z = str.ReadDouble();
            return vec;
        }
    }
}
