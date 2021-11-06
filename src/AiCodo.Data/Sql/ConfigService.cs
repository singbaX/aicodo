using System;
using System.Collections.Generic;
using System.Text;

namespace AiCodo.Data
{
    public static class ConfigService
    {
        #region connection
        public static void CreateConnection(DynamicEntity data)
        {
            var name = data.GetString("name");
            if (name.IsNullOrEmpty())
            {
                throw new Exception("缺少必须参数[name]");
            }
            if (SqlData.Current.Connections[name] != null)
            {
                throw new Exception($"连接[{name}]已存在");
            }
            var providerName = data.GetString("provider");
            if (providerName.IsNullOrEmpty())
            {
                throw new Exception("缺少必须参数[provider]");
            }
            var provider = DbProviderFactories.GetProvider(providerName);
            if (provider == null)
            {
                throw new Exception($"没有对应的数据库处理[{providerName}]");
            }
            var connectionString = provider.CreateConnectionString(data);
            var sqlConn = new SqlConnection { Name = name, ProviderName = providerName, ConnectionString = connectionString };
            SqlData.Current.Connections.Add(sqlConn);
            sqlConn.ReloadTables();
            SqlData.Current.GenerateItems();
            SqlData.Current.Save();
        }
        #endregion
    }
}
