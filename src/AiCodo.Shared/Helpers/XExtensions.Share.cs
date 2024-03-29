﻿// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    public static partial class BaseHelper
    {
        public static T ToXObject<T>(this string xmlString)
        {
            if (string.IsNullOrEmpty(xmlString))
            {
                return default(T);
            }
            T item = (T)GetDeserializeObject(typeof(T), xmlString);
            return item;
        }

        public static object XCopy(this object obj)
        {
            return obj.ToXString().ToXObject(obj.GetType());
        }

        public static object ToXObject(this string xmlString, Type type)
        {
            object item = GetDeserializeObject(type, xmlString);
            return item;
        }

        private static object GetDeserializeObject(this Type objtype, string xmlstring)
        {
            if (string.IsNullOrEmpty(xmlstring))
                return null;

            try
            {
                System.Xml.Serialization.XmlSerializer deserializer = new System.Xml.Serialization.XmlSerializer(objtype);
                System.IO.StringReader textReader = new System.IO.StringReader(xmlstring);
                object obj = deserializer.Deserialize(textReader);
                textReader.Close();
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// serialize to xml
        /// </summary>
        /// <param name="xobj"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static string ToXString(this object xobj, string root)
        {
            return ToXmlString(xobj, root);
        }

        private static string ToXmlString(object obj, string root)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            System.IO.StringWriter textWriter = new System.IO.StringWriter();
            serializer.Serialize(textWriter, obj);

            string strtemp = textWriter.ToString();
            int index = strtemp.IndexOf("<" + root);
            return index > 0 ? strtemp.Substring(index) : strtemp;
        }

        /// <summary>
        /// serialize to xml
        /// </summary>
        /// <param name="xobj"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        public static string ToXString(this object xobj)
        {
            if (xobj == null)
            {
                return string.Empty;
            }
            string name = string.Empty;

            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(xobj.GetType());
            if (attrs.Where(a => a is System.Xml.Serialization.XmlRootAttribute).Count() > 0)
            {
                System.Xml.Serialization.XmlRootAttribute attr = (System.Xml.Serialization.XmlRootAttribute)attrs.Where(a => a is System.Xml.Serialization.XmlRootAttribute).FirstOrDefault();
                name = attr.ElementName;
            }
            else
            {
                int index = xobj.GetType().FullName.LastIndexOf('.');
                name = xobj.GetType().FullName.Substring(index >= 0 ? index + 1 : 0);
            }
            return ToXmlString(xobj, name);
        }

        #region 用反射找属性的相关扩展方法
        public static T ItemFind<T>(this IEnumerable<T> items, string findText)
        {
            if (items == null || items.Count() == 0)
            {
                return default(T);
            }
            var PInfos = typeof(T).GetProperties().Where(p =>
            {
#if SILVERLIGHT
                var attr = p.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault();
                if (attr == null || (attr as DisplayAttribute).AutoGenerateField == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
#else
                return true;
#endif
            });
            foreach (var item in items)
            {
                foreach (var p in PInfos)
                {
                    object obj = p.GetValue(item, null);
                    if (obj == null)
                    {
                        continue;
                    }
                    if (obj.ToString().IndexOf(findText) > -1)
                    {
                        return item;
                    }
                }
            }
            return default(T);
        }

        public static void CopyProperties(this object targetOjbect, object sourceObject, params string[] properties)
        {
            if (targetOjbect == null || sourceObject == null)
            {
                return;
            }
            foreach (var p in properties)
            {
                targetOjbect.SetPropertyValue(p, sourceObject.GetPathValue(p));
            }
        }

        public static T GetPropertyAttribute<T>(this object obj, string propertyName)
        {
            if (obj == null)
            { return default(T); }

            PropertyInfo p = obj.GetType().GetProperty(propertyName);
            if (p == null)
            {
                return default(T);
            }
            Type attrType = typeof(T);
            return (T)p.GetCustomAttributes(attrType, false).FirstOrDefault();
        }

        public static MethodInfo GetMethod(this object obj, string name)
        {
            return obj.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals(name));
        }

        public static object GetPathValue(this object obj, string path)
        {
            if (obj == null)
            {
                return null;
            }

            int index = path.IndexOf('.');
            if (index > 0)
            {
                var pvalue = obj.GetPathValue(path.Substring(0, index));
                if (pvalue == null)
                    return null;
                return pvalue.GetPathValue(path.Substring(index + 1));
            }
            else
            {
                MethodInfo method = obj.GetType().GetMethods().FirstOrDefault(m => m.Name.Equals("GetPropertyValue") &&
                    m.GetParameters().Count() == 1 && m.ReturnType == typeof(object));
                if (method != null)
                {
                    return method.Invoke(obj, new object[] { path });
                }

                PropertyInfo p = obj.GetType().GetProperty(path);
                if (p == null)
                {
                    return null;
                }
                return p.GetValue(obj, null);
            }
        }

        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            if (obj == null)
            { return; }

            PropertyInfo p = obj.GetType().GetProperty(propertyName);
            if (p == null)
            {
                return;
            }
            p.SetValue(obj, value, null);
        }

        public static bool SetPathValue(this object obj, string path, object value)
        {
            if (obj == null)
            {
                return false;
            }

            int index = path.IndexOf('.');
            if (index > 0)
            {
                var pvalue = obj.GetPathValue(path.Substring(0, index));
                if (pvalue == null)
                    return false;
                return pvalue.SetPathValue(path.Substring(index + 1), value);
            }
            else
            {
                if (obj is IEntity)
                {
                    (obj as IEntity).SetValue(path, value);
                    return true;
                }

                PropertyInfo p = obj.GetType().GetProperty(path);
                if (p == null)
                {
                    return false;
                }
                p.SetValue(obj, value, null);
                return true;
            }
        }

        #endregion
    }
}
