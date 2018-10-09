using System.Text;
using Newtonsoft.Json;

namespace Core
{
    public static class ObjectExtensions
    {
        public static T Deserialize<T>(this byte[] bytes)
        {

            var json = Encoding.Default.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static byte[] Serialize(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var json = JsonConvert.SerializeObject(obj);
            return Encoding.ASCII.GetBytes(json);
        }
    }
}
