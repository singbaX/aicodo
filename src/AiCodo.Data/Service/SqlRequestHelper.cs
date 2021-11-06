using System;
using System.Collections.Generic;
using System.Text;

namespace AiCodo.Data
{
    public static class SqlRequestHelper
    {
        public static object Execute(this ISqlRequest request)
        {
            return SqlService.ExecuteSql(request.SqlName, request.GetNameValues());
        }
    }
}
