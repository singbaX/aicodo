using System;
using System.Collections.Generic;
using System.Text;

namespace AiCodo.Data
{
    public class Sort
    {        
        public static Sort SortOfUpdateTime = new Sort { Name = "UpdateTime" };
        public static Sort SortOfUpdateTimeDesc = new Sort { Name = "UpdateTime", IsDesc = true };

        public string Name { get; set; }

        public bool IsDesc { get; set; } = false;

        public override string ToString()
        {
            return IsDesc ? $"{Name} DESC" : Name;
        }
    }
}
