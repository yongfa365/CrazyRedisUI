using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyRedisUI
{
    public static class JsonHelper
    {
        public static string JsonFormat(this string input)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(input), Formatting.Indented);
        }
    }
}
