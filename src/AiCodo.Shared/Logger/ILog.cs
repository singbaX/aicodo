/* 
 * author      : singba 
 * email       : singba@163.com 
 * version     : 20210831
 * package     : AiCodo
 * license     : MIT
 * description : let me think a while
 */
namespace AiCodo
{
    using System;
    using System.Threading;

    public delegate void LogDelegate(object sender, string msg, Category category = Category.Info);


    public enum Priority
    {
        /// <summary>
        /// 没有指定
        /// </summary>
        None = 0,

        /// <summary>
        /// 高级
        /// </summary>
        High = 1,

        /// <summary>
        /// 中级
        /// </summary>
        Medium = 2,

        /// <summary>
        /// 低级
        /// </summary>
        Low = 3,
    }

    /// <summary>
    /// 错误类别
    /// </summary>
    [Flags]
    public enum Category
    {
        /// <summary>
        /// 调试
        /// </summary>
        Debug = 1,

        /// <summary>
        /// 异常
        /// </summary>
        Exception = 2,

        /// <summary>
        /// 信息
        /// </summary>
        Info = 4,

        /// <summary>
        /// 警告
        /// </summary>
        Warn = 8,

        /// <summary>
        /// 致命错误
        /// </summary>
        Fatal = 16,
        /// <summary>
        /// 所有
        /// </summary>
        All = 31
    }

}
