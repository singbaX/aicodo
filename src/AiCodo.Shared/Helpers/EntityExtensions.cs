﻿/* 
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
    using System.Text;
    using System.Linq;
    using Newtonsoft.Json;
    using System.IO;
    using System.Reflection;
    public static class EntityExtensions
    {
        static Dictionary<string, PropertyInfo> _EntityProperties
            = new Dictionary<string, PropertyInfo>();

        public static DynamicEntity ToDynamicEntity(this object[] nameValues)
        {
            return new DynamicEntity(nameValues);
        }

        /// <summary>
        /// 根据键、值更新原实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="newEntity"></param>
        public static void UpdateWith(this IEntity entity, IEntity newEntity, params string[] ignores)
        {
            if (newEntity != null)
            {
                var names = newEntity.GetFieldNames();
                if (ignores != null && ignores.Length > 0)
                {
                    names = names.Where(f => ignores.FirstOrDefault(n => n.Equals(f, StringComparison.OrdinalIgnoreCase)) == null);
                }

                var nameValues = new List<object>();
                foreach (var name in names)
                {
                    nameValues.Add(name);
                    nameValues.Add(newEntity.GetValue(name));
                }
                entity.UpdateWith(nameValues.ToArray());
            }
        }

        /// <summary>
        /// 根据键、值更新原实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="namevalues"></param>
        public static IEntity UpdateWith(this IEntity entity, object[] namevalues)
        {
            if (namevalues != null)
            {
                for (int i = 0; i < namevalues.Length - 1; i += 2)
                {
                    entity.SetValue(namevalues[i].ToString(), namevalues[i + 1]);
                }
            }
            return entity;
        }

        public static T CopyFrom<T>(this T obj, IEntity entity, string[] names) where T : IEntity
        {
            if (names != null)
            {
                names.ForEach(name => obj.SetValue(name, entity.GetValue(name)));
            }
            return obj;
        }

        public static T Create<T>(this IEntity entity, params string[] names) where T : IEntity, new()
        {
            T item = new T();
            if (entity != null)
            {
                if(names==null || names.Length == 0)
                {
                    names = entity.GetFieldNames().ToArray();
                }
                foreach (var name in names)
                {
                    var value = entity.GetValue(name, null);
                    if (value != null)
                    {
                        item.SetValue(name, value);
                    }
                }
            }
            return item;
        }

        /// <summary>
        /// 更新对象实体列表，如果collection中有相同的元素则执行更新，如果collection中没有相同元素则执行添加
        /// </summary>
        /// <typeparam name="T">对象实体类型</typeparam>
        /// <param name="collection">对象实体列表</param>
        /// <param name="newItems">新的对象实体列表</param>
        /// <param name="getOldItem">从collection中取相同的元素的函数</param>
        public static void UpdateWith<T>(this ICollection<T> collection, IEnumerable<T> newItems, Func<T, T> getOldItem = null) where T : IEntity, new()
        {
            foreach (var item in newItems)
            {
                var oldItem = getOldItem(item);
                if (oldItem != null)
                {
                    oldItem.UpdateWith(item);
                }
                else
                {
                    collection.Add(item);
                }
            }
        }

        public static string ServiceSerialize(this IEntity entity)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;

                jsonWriter.WriteStartObject();
                foreach (var name in entity.GetFieldNames())
                {
                    jsonWriter.WritePropertyName(name);
                    jsonWriter.WriteValue(entity.GetValue(name));
                }
                jsonWriter.WriteEndObject();
            }
            return sb.ToString();
        }

        public static string ServiceDeserialize<T>(this T entity) where T : IEntity, new()
        {
            return null;
        }

        public static T Update<T>(this T entity, string pName, object value, Action onChanged) where T : class
        {
            if (entity is IEntity obj)
            {
                var ovalue = obj.GetValue(pName);
                if (ovalue == value)
                {
                }
                else
                {
                    obj.SetValue(pName, value);
                    onChanged?.Invoke();
                }
                return entity;
            }

            var key = $"{typeof(T).FullName}_{pName}";

            if (!_EntityProperties.TryGetValue(key, out PropertyInfo p))
            {
                lock (_EntityProperties)
                {
                    if (!_EntityProperties.TryGetValue(key, out p))
                    {
                        p = typeof(T).GetProperty(pName);
                        _EntityProperties[key] = p;
                    }
                }
            }

            var oldValue = p.GetValue(entity);
            if (oldValue == value)
            {
            }
            else
            {
                p.SetValue(entity, value);
                onChanged?.Invoke();
            }
            return entity;
        }
    }
}
