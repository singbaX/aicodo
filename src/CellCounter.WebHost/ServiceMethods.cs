using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellCounter.WebHost
{
    public static class ServiceMethods
    {
        static Dictionary<string, Func<Dictionary<string, string>, string, object>> _Methods
            = new Dictionary<string, Func<Dictionary<string, string>, string, object>>();

        public static void AddMethod(string name, Func<Dictionary<string, string>, string, object> method)
        {
            _Methods[name] = method;
        }

        public static bool HasMethod(string name)
        {
            return _Methods.ContainsKey(name);
        }

        public static IEnumerable<string> GetMethods()
        {
            return _Methods.Keys.ToList();
        }

        internal static object Request(string methodName,Dictionary<string, string> parameters,string data)
        {
            if(_Methods.TryGetValue(methodName, out var method))
            {
                return method(parameters, data);
            }
            return null;
        }
        public static string ReadToEnd(this Stream stream)
        {
            if (stream == null)
            {
                return string.Empty;
            }
            using (var reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
            {
                return reader.ReadToEndAsync().Result;
            }
        }
    }
}
