using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AppFactory.Runtime.Server.Providers.SQLServerCache
{
    public static class CacheUtility
    {
        // Convert an object to a byte array
        public static byte[] SerializeToByteArray(object obj)
        {
            try
            {
                if (obj == null)
                {
                    return null;
                }
                var bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, obj);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T Deserialize<T>(byte[] byteArray)
        {
            try
            {
                if (byteArray != null)
                {
                    using (var memStream = new MemoryStream())
                    {
                        var binForm = new BinaryFormatter();
                        memStream.Write(byteArray, 0, byteArray.Length);
                        memStream.Seek(0, SeekOrigin.Begin);
                        var obj = binForm.Deserialize(memStream);
                        return (T)obj;
                    }
                }
                return default(T);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static object Deserialize(byte[] byteArray)
        {
            return Deserialize<object>(byteArray);
        }

    }
}
