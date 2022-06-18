/* 
 * author      : singba singba@163.com 
 * version     : 20161221
 * source      : AF.Wpf
 * license     : free use or modify
 * description : 页面模型的MEF输出属性
 */
using System; 

namespace AiCodo
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ViewModelExportAttribute : Attribute
    {
        public ViewModelExportAttribute(string name) 
        {
            Name = name;
        }
        public string Name { get; }
    }
}
