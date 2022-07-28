// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using System.Text;
    using System.IO;
    using System.Data;
    public static class JsonHelper
    {
        static JsonHelper()
        {
            _DefaultValues.Add(typeof(string), string.Empty);
            _DefaultValues.Add(typeof(int), 0);
            _DefaultValues.Add(typeof(long), 0);
            _DefaultValues.Add(typeof(Int16), 0);
            _DefaultValues.Add(typeof(double), 0);
            _DefaultValues.Add(typeof(DateTime), DateHelper.MinDate);
        }

        #region 类型默认值
        private static Dictionary<Type, object> _DefaultValues = new Dictionary<Type, object>();
        #endregion

        public static string CreateJson<T>(this IEnumerable<T> items) where T : IEntity
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                AppendObject<T>(jsonWriter, items);
            }
            return sb.ToString();
        }

        public static string CreateJson(this IEntity entity)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                AppendObject(jsonWriter, entity);
            }
            return sb.ToString();
        }


        private static void AppendObject<T>(JsonWriter jsonWriter, IEnumerable<T> items) where T : IEntity
        {
            jsonWriter.WriteStartArray();
            foreach (IEntity entity in items)
            {
                AppendObject(jsonWriter, entity);
            }
            jsonWriter.WriteEndArray();
        }

        private static void AppendObject(JsonWriter jsonWriter, IEntity entity)
        {
            jsonWriter.WriteStartObject();

            var nameValues = entity.GetNameValues();
            for (int i = 0; i < nameValues.Length - 1; i += 2)
            {
                jsonWriter.WritePropertyName(nameValues[i].ToString());
                jsonWriter.WriteValue(nameValues[i + 1]);
            }
            jsonWriter.WriteEndObject();
        }

        public static string CreateJson(this IDataReader reader)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.WriteStartArray();

                while (reader.Read())
                {
                    jsonWriter.WriteStartObject();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (!reader.IsDBNull(i))
                        {
                            jsonWriter.WritePropertyName(reader.GetName(i));
                            jsonWriter.WriteValue(reader.GetValue(i));
                        }
                    }
                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndArray();

            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                return json;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string ToFormatJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public static bool TryToJsonObject<T>(this string jsonString, out T obj)
        {
            try
            {
                obj = JsonConvert.DeserializeObject<T>(jsonString);
                return true;
            }
            catch
            {
                obj = default(T);
                return false;
            }
        }

        public static bool TryToJsonObject(this string jsonString, Type type, out object obj)
        {
            try
            {
                obj = JsonConvert.DeserializeObject(jsonString, type);
                return true;
            }
            catch
            {
                obj = null;
                return false;
            }
        }

        public static T ToJsonObject<T>(this string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static object ToJsonObject(this string jsonString, Type type)
        {
            return JsonConvert.DeserializeObject(jsonString, type);
        }
    }
}
