using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACool.Library.DataProcess.Extension
{
    public static class JsonExt
    {
        public static string Order(this string Json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(Json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        public static T JsonDeserializeObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string JsonSerializeObject(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
