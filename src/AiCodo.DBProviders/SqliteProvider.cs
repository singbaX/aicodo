//using AI.Sql.Properties;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace AiCodo
{
    #region Sqlite 
    /*
    public class SqliteProvider : DbProvider
    {
        private SqliteProvider()
        {

        }

        private static SqliteProvider _Instance = new SqliteProvider();
        public static SqliteProvider Instance
        {
            get
            {
                return _Instance;
            }
        }

        protected override DbProviderFactory GetFactory()
        {
            return Microsoft.Data.Sqlite.SqliteFactory.Instance;
        }

        public override string GetLastIdentity()
        {
            return "last_insert_rowid()";
        }

        public override string GetName(string columnName)
        {
            return $"{columnName}";
        }

        public override string GetParameter(string columnName)
        {
            return $"@{columnName}";
        }
    }
    */
    #endregion



}
