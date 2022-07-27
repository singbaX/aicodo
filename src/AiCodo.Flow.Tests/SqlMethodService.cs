using AiCodo.Data;
using AiCodo.Flow.Configs;

namespace AiCodo.Tests
{
    public class SqlMethodService : IMethodService
    {
        #region 属性 Current
        private static SqlMethodService _Current = new SqlMethodService();
        public static SqlMethodService Current
        {
            get
            {
                return _Current;
            }
        }
        #endregion

        public IFunctionItem GetItem(string name)
        {
            if (TryGetSqlItem(name, out var table, out var item))
            {
                return CreateSqlFunctionItem(table, item);
            }
            return null;
        }

        private IFunctionItem CreateSqlFunctionItem(SqlTableGroup table, SqlItem item)
        {
            var sqlItem = new SqlFunctionItemConfig();
            sqlItem.SqlName = $"{table.Name}.{item.Name}";

            item.CommandText.GetParameters().Select(p => new ParameterItem
            {
                Name = p,
            }).AddToCollection(sqlItem.Parameters);

            sqlItem.ResultParameters.Add(new ResultParameter { Name = "Result", Type = ParameterType.Object, DisplayName = "执行结果" });
            return sqlItem;
        }

        private static bool TryGetSqlItem(string name, out SqlTableGroup table, out SqlItem item)
        {
            item = SqlData.Current.GetSqlItem(name);
            table = item?.Group;
            return item != null;
        }

        public IEnumerable<NameItem> GetItems()
        {
            foreach (var m in SqlData.Current.Groups)
            {
                foreach (var t in m.Items)
                {
                    foreach (var item in t.Items)
                    {
                        yield return new NameItem($"{t.Name}.{item.Name}", item.Name);
                    }
                }
            }
        }

        public IFunctionResult Run(string name, Dictionary<string, object> args)
        {
            if (TryGetSqlItem(name, out var table, out var item))
            {
                var data = RunSqlItem(item, args);
                var result = new FunctionResult { };
                result.Data.Add("Result", data);
                return result;
            }
            return new FunctionResult { ErrorCode = FunctionResult.NotFound, ErrorMessage = $"命令[{name}]不存在" };
        }

        public static object RunSqlItem(SqlItem item, Dictionary<string, object> args)
        {
            switch (item.SqlType)
            {
                case SqlType.QueryOne:
                    return GetConnection().ExecuteQuery<DynamicEntity>(item.CommandText, args.ToNameValues()).FirstOrDefault();
                case SqlType.Query:
                    return GetConnection().ExecuteQuery<DynamicEntity>(item.CommandText, args.ToNameValues());
                case SqlType.Execute:
                    return GetConnection().ExecuteNoneQuery(item.CommandText, args.ToNameValues());
                case SqlType.Scalar:
                    return GetConnection().ExecuteScalar(item.CommandText, args.ToNameValues());
                default:
                    break;
            }
            return null;
        }

        private static System.Data.Common.DbConnection GetConnection(string connName = "")
        {
            var connItem = connName.IsNullOrEmpty() ?
                SqlData.Current.Connections.FirstOrDefault() :
                SqlData.Current.Connections[connName];
            if (connItem == null)
            {
                throw new Exception($"Connection[{connName}] not found.");
            }
            var p = DbProviderFactories.GetFactory(connItem.ProviderName);
            if (p == null)
            {
                throw new Exception($"Provider not found ({connItem.ProviderName})");
            }
            var conn = p.CreateConnection();
            conn.ConnectionString = connItem.ConnectionString;
            conn.Open();
            return conn;
        }
    }

    public partial class SqlFunctionItemConfig : FunctionItemBase
    {
        #region 属性 SqlName
        private string _SqlName = string.Empty;
        public string SqlName
        {
            get
            {
                return _SqlName;
            }
            set
            {
                if (_SqlName == value)
                {
                    return;
                }
                _SqlName = value;
                RaisePropertyChanged("SqlName");
            }
        }
        #endregion
    }

}

