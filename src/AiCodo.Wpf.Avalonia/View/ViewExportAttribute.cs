using System;

namespace AiCodo
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ViewExportAttribute : Attribute
    {
        public ViewExportAttribute(string name,bool isDialog=false)
        {
            Name = name;
            IsDialog = isDialog;            
        }

        public string Name { get; }

        public bool IsDialog { get; }
    }
}
