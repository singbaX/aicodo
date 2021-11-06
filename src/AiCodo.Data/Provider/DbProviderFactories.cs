using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace AiCodo.Data
{ 
    public static class DbProviderFactories
    {
        static Dictionary<string, IDbProvider> _Items
            = new Dictionary<string, IDbProvider>();

        static DbProviderFactories()
        {
            //SetFactory("mysql", MySqlProvider.Instance);
            //SetFactory("MySql.Data.MySqlClient", MySqlProvider.Instance);
            //SetFactory("sqlite", SqliteProvider.Instance);
            //SetFactory("sql", MSSqlProvider.Instance);
        }

        public static IEnumerable<string> GetProviderNames()
        {
            return _Items.Keys.ToList();
        }

        public static IDbProvider GetProvider(string name)
        {
            if (_Items.TryGetValue(name, out IDbProvider provider))
            {
                return provider;
            }
            return null;
        }

        public static DbProviderFactory GetFactory(string name)
        {
            if (_Items.TryGetValue(name, out IDbProvider provider))
            {
                return provider.Factory;
            }
            return null;
        }

        public static void SetFactory(string name, IDbProvider provider)
        {
            lock (_Items)
            {
                _Items[name] = provider;
                "DbProviderFactories".Log($"Add [{name}]={provider.GetType()}");
            }
        }
    }
}
