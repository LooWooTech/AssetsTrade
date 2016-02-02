using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LooWooTech.AssetsTrade.Common
{
    public static class XmlSerializeExtension
    {

        private static ConcurrentDictionary<Type, XmlSerializer> _cache;

        static XmlSerializeExtension()
        {
            _cache = new ConcurrentDictionary<Type, XmlSerializer>();
        }


        private static XmlSerializer GetSerializer<T>()
        {
            var type = typeof(T);
            return _cache.GetOrAdd(type, new XmlSerializer(type));
        }


        public static string XmlSerialize<T>(this T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                GetSerializer<T>().Serialize(memoryStream, obj);
                return Encoding.UTF8.GetString(memoryStream.GetBuffer());
            }
        }

        public static T XmlDeserialize<T>(this string xml)
        {
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                var obj = GetSerializer<T>().Deserialize(memoryStream);
                return obj == null ? default(T) : (T)obj;
            }
        }
    }
}
