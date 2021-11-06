/* 
 * author      : singba 
 * email       : singba@163.com 
 * version     : 20210831
 * package     : AiCodo
 * license     : MIT
 * description : let me think a while
 */
namespace AiCodo.Data
{
    using System;
    using System.Collections.Generic;
    public static class SqlService
    {
        static object _LoadLock = new object();

        static SqlService()
        {
        }

        #region 属性 Config
        public static SqlData Config
        {
            get
            {
                return SqlData.Current;
            }
        }
        #endregion 

        public static object ExecuteSql(string sqlName, params object[] nameValues)
        {
            SqlItem sql = GetSqlItem(sqlName);
            switch (sql.SqlType)
            {
                case SqlType.Execute:
                    return sql.ExecuteNoneQuery(nameValues);
                case SqlType.Scalar:
                    return sql.ExecuteScalar(nameValues);
                case SqlType.Query:
                default:
                    return sql.ExecuteQuery<DynamicEntity>(nameValues);
            }
        }

        private static SqlItem GetSqlItem(string sqlName)
        {
            var config = Config;
            if (config == null)
            {
                throw new Exception("配置文件没有加载");
            }

            var sql = Config.GetSqlItem(sqlName);
            return sql;
        }

        public static IEnumerable<T> ExecuteQuery<T>(string sqlName, params object[] nameValues) where T : IEntity, new()
        {
            SqlItem sql = GetSqlItem(sqlName);
            if(sql == null)
            {
                throw new Exception($"命令[{sqlName}]不存在");
            }
            return sql.ExecuteQuery<T>(nameValues);
        }

        public static int ExecuteNoneQuery(string sqlName, params object[] nameValues)
        {
            SqlItem sql = GetSqlItem(sqlName);
            if(sql == null)
            {
                throw new Exception($"命令[{sqlName}]不存在");
            }
            return sql.ExecuteNoneQuery(nameValues);
        }

        public static object ExecuteScalar(string sqlName, params object[] nameValues)
        {
            SqlItem sql = GetSqlItem(sqlName);
            if(sql == null)
            {
                throw new Exception($"命令[{sqlName}]不存在");
            }
            return sql.ExecuteScalar(nameValues);
        }
    }
}
