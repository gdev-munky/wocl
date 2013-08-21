using System;

namespace WOCL.Shared.Utils
{
    /// <summary>
    /// Color, described by 3 floats
    /// </summary>
    public class Clr3fl
    {
        //values that are exactly in [0..1] - used un this-class code in order to use direct calls w.o. checkings
        private float _r, _g, _b;
        //properties can be set to a non-[0..1] value, but will be corrected - used in unsafe places
        public float R
        {
            get { return _r; }
            set { _r = Math.Min(Math.Max(value, 0f), 1f); }
        }
        public float G
        {
            get { return _g; }
            set { _g = Math.Min(Math.Max(value, 0f), 1f); }
        }
        public float B
        {
            get { return _b; }
            set { _b = Math.Min(Math.Max(value, 0f), 1f); }
        }
        public Clr3fl(byte r, byte g, byte b)
        {
            _r = r / 255;
            _g = g / 255;
            _b = b / 255;
        }
        public Clr3fl(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }
        public Clr3fl(double r, double g, double b)
        {
            R = (float)r;
            G = (float)g;
            B = (float)b;
        }
        public Clr3fl(Vector3D v)
        {
            R = (float)v.x;
            G = (float)v.y;
            B = (float)v.z;
        }
        public Clr3fl(int packedARGB)
        {
            var bts = BitConverter.GetBytes(packedARGB);
            var A = (float)(bts[0] / 255);
            _r = bts[1] * A / 255;
            _g = bts[2] * A / 255;
            _b = bts[3] * A / 255;
        }
        public Clr3fl(byte[] packedARGB)
        {
            var A = (float)(packedARGB[0] / 255);
            _r = packedARGB[1] * A / 255;
            _g = packedARGB[2] * A / 255;
            _b = packedARGB[3] * A / 255;
        }
        /// <summary>
        /// Converts to 4 bytes (ARGB), interpretated as int32
        /// </summary>
        /// <returns></returns>
        public int PackColor()
        {
            return BitConverter.ToInt32(GetBytes(), 0);
        }
        /// <summary>
        /// Converts color to 4-bytes ARGB
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return new byte[4] { 1, (byte)(R * 255), (byte)(G * 255), (byte)(B * 255) };
        }

        public static Clr3fl operator *(Clr3fl a, Clr3fl b)
        {
            return new Clr3fl(a._r * b._r, a._g * b._g, a._b * b._b);
        }
        public void Mul(Clr3fl cOther)
        {
            _r *= cOther._r;
            _g *= cOther._g;
            _b *= cOther._b;
        }

        public static readonly Clr3fl NULL = new Clr3fl(0f, 0f, 0f);

        public void ModifyBrightness(double value)
        {
            if (Equals(0.0, value)) return;
            value = 1/value;
            R = (float) Math.Pow(R, value);
            G = (float) Math.Pow(G, value);
            B = (float) Math.Pow(B, value);
        }
        public void ModifyBrightness(float value)
        {
            if (Equals(0.0f, value)) return;
            value = 1/value;
            R = (float)Math.Pow(R, value);
            G = (float)Math.Pow(G, value);
            B = (float)Math.Pow(B, value);
        }
    }
}
