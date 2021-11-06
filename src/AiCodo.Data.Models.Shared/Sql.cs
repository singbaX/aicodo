using System;
using System.Collections.Generic;
using System.Text;

namespace AiCodo.Data.Models
{
    public class ConnItem
    {
        public string Name { get; set; }

        public string ProviderName { get; set; }

        public string ConnectionString { get; set; }
    }

    public class TableItem
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

    }

    public class ColumnItem
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Comment { get; set; }
        public string ColumnType { get; set; }
        public bool NullAble { get; set; }
        public bool IsKey { get; set; }
    }
}
