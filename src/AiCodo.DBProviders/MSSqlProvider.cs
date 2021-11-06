using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace AiCodo
{
    #region Sql
    //public class MSSqlProvider : DbProvider
    //{
    //    DbProviderFactory _Factory = null;

    //    private MSSqlProvider()
    //    {

    //    }

    //    private static MSSqlProvider _Instance = new MSSqlProvider();
    //    public static MSSqlProvider Instance
    //    {
    //        get
    //        {
    //            return _Instance;
    //        }
    //    }

    //    protected override DbProviderFactory GetFactory()
    //    {
    //        return _Factory;
    //        //return System.Data.SqlClient.SqlClientFactory.Instance;
    //    }

    //    public override string GetLastIdentity()
    //    {
    //        return "scope_identity()";
    //    }

    //    public override string GetName(string columnName)
    //    {
    //        return $"[{columnName}]";
    //    }

    //    public override string GetParameter(string columnName)
    //    {
    //        return $"@{columnName}";
    //    }
    //}
    #endregion
}
