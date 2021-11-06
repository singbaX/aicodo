using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace AiCodo.Data
{
    static class ConnectionHelper
    {
        public static DbDataAdapter CreateAdapter(this SqlConnection sqlConn)
        {
            var factory = GetProviderFactory(sqlConn.ProviderName);
            var adp = factory.CreateDataAdapter();
            return adp;
        }

        public static DbConnection Open(this SqlConnection sqlConn)
        {
            return Open(sqlConn.ProviderName, sqlConn.ConnectionString);
        }

        public static DbConnection Open(string providerName, string connectionString)
        {
            DbConnection conn = null;
            DbProviderFactory p = GetProviderFactory(providerName);

            conn = p.CreateConnection();
            conn.ConnectionString = connectionString;
            conn.Open();
            return conn;
        }

        public static DbProviderFactory GetProviderFactory(string providerName)
        {
            var p = DbProviderFactories.GetFactory(providerName);
            if (p == null)
            {
                throw new Exception($"Provider not found ({providerName})");
            }

            return p;
        }

        public static IEnumerable<TableSchema> LoadTables(this SqlConnection sqlConn)
        {
            var provider = DbProviderFactories.GetProvider(sqlConn.ProviderName);
            var conn = sqlConn.Open();
            foreach (var table in provider.LoadTables(conn).ToList())
            {
                yield return table;
            }
        }
    }
}
