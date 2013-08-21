using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WOCL.Shared.Utils
{
    public class LangPack
    {
        #region STATIC_LANGPACK
        /// <summary>
        /// Default language pack (default is null)
        /// </summary>
        public static LangPack Default;
        /// <summary>
        /// Safely gets a value assosiated with a key; 
        /// Returns null, if does not contain such key
        /// </summary>
        /// <param name="key">Key string</param>
        /// <returns></returns>
        public static string Get(string key)
        {
            if (Default != null) return null;
            return Default[key];
        }
        /// <summary>
        /// Safe set. If key does not exist - creates new pair key=value;
        /// Otherwise changes existing value to specified
        /// </summary>
        /// <param name="key">Key string</param>
        /// <param name="value">Value string</param>
        public static void Set(string key, string value)
        {
            if (Default != null) return;
            Default[key] = value;
        }
        #endregion
        #region OBJECT_LANGPACK
        internal Dictionary<string, string> dict = new Dictionary<string, string>();

        public LangPack(Stream str)
        {
            var file = new BinaryReader(str);
            var count = file.ReadInt32();
            for (var id = 0; id < count; id++)
            {
                var key = DefString.Read(file);
                var value = DefString.Read(file);
                if (dict.ContainsKey(key))
                {
                    LogManager.Post("[WARN] Duplicated key '{0}' - overwriting", key);
                    dict[key] = value;
                }
                else
                    dict.Add(key, value);
            }
        }
        /// <summary>
        /// Adds dictionary from stream without overwriting existing keys(keeps old)
        /// </summary>
        /// <param name="str">Stream (BinaryReader convertable)</param>
        public void Append(Stream str)
        {
            var file = new BinaryReader(str);
            var count = file.ReadInt32();
            for (var id = 0; id < count; id++)
            {
                var key = DefString.Read(file);
                var value = DefString.Read(file);
                if (!dict.ContainsKey(key))
                    dict.Add(key, value);
            }
        }
        /// <summary>
        /// Adds dictionary from stream with overwriting existing keys without log warnings
        /// </summary>
        /// <param name="str">Stream (BinaryReader convertable)</param>
        public void AddOverwrite(Stream str)
        {
            var file = new BinaryReader(str);
            var count = file.ReadInt32();
            for (var id = 0; id < count; id++)
            {
                var key = DefString.Read(file);
                var value = DefString.Read(file);
                if (dict.ContainsKey(key))
                    dict[key] = value;
                else
                    dict.Add(key, value);
            }
        }
        /// <summary>
        /// Clears dictionary
        /// </summary>
        public void Clear()
        {
            dict.Clear();
        }

        public string this[string key]
        {
            get
            {
                if (dict.ContainsKey(key)) return dict[key];
                return null;
            }
            set
            {
                if (dict.ContainsKey(key))
                    dict[key] = value;
                else
                    dict.Add(key, value);
            }
        }
        #endregion
    }
}
