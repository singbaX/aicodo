/* 
 * author      : singba singba@163.com 
 * version     : 20161226
 * source      : AF.Core
 * license     : free use or modify
 * description : 键值对基类
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiCodo
{
    public class KeyValueItem<T> : EntityBase
    {
        #region 属性 Key
        private string _Key = string.Empty;
        public string Key
        {
            get
            {
                return _Key;
            }
            set
            {
                _Key = value;
                RaisePropertyChanged("Key");
            }
        }
        #endregion

        #region 属性 Value
        private T _Value = default(T);
        public T Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                RaisePropertyChanged("Value");
            }
        }
        #endregion

        public KeyValueItem()
        {

        }

        public KeyValueItem(string key, T value)
        {
            Key = key;
            Value = value;
        }
    }

    public class KeyValueItem : KeyValueItem<string>
    {
    }

    public class KeyValueCollection : CollectionBase<KeyValueItem>
    {
        #region 属性 StringValues

        private string _StringValues = "";

        public string StringValues
        {
            get { return _StringValues; }
            set
            {
                _StringValues = value;
                ResetItems();
            }
        }

        #endregion

        public KeyValueItem this[string key]
        {
            get
            {
                return GetItem(key);
            }
        }

        private KeyValueItem GetItem(string key, string defaultValue = "")
        {
            var item = Items.FirstOrDefault(f => f.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (item == null)
            {
                item = new KeyValueItem { Key = key, Value = defaultValue };
                this.Add(item);
            }
            return item;
        }

        public string GetValue(string key, string defaultValue = "")
        {
            var item = GetItem(key, defaultValue);
            return item.Value;
        }

        private void ResetItems()
        {
            this.Clear();
            foreach (KeyValueItem item in StringValues.ToKeyValues('|', ':'))
            {
                this.Add(item);
            }
        }
    }
    public static class KeyValueHelper
    {
        public static IEnumerable KeyValueDataSource(string[] keyValues)
        {
            if (keyValues == null || keyValues.Length == 0)
            {
                yield break;
            }
            foreach (string item in keyValues)
            {
                string[] kv = item.Split(':');
                var kitem = new KeyValueItem();
                if (kv.Length > 0)
                {
                    kitem.Key = kv[0];
                }
                if (kv.Length > 1)
                {
                    kitem.Value = kv[1];
                }
                yield return kitem;
            }
        }

        public static IEnumerable<KeyValueItem> ToKeyValues(this string keyValues, char itemSplit = '|', char split = ':')
        {
            if (string.IsNullOrEmpty(keyValues))
                yield break;
            foreach (string item in keyValues.Split(itemSplit))
            {
                string[] kv = item.Split(split);
                var kitem = new KeyValueItem();
                if (kv.Length > 0)
                {
                    kitem.Key = kv[0];
                }
                if (kv.Length > 1)
                {
                    kitem.Value = kv[1];
                }
                yield return kitem;
            }
        }

        public static string ToString(this IEnumerable<KeyValueItem> keyValues, char itemSplit = '|', char split = ':')
        {
            var sb = new StringBuilder();
            foreach (KeyValueItem item in keyValues)
            {
                sb.AppendFormat("{0}{1}{2}{3}", item.Key, split, item.Value, itemSplit);
            }
            return sb.ToString();
        }
    }
}

