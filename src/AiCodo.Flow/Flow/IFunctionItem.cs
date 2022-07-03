// *************************************************************************************************
// 代码文件：FunctionConfig.cs
// 
// 配置文件：FunctionConfig.xml
// 
// 作    者：SingbaX
// 
// 创建日期：202206
// 
// 功能说明：算法定义配置文件的对应
//
// 修改记录：
// *************************************************************************************************

using System.Collections.Generic;
using System.Reflection;

namespace AiCodo.Flow.Configs
{
    public interface IFunctionItem
    {
        string DisplayName { get; set; }
        string Name { get; set; }
        IEnumerable<ParameterItem> GetParameters();
        IEnumerable<ResultParameter> GetResultParameter();
    }
}