/* 
 * author      : singba 
 * email       : singba@163.com 
 * version     : 20210831
 * package     : AiCodo
 * license     : MIT
 * description : let me think a while
 */
namespace AiCodo
{
    using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;
    using Newtonsoft.Json.Linq;
    using Newtonsoft.Json;
    using System.Collections.ObjectModel;
    /// <summary>
    /// 动态类
    /// </summary>
    [JsonConverter(typeof(DynamicExpandoJsonConverter<DynamicExpando>))]
    public class DynamicExpando : System.Dynamic.DynamicObject, IDictionary<string, object>, IEntity
    {
        #region 实现动态操作
        IDictionary<string, object> data = new Dictionary<string, object>();

        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            SetValue(binder.Name, value);
            return true;
        }

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            result = GetValue(binder.Name, null);
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return data.Keys;
        }

        public virtual IEnumerable<string> GetFieldNames()
        {
            return data.Keys;
        }

        public virtual object[] GetNameValues()
        {
            return data.ToNameValues();
        }

        /// <summary>
        /// 这个命令不会触发PropertyChanged事件
        /// </summary>
        /// <param name="nameValues"></param>
        public virtual void SetNameValues(params object[] nameValues)
        {
            if (nameValues == null || nameValues.Length == 0)
            {
                return;
            }
            for (int i = 0; i < nameValues.Length - 1; i += 2)
            {
                //SetValue(nameValues[i].ToString(), nameValues[i + 1]);
                data[nameValues[i].ToString()] = nameValues[i + 1];
            }
        }

        public virtual void SetValue(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            data[key] = value is DBNull ? null : value;
            RaisePropertyChanged(key);
        }

        public virtual object GetValue(string key, object defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            if (data.TryGetValue(key, out object dataValue))
            {
            }

            if (dataValue == null && defaultValue != null)
            {
                data[key] = defaultValue;
                dataValue = defaultValue;
                RaisePropertyChanged(key);
            }

            return dataValue;
        }

        public virtual void RemoveKey(string key)
        {
            if (data.ContainsKey(key))
            {
                data.Remove(key);
            }
        }

        public IReadOnlyDictionary<string, object> GetData()
        {
            return new ReadOnlyDictionary<string, object>(data);
        }
        #endregion

        #region IDictionary<string, object> members
        public void Add(string key, object value)
        {
            SetValue(key, value);
        }

        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get
            {
                return data.Keys;
            }
        }

        public bool Remove(string key)
        {
            return data.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return data.TryGetValue(key, out value);
        }

        [JsonIgnore]
        [System.Xml.Serialization.XmlIgnore()]
        public ICollection<object> Values
        {
            get
            {
                return data.Values;
            }
        }

        public object this[string key]
        {
            get
            {
                return GetValue(key);
            }
            set
            {
                SetValue(key, value);
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return data.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            data.CopyTo(array, arrayIndex);
        }

        [System.Xml.Serialization.XmlIgnore()]
        public int Count
        {
            get
            {
                return data.Count;
            }
        }

        [System.Xml.Serialization.XmlIgnore()]
        public bool IsReadOnly
        {
            get
            {
                return data.IsReadOnly;
            }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return data.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var item in data)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in data)
            {
                yield return item;
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private static Dictionary<Type, Dictionary<string, System.Reflection.PropertyInfo>> _DynamicTypeProperties = new Dictionary<Type, Dictionary<string, System.Reflection.PropertyInfo>>();

        protected virtual void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                #region 缓存动态类型的属性
                var type = this.GetType();
                var item = _DynamicTypeProperties.GetDictionaryValue(type, null);
                if (item == null)
                {
                    lock (_DynamicTypeProperties)
                    {
                        if (_DynamicTypeProperties.ContainsKey(type))
                        {
                            item = _DynamicTypeProperties[type];
                        }
                        else
                        {
                            var properties =
                                type.GetProperties(System.Reflection.BindingFlags.Public)
                                .ToDictionary((p) => p.Name);
                            _DynamicTypeProperties[type] = properties;
                            item = properties;
                        }
                    }
                }
                #endregion

                if (item.ContainsKey(name))
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
                }
                else
                {
                    PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs($"Item[]"));
                }
            }
        }
        #endregion

        //只拷贝属性，属性值如果是引用还是引用（对象）
        public T Copy<T>() where T : IEntity, new()
        {
            var t = new T();
            foreach (var item in data)
            {
                t.SetValue(item.Key, item.Value);
            }
            return t;
        }

        public virtual string ToJson()
        {
            return JsonHelper.ToJson(this);
        }

        public override string ToString()
        {
            return this.ToJson();
        }
    }

    public class DynamicExpandoJsonConverter<T> : JsonConverter where T : DynamicExpando, new()
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = null;
            try
            {
                jsonObject = JObject.Load(reader);
                return CreateDynamicExpando(jsonObject);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        private static object CreateDynamicExpando(JObject jsonObject)
        {
            var properties = jsonObject.Properties().ToList();
            var d = new T();
            foreach (var p in properties)
            {
                switch (p.Value.Type)
                {
                    case JTokenType.Array:
                        List<object> items = new List<object>();
                        var values = p.Value as JArray;
                        foreach (var pitem in values)
                        {
                            if (pitem is JValue)
                            {
                                items.Add(pitem);
                            }
                            else if (pitem is JObject)
                            {
                                var ditem = CreateDynamicExpando(pitem as JObject);
                                items.Add(ditem);
                            }
                        }
                        d.SetValue(p.Name, items);
                        break;
                    case JTokenType.Object:
                        d.SetValue(p.Name, CreateDynamicExpando(p.Value as JObject));
                        break;
                    //case JTokenType.Constructor: 
                    //case JTokenType.None:
                    //case JTokenType.Property:
                    //case JTokenType.Comment:
                    //case JTokenType.Integer:
                    //case JTokenType.Float:
                    //case JTokenType.String:
                    //case JTokenType.Boolean:
                    //case JTokenType.Null:
                    //case JTokenType.Undefined:
                    //case JTokenType.Date:
                    //case JTokenType.Raw:
                    //case JTokenType.Bytes:
                    //case JTokenType.Guid:
                    //case JTokenType.Uri:
                    //case JTokenType.TimeSpan:
                    default:
                        var value = p.Value;
                        if (value is JValue)
                        {
                            var pvalue = (value as JValue).Value;
                            if (pvalue is long)
                            {
                                if ((long)pvalue < int.MaxValue && (long)pvalue > int.MinValue)
                                {
                                    d.SetValue(p.Name, Convert.ToInt32(pvalue));
                                    break;
                                }
                            }
                            d.SetValue(p.Name, pvalue);
                        }
                        else
                        {
                            d.SetValue(p.Name, p.Value.ToString());
                        }
                        break;
                }
            }
            return d;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var d = value as T;

            writer.WriteStartObject();
            foreach (var name in d.GetDynamicMemberNames())
            {
                writer.WritePropertyName(name);
                serializer.Serialize(writer, d.GetValue(name));
            }
            writer.WriteEndObject();
        }
    }
}