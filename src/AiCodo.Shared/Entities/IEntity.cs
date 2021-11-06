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
    using System.ComponentModel;
    public interface IEntity : INotifyPropertyChanged
    {
        //取得动态扩展属性的值
        object GetValue(string key, object defaultValue = null);
        //设置动态扩展属性的值
        void SetValue(string key, object value);

        void RemoveKey(string key);

        //取所有（动态）属性名称
        IEnumerable<string> GetFieldNames();
        //取所有（动态）属性的名称、值系列
        object[] GetNameValues();
    }
}
