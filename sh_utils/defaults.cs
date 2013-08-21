using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WOCL.Shared.Utils
{
    /// <summary>
    /// Represents some shared functions for strings.
    /// You are strongly encouraged to use this in order to prevent all sorts of discrepancies with strings
    /// Also, to change DefaultEncoding - please, change its value here and recompile. Do not change DefaultEncoding 
    /// in other places.
    /// </summary>
    public static class DefString
    {
        /// <summary>
        /// Default encoding for any conversions (in WoCL solution) between strings and byte arrays
        /// </summary>
        public static Encoding DefaultEncoding = Encoding.UTF8;
        /// <summary>
        /// Writes formated string to a Stream
        /// </summary>
        /// <param name="file">Stream</param>
        /// <param name="t">String to write</param>
        /// <param name="args">Arguments for format</param>
        public static void Write(BinaryWriter file, string t, params object[] args)
        {
            var bts = DefaultEncoding.GetBytes(string.Format(t, args));
            file.Write(bts.Length);
            file.Write(bts);
        }
        /// <summary>
        /// Tries to read a string from a stream
        /// (string have to be writen by DefString.Write with the same encoding)
        /// </summary>
        /// <param name="file">Stream</param>
        /// <returns></returns>
        public static string Read(BinaryReader file)
        {
            var c = file.ReadInt32();
            return DefaultEncoding.GetString(file.ReadBytes(c));
        }
    }
}
