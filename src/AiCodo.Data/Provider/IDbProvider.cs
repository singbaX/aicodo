using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace AiCodo.Data
{
    public interface IAlterTable
    {
        string CreateChangeColumn(string tableName, Column c, string afterName = "");
        string CreateAddColumn(string tableName, Column c, string afterName = "");
    }

    public interface IColumnDefaultValue
    {
        void SetDefaultValue(string dataType, string defaultValue);
        string GetDefaultValue(string dataType);
    }

    public interface IDbProvider
    {
        DbProviderFactory Factory { get; }

        string ResetQueryLimit(string sql, int from, int count);
        string ResetQueryTotal(string sql);

        IEnumerable<string> GetParameters(string commandText);

        TableSchema GetTableSchema(DbConnection db, string tableName);

        IEnumerable<string> GetTables(DbConnection db);

        IEnumerable<TableSchema> LoadTables(DbConnection db);

        #region DbCommand
        DbCommand CreateCommand(DbConnection db, string sql, params object[] nameValues);
        IEnumerable<T> ToItems<T>(IDataReader reader) where T : IEntity, new();

        string CreateFilter(ISqlFilter filter, IEntity parameters, string prefix, ref int pIndex);
        string CreateOrderBy(params Sort[] sort);

        #endregion

        string CreateConnectionString(DynamicEntity args);
        string CreateTable(TableSchema table);

        string CreateAlterTable(TableSchema table, TableSchema oldTable);

        string CreateInsert(TableSchema table,
            bool reload = false, bool reloadAutoColumnOnly = false);

        string CreateUpdate(TableSchema table);

        string CreateDelete(TableSchema table);

        string CreateCount(TableSchema table);

        string CreateSelect(TableSchema table);

        string CreateSelect(TableSchema table, string where);

        string CreateSelectByKeys(TableSchema table);

        string CreateView(string name, string select, bool reset = true);
    }
}
