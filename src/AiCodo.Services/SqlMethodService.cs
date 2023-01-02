// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
namespace AiCodo.Services
{
    using AiCodo.Data;
    using AiCodo.Flow.Configs;
    using System.Data.Common;
    public class SqlMethodService : IMethodService
    {
        public const string ConnectionArgName = "_Connection";

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

            if (item.SqlType == SqlType.Query && item.CanUsePage == true)
            {
                sqlItem.Parameters.Add(new ParameterItem { Name = "PageIndex", TypeName = "Int", DefaultValue = "0" });
                sqlItem.Parameters.Add(new ParameterItem { Name = "PageSize", TypeName = "Int", DefaultValue = "0" });
                sqlItem.Parameters.Add(new ParameterItem { Name = "OrderBy", TypeName = "String", DefaultValue = "" });
            }

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
            SqlConnectionContext connContext = GetConnContext(item, args);

            try
            {
                if (item.IsQueryOnly)
                {
                    if (item.SqlType == SqlType.Query && args.ContainsKey("PageSize") && args.ContainsKey("PageIndex"))
                    {
                        if (ExecuteQuery(item, args, out var result))
                        {
                            return result;
                        }
                    }

                    switch (item.SqlType)
                    {
                        case SqlType.QueryOne:
                            return connContext.Connection.ExecuteQuery<DynamicEntity>(item.CommandText, args.ToNameValues()).FirstOrDefault();
                        case SqlType.Query:
                            return connContext.Connection.ExecuteQuery<DynamicEntity>(item.CommandText, args.ToNameValues());
                        case SqlType.Execute:
                            return connContext.Connection.ExecuteNoneQuery(item.CommandText, args.ToNameValues());
                        case SqlType.Scalar:
                            return connContext.Connection.ExecuteScalar(item.CommandText, args.ToNameValues());
                        default:
                            break;
                    }
                }
                else
                {
                    if (connContext.Transaction == null)
                    {
                        var trans = connContext.Connection.BeginTransaction();
                        connContext.Transaction = trans;
                        connContext.Log($"[{connContext.Name}] BeginTransaction");
                    }
                    switch (item.SqlType)
                    {
                        case SqlType.QueryOne:
                            return connContext.Connection.ExecuteQuery<DynamicEntity>(connContext.Transaction, item, args.ToNameValues()).FirstOrDefault();
                        case SqlType.Query:
                            return connContext.Connection.ExecuteQuery<DynamicEntity>(connContext.Transaction, item, args.ToNameValues());
                        case SqlType.Execute:
                            return connContext.Connection.ExecuteNoneQuery(connContext.Transaction, item, args.ToNameValues());
                        case SqlType.Scalar:
                            return connContext.Connection.ExecuteScalar(connContext.Transaction, item, args.ToNameValues());
                        default:
                            break;
                    }
                    connContext.WaitCommitCount++;
                }
                connContext.RunCount++;
            }
            catch (Exception ex)
            {
                connContext.HasError = true;
                connContext.Error = ex;
                throw;
            }
            return null;
        }

        private static bool ExecuteQuery(SqlItem item, Dictionary<string, object> args, out QueryResult result)
        {
            var sql = new SqlContext(item);
            if (args.TryGetValue("PageIndex", out var indexValue) && args.TryGetValue("PageSize", out var sizeValue))
            {
                var pageIndex = indexValue.ToInt32();
                var pageSize = sizeValue.ToInt32();

                sql.SetPage(pageIndex, pageSize);
                if (args.TryGetValue("OrderBy", out object order))
                {
                    var orderBy = order.ToString();
                    if (orderBy.IsNotEmpty())
                    {
                        var sorts = orderBy.Split(',').Select(x => new Sort(x)).ToArray();
                        sql.SetSorts(sorts);
                    }
                }
                var values = new List<object>();
                foreach (var a in args)
                {
                    if (a.Key == "PageIndex" || a.Key == "PageSize" || a.Key == "OrderBy")
                    {
                        continue;
                    }
                    values.Add(a.Key);
                    values.Add(a.Value);
                }
                var paras = values.ToArray();
                var count = sql.GetTotalCount(paras);
                var items = sql.ExecuteQuery<DynamicEntity>(paras);
                result = new QueryResult
                {
                    Items = items.ToList(),
                    TotalCount = count,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                };
                return true;
            }
            result = null;
            return false;
        }

        private static SqlConnectionContext GetConnContext(SqlItem item, Dictionary<string, object> args)
        {
            if (!args.TryGetValue("_RootContext", out var context))
            {
                throw new ArgumentException($"没有找到FlowContext", "_RootContext");
            }
            if (context is FlowContext flowContext)
            {

            }
            else
            {
                throw new ArgumentException($"没有找到FlowContext", "_RootContext");
            }

            if (!flowContext.TryGetArg(ConnectionArgName, out var connItem))
            {
                var conn = GetConnection(item.ConnectionName);
                connItem = new SqlConnectionContext
                {
                    Name = item.ConnectionName,
                    Connection = conn
                };
                flowContext.SetArgs(ConnectionArgName, connItem);
            }
            if (connItem is SqlConnectionContext connContext)
            {
            }
            else
            {
                throw new Exception($"参数{ConnectionArgName}不是正确的类型");
            }

            return connContext;
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
            var p = Data.DbProviderFactories.GetFactory(connItem.ProviderName);
            if (p == null)
            {
                throw new Exception($"Provider not found ({connItem.ProviderName})");
            }
            var conn = p.CreateConnection();
            conn.ConnectionString = connItem.GetConnectionString();
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

    public class SqlConnectionContext
    {
        public string Name { get; set; } = "";

        public int RunCount { get; set; } = 0;

        public int WaitCommitCount { get; set; } = 0;

        public bool HasError { get; set; } = false;

        public Exception Error { get; set; } = null;

        public DbConnection Connection { get; set; }

        public DbTransaction Transaction { get; set; }

        public bool IsClosed { get; private set; } = false;

        public void End()
        {
            IsClosed = true;
            try
            {
                if (HasError)
                {
                    if (WaitCommitCount > 0 && Transaction != null)
                    {
                        Transaction.Rollback();
                    }
                    Connection.Close();
                }
                else
                {
                    if (WaitCommitCount > 0 && Transaction != null)
                    {
                        Transaction.Commit();
                    }
                    Connection.Close();
                }
            }
            catch (Exception ex)
            {
                ex.WriteErrorLog();
            }
        }
    }
}

